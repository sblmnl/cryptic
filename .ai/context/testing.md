# Testing Context

See [context.md](context.md) for the high-level project overview and [development.md](development.md) for code style and patterns.

## Backend Testing (C# / xUnit)

### Test Projects

| Project | Purpose |
|---------|---------|
| `server/tests/Cryptic.Core.Tests` | Domain unit tests — command handlers, value objects, domain logic |
| `server/tests/Cryptic.Web.Server.Tests` | Endpoint tests — HTTP handler logic, error mapping, request/response shape |

### Frameworks & Libraries
- **xUnit** 2.9.3 — test runner and assertions (`Assert.*`)
- **Moq** 4.20.72 — mocking (used sparingly; most tests use real implementations)

### Global Usings
Each test project has a `GlobalUsings.cs` that imports commonly needed namespaces so individual test files stay clean.

### Domain Tests (`Cryptic.Core.Tests`)

Use `TestAppDbContext` (in `Helpers/`) which creates an isolated in-memory EF Core database per test instance:

```csharp
public class CreateNoteHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsNoteIdAndControlToken()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "Hello, world!",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(default, result.Value.NoteId);
    }

    [Fact]
    public async Task HandleAsync_ContentTooShort_ReturnsFailure()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand { Content = "ab", DeleteAfter = DeleteAfter.OneDay };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteContentTooShortError>());
    }
}
```

**Conventions:**
- Test method names: `MethodName_Condition_ExpectedOutcome` (e.g. `HandleAsync_ValidCommand_ReturnsNoteIdAndControlToken`)
- Always `await using` `TestAppDbContext` (it is `IAsyncDisposable`)
- Assert on `result.IsSuccess` / `result.IsFailed` before accessing `result.Value`
- Use `result.Errors.OfType<TError>()` to assert on specific typed errors
- Verify persisted state by querying `db.*` after the handler call

### Endpoint Tests (`Cryptic.Web.Server.Tests`)

Use `FakeCommandMediator` (in `Helpers/`) to inject controlled command results without exercising domain logic:

```csharp
public class CreateNoteEndpointTests
{
    private readonly FakeCommandMediator _mediator = new();

    private Task<IResult> HandleRequest(CreateNoteHttpRequest request)
    {
        var ctx = EndpointTestHelper.CreateHttpContext();
        return CreateNoteHttpEndpoint.HandleRequest(request, _mediator, ctx);
    }

    [Fact]
    public async Task HandleRequest_WithValidCommand_Returns201WithNoteIdAndControlToken()
    {
        _mediator.Setup<CreateNoteCommand, Result<CreateNoteResponse>>(
            _ => Result.Ok(new CreateNoteResponse { NoteId = NoteId.New(), ControlToken = ControlToken.Create() }));

        var result = await HandleRequest(new CreateNoteHttpRequest { Content = "Hello", DeleteAfter = DeleteAfter.OneDay });
        var (statusCode, body) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status201Created, statusCode);
        var data = body.GetProperty("data");
        Assert.NotNull(data.GetProperty("noteId").GetString());
    }
}
```

**Helpers:**
- `FakeCommandMediator` — setup command responses and capture sent commands (`GetSentCommand<T>()`)
- `EndpointTestHelper.CreateHttpContext()` — creates a minimal `HttpContext` for tests
- `ResultAssertions.ExecuteAndDeserialize<T>(result)` — executes an `IResult` and returns `(statusCode, deserializedBody)`

**Conventions:**
- Test each HTTP status code path (success, domain error → 400, unrecognized error → 500)
- Use `_mediator.GetSentCommand<TCommand>()` to verify the endpoint correctly maps request fields to the command

---

## Frontend Testing

### Unit Tests (Vitest)

- **Framework**: Vitest 4.1.2
- **Location**: `client/tests/unit/`
- **Config**: `client/vitest.config.ts` (globals enabled, `@/` alias)
- **Run**: `npm run test:unit` (from `client/`)

Test files mirror the `src/` structure under `tests/unit/` — the `app/`, `features/`, and `shared/` zones map directly:

| Source file | Test file |
|------------|-----------|
| `src/features/notes/note.ts` | `tests/unit/features/notes/note.test.ts` |
| `src/shared/util/crypto/aes-gcm.ts` | `tests/unit/shared/util/crypto/aes-gcm.test.ts` |
| `src/shared/types/delete-after.ts` | `tests/unit/shared/types/delete-after.test.ts` |

**Conventions:**

- Globals enabled — no need to import `describe`, `it`, `expect`; import `vi` and lifecycle hooks explicitly
- Use `vi.mock()` at the top of the file (before imports) to mock heavy dependencies (e.g. libsodium)
- Use `afterEach(() => vi.restoreAllMocks())` to clean up mocks
- Mock libsodium with a deterministic fake KDF to keep unit tests fast:

```typescript
vi.mock("libsodium-wrappers-sumo", () => ({
  default: {
    ready: Promise.resolve(),
    crypto_pwhash: (keyLen: number, password: string) => {
      const key = new Uint8Array(keyLen);
      for (let i = 0; i < keyLen; i++) key[i] = password.charCodeAt(i % password.length) ^ (i * 37);
      return key;
    },
    crypto_pwhash_ALG_ARGON2ID13: 2,
  },
}));
```

- Argon2/libsodium is always mocked in unit tests to avoid slow KDF execution
- Test both the happy path and edge cases (empty password, unicode content, wrong password, JSON serialization round-trips)

### End-to-End Tests (Playwright)

- **Framework**: Playwright 1.59.0
- **Location**: `client/tests/e2e/`
- **Config**: `client/playwright.config.ts`
- **Run**: `npm run test:e2e` or `npm run test:e2e:ui` (from `client/`)

**Configuration:**

- Base URL: `https://localhost:5173`
- Browser: Desktop Chrome only
- HTTPS errors ignored (local dev cert)
- Reporter: HTML
- Retries: 2 in CI, 0 locally
- Trace on first retry
- Automatically launches the Quasar dev server (`npm run dev`) before running tests

---

## Running Tests

### Backend

```sh
# From repo root or server/
dotnet test                                      # all test projects
dotnet test server/tests/Cryptic.Core.Tests      # domain tests only
dotnet test server/tests/Cryptic.Web.Server.Tests # endpoint tests only
```

### Frontend

```sh
# From client/
npm run test:unit        # Vitest unit tests (watch mode)
npm run test:unit -- --run  # single run (CI)
npm run test:e2e         # Playwright e2e (requires running server)
npm run test:e2e:ui      # Playwright UI mode
```
