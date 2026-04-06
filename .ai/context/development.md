# Project Development Context

See [context.md](context.md) for the high-level project overview and tech stack.

## C# Code Style

### Formatting

- **Indent**: 4 spaces (tabs for `.sln` files)
- **Max line length**: 120 characters
- **Line endings**: LF
- **Namespaces**: file-scoped (`namespace Foo.Bar;`)
- **Braces**: always on new line (`csharp_new_line_before_open_brace = all`)
- **`using` directives**: outside namespace

### Naming

| Construct               | Convention                       |
|-------------------------|----------------------------------|
| Private/internal fields | `_camelCase`                     |
| Constants               | `PascalCase`                     |
| Everything else         | `PascalCase` (standard C# rules) |

### `var` Usage

- Do **not** use `var` for built-in types (`string`, `int`, etc.) — use the predefined type name
- Use `var` when the type is apparent from the right-hand side
- Use `var` elsewhere (implicit)

### Preferred Patterns

- Expression-bodied members (properties, operators, simple methods)
- Pattern matching over `is`-with-cast and `as`-with-null-check
- Null propagation (`?.`, `??`, `??=`)
- Object and collection initializers
- Switch expressions over switch statements
- `readonly` fields where possible
- No primary constructors (`csharp_style_prefer_primary_constructors = false`)
- No collection expressions (`dotnet_style_prefer_collection_expression = never`)

---

## C# Architecture Patterns

### Command / Handler (LiteBus)

All business operations are expressed as commands dispatched through `ICommandMediator`.

```csharp
// Command — defines the operation's input
public class CreateNoteCommand : ICommand<Result<CreateNoteResponse>>
{
    public required string Content { get; init; }
    public DeleteAfter DeleteAfter { get; init; }
    public string? Password { get; init; }
}

// Handler — executes the operation, returns Result<T>
public class CreateNoteCommandHandler : ICommandHandler<CreateNoteCommand, Result<CreateNoteResponse>>
{
    private readonly AppDbContext _db;

    public CreateNoteCommandHandler(AppDbContext db) => _db = db;

    public async Task<Result<CreateNoteResponse>> HandleAsync(
        CreateNoteCommand message,
        CancellationToken cancellationToken = default)
    {
        // ... business logic ...
        return result.Map(x => new CreateNoteResponse { ... });
    }
}
```

Handlers are auto-registered from the assembly via `AddLiteBusModules()`.

### Result Pattern (FluentResults)

Never throw for expected failures — return `Result<T>`.

```csharp
// Success
return Result.Ok(new CreateNoteResponse { ... });

// Failure with typed error
return Result.Fail<CreateNoteResponse>(new NoteContentTooShortError());

// Propagate failure from inner result
if (result.IsFailed)
    return result.ToResult<OuterType>();

// Transform success value
return result.Map(x => new OtherType { ... });
```

All domain errors inherit from `CodedError` (in `Cryptic.Core.Common.Errors`), which carries an error code and optional metadata dictionary.

### HTTP Endpoints (Minimal API)

Endpoints are defined as `static` classes with a `HandleRequest` static method and a `Map*HttpEndpoint` extension on `WebApplication`.

```csharp
public static class CreateNoteHttpEndpoint
{
    public static async Task<IResult> HandleRequest(
        [FromBody] CreateNoteHttpRequest request,
        [FromServices] ICommandMediator mediator,
        HttpContext ctx)
    {
        var command = new CreateNoteCommand { ... };
        var result = await mediator.SendAsync(command, ctx.RequestAborted);

        if (result.IsFailed)
            return HandleFailure(result);  // maps CodedError → 400/500

        return HttpResponses.Ok(new CreateNoteHttpResponseBody { ... }, StatusCodes.Status201Created);
    }

    public static void MapCreateNoteHttpEndpoint(this WebApplication app)
    {
        app.MapPost("/api/notes", HandleRequest)
            .WithName("CreateNote")
            .Produces<OkHttpResponseBody<CreateNoteHttpResponseBody>>(StatusCodes.Status201Created)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status400BadRequest)
            .WithOpenApi();
    }
}
```

HTTP status mapping convention:

- `400` — domain/validation errors (recognized `CodedError`)
- `401` — authentication failures
- `403` — authorization failures
- `404` — not found
- `500` — unrecognized errors or unhandled exceptions

### EF Core

- `AppDbContext` in `Cryptic.Core.Persistence`
- Entity configuration via `IEntityTypeConfiguration<T>` inner classes
- snake_case naming via `EFCore.NamingConventions`
- Migrations in `Cryptic.Core/Persistence/Migrations/`

---

## TypeScript / Vue Code Style

### Formatting (Prettier)

- **Print width**: 120 characters
- **Line endings**: LF
- **Import order**: auto-sorted by `prettier-plugin-organize-imports`

### Linting (ESLint flat config)

- `@typescript-eslint/consistent-type-imports` — use `import type` for type-only imports
- `@typescript-eslint/no-explicit-any` — disabled
- `no-debugger` — error in production, disabled in dev

### TypeScript Config

- Strict mode enabled
- `noUnusedLocals: true`, `noUnusedParameters: true`
- `noFallthroughCasesInSwitch: true`
- Path alias: `@/` → `./src/`
    - Use when import crosses technical-concern/feature/module boundaries
    - Use when relative path access exceeds 2 directory levels
    - Do not use when importing nearby files that are closesly related to the current technical-concern/feature/module

---

## Vue / Frontend Patterns

### Component Structure

- Always use `<script setup>` (Composition API)
- Event handler naming: `on<Action>` (e.g. `onCreateBtnClick`, `onCancelBtnClick`)
- Emit events for parent communication; avoid global state for component-level concerns

### Source Layout (Vertical Slices)

The frontend follows a three-zone vertical slice layout:

| Zone | Path | Purpose |
|------|------|---------|
| App shell | `src/` (boot, components, css, layouts, pages, router) | Boot files, layouts, app-level pages, router, global styles |
| Feature slices | `src/features/<name>/` | Everything for one domain feature: `api/`, `components/`, `pages/`, and root-level `.ts` files |
| Shared | `src/shared/` | Cross-feature types, utilities, and reusable components |

Each feature slice (`src/features/<name>/`) contains:
- `api/` — HTTP request/response interfaces **and** `send*Request` functions (the only place axios is called for that feature)
- `components/` — Vue components scoped to that feature
- `pages/` — Route-level page components
- `*.ts` at feature root — Domain/business logic (encryption, model construction, etc.)

### API Calls

All HTTP calls for a feature live in its `api/` folder as named `send*Request` functions. Never call axios directly from a page or component.

```typescript
// features/notes/api/create-note.ts
export async function sendCreateNoteRequest(body: CreateNoteHttpRequest): Promise<CreateNoteHttpResponse> {
  const res = await api.post<OkHttpResponseBody<CreateNoteHttpResponse>>("/notes", body);
  return res.data.data;
}

// In a component — import and call the function; catch axios errors for UI feedback
import { sendCreateNoteRequest } from "@/features/notes/api/create-note";

try {
  const data = await sendCreateNoteRequest(reqBody);
} catch (err) {
  if (isAxiosError<FailedHttpResponseBody>(err)) {
    const errors = err.response?.data.errors ?? [];
  }
}
```

The `api` Axios instance (base URL `/api`) is created in `src/boot/axios.ts`:

```typescript
import { api } from "@/boot/axios";
```

### Router

- Routes defined in `src/router/routes.ts` with lazy-loaded page components
- Route meta: `title?: string`, `layout?: AppLayout`
- Router mode (`hash` or `history`) controlled by `VITE_ROUTER_MODE` env var
