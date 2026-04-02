using System.Text.Json;
using Cryptic.Web.Server.Tests.Helpers;
using Microsoft.AspNetCore.Http;

namespace Cryptic.Web.Server.Tests.Notes.Endpoints;

public class ReadNoteEndpointTests
{
    private readonly FakeCommandMediator _mediator = new();
    private readonly NoteId _noteId = NoteId.New();

    private Task<IResult> HandleRequest(string? password = null)
    {
        var ctx = EndpointTestHelper.CreateHttpContext();
        return ReadNoteHttpEndpoint.HandleRequest(_noteId.Value, password, _mediator, ctx);
    }

    [Fact]
    public async Task HandleRequest_WithSuccess_Returns200WithContent()
    {
        _mediator.Setup<ReadNoteCommand, Result<ReadNoteResponse>>(
            _ => Result.Ok(new ReadNoteResponse
            {
                NoteId = _noteId,
                Content = "secret message",
                Destroyed = false,
                ClientMetadata = "{\"kdf\":\"argon2\"}",
            }));

        var result = await HandleRequest();
        var (statusCode, body) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status200OK, statusCode);
        var data = body.GetProperty("data");
        Assert.Equal("secret message", data.GetProperty("content").GetString());
        Assert.False(data.GetProperty("destroyed").GetBoolean());
        Assert.Equal("{\"kdf\":\"argon2\"}", data.GetProperty("clientMetadata").GetString());
    }

    [Fact]
    public async Task HandleRequest_WithNoteNotFound_Returns404()
    {
        _mediator.Setup<ReadNoteCommand, Result<ReadNoteResponse>>(
            _ => Result.Fail<ReadNoteResponse>(new NoteNotFoundError(_noteId)));

        var result = await HandleRequest();
        var (statusCode, body) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
        Assert.Equal("failed", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task HandleRequest_WithPasswordNotProvided_Returns401()
    {
        _mediator.Setup<ReadNoteCommand, Result<ReadNoteResponse>>(
            _ => Result.Fail<ReadNoteResponse>(new NotePasswordNotProvidedError(_noteId)));

        var result = await HandleRequest();
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status401Unauthorized, statusCode);
    }

    [Fact]
    public async Task HandleRequest_WithIncorrectPassword_Returns401()
    {
        _mediator.Setup<ReadNoteCommand, Result<ReadNoteResponse>>(
            _ => Result.Fail<ReadNoteResponse>(new IncorrectNotePasswordError(_noteId)));

        var result = await HandleRequest("wrong");
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status401Unauthorized, statusCode);
    }

    [Fact]
    public async Task HandleRequest_WithUnrecognizedError_Returns500()
    {
        _mediator.Setup<ReadNoteCommand, Result<ReadNoteResponse>>(
            _ => Result.Fail<ReadNoteResponse>("unknown error"));

        var result = await HandleRequest();
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact]
    public async Task HandleRequest_PassesPasswordToCommand()
    {
        _mediator.Setup<ReadNoteCommand, Result<ReadNoteResponse>>(
            _ => Result.Ok(new ReadNoteResponse
            {
                NoteId = _noteId,
                Content = "test",
                Destroyed = false,
            }));

        await HandleRequest("my-password");

        var capturedCommand = _mediator.GetSentCommand<ReadNoteCommand>();
        Assert.NotNull(capturedCommand);
        Assert.Equal(_noteId, capturedCommand!.NoteId);
        Assert.Equal("my-password", capturedCommand.Password);
    }
}
