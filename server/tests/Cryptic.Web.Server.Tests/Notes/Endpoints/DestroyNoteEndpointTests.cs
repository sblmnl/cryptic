using System.Text.Json;
using Cryptic.Web.Server.Tests.Helpers;
using Microsoft.AspNetCore.Http;

namespace Cryptic.Web.Server.Tests.Notes.Endpoints;

public class DestroyNoteEndpointTests
{
    private readonly FakeCommandMediator _mediator = new();
    private readonly NoteId _noteId = NoteId.New();

    private Task<IResult> HandleRequest(string? controlToken)
    {
        var ctx = EndpointTestHelper.CreateHttpContext();
        return DestroyNoteHttpEndpoint.HandleRequest(_noteId.Value, controlToken, _mediator, ctx);
    }

    [Fact]
    public async Task HandleRequest_WithValidControlToken_Returns200()
    {
        _mediator.Setup<DestroyNoteCommand, Result>(_ => Result.Ok());

        var token = ControlToken.Create();
        var result = await HandleRequest(token.ToString());
        var (statusCode, body) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status200OK, statusCode);
        Assert.Equal("ok", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task HandleRequest_WithNullControlToken_Returns403()
    {
        var result = await HandleRequest(null);
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status403Forbidden, statusCode);
        Assert.Empty(_mediator.SentCommands);
    }

    [Fact]
    public async Task HandleRequest_WithInvalidBase64ControlToken_Returns403()
    {
        var result = await HandleRequest("not-valid-base64!!!");
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status403Forbidden, statusCode);
        Assert.Empty(_mediator.SentCommands);
    }

    [Fact]
    public async Task HandleRequest_WithNoteNotFound_Returns404()
    {
        _mediator.Setup<DestroyNoteCommand, Result>(
            _ => Result.Fail(new NoteNotFoundError(_noteId)));

        var token = ControlToken.Create();
        var result = await HandleRequest(token.ToString());
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
    }

    [Fact]
    public async Task HandleRequest_WithIncorrectControlToken_Returns403()
    {
        _mediator.Setup<DestroyNoteCommand, Result>(
            _ => Result.Fail(new IncorrectNoteControlTokenError(_noteId)));

        var token = ControlToken.Create();
        var result = await HandleRequest(token.ToString());
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status403Forbidden, statusCode);
    }

    [Fact]
    public async Task HandleRequest_WithUnrecognizedError_Returns500()
    {
        _mediator.Setup<DestroyNoteCommand, Result>(_ => Result.Fail("unknown error"));

        var token = ControlToken.Create();
        var result = await HandleRequest(token.ToString());
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact]
    public async Task HandleRequest_PassesParsedControlTokenToCommand()
    {
        _mediator.Setup<DestroyNoteCommand, Result>(_ => Result.Ok());

        var token = ControlToken.Create();
        await HandleRequest(token.ToString());

        var capturedCommand = _mediator.GetSentCommand<DestroyNoteCommand>();
        Assert.NotNull(capturedCommand);
        Assert.Equal(_noteId, capturedCommand!.NoteId);
        Assert.Equal(token.Value, capturedCommand.ControlToken.Value);
    }
}
