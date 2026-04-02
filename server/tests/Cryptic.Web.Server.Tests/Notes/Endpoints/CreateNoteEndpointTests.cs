using System.Text.Json;
using Cryptic.Web.Server.Tests.Helpers;
using Microsoft.AspNetCore.Http;

namespace Cryptic.Web.Server.Tests.Notes.Endpoints;

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
        var noteId = NoteId.New();
        var controlToken = ControlToken.Create();

        _mediator.Setup<CreateNoteCommand, Result<CreateNoteResponse>>(
            _ => Result.Ok(new CreateNoteResponse
            {
                NoteId = noteId,
                ControlToken = controlToken,
            }));

        var request = new CreateNoteHttpRequest
        {
            Content = "Hello, world!",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await HandleRequest(request);
        var (statusCode, body) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status201Created, statusCode);

        var data = body.GetProperty("data");
        Assert.Equal(noteId.Value.ToString(), data.GetProperty("noteId").GetString());
        Assert.Equal(controlToken.ToString(), data.GetProperty("controlToken").GetString());
    }

    [Fact]
    public async Task HandleRequest_WithValidationError_Returns400()
    {
        _mediator.Setup<CreateNoteCommand, Result<CreateNoteResponse>>(
            _ => Result.Fail<CreateNoteResponse>(new NoteContentTooShortError()));

        var request = new CreateNoteHttpRequest
        {
            Content = "ab",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await HandleRequest(request);
        var (statusCode, body) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
        Assert.Equal("failed", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task HandleRequest_WithUnrecognizedError_Returns500()
    {
        _mediator.Setup<CreateNoteCommand, Result<CreateNoteResponse>>(
            _ => Result.Fail<CreateNoteResponse>("some unknown error"));

        var request = new CreateNoteHttpRequest
        {
            Content = "Hello",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await HandleRequest(request);
        var (statusCode, _) = await ResultAssertions.ExecuteAndDeserialize<JsonElement>(result);

        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact]
    public async Task HandleRequest_MapsRequestFieldsToCommand()
    {
        _mediator.Setup<CreateNoteCommand, Result<CreateNoteResponse>>(
            _ => Result.Ok(new CreateNoteResponse
            {
                NoteId = NoteId.New(),
                ControlToken = ControlToken.Create(),
            }));

        var request = new CreateNoteHttpRequest
        {
            Content = "test content",
            DeleteAfter = DeleteAfter.OneWeek,
            Password = "secret",
            ClientMetadata = "{\"kdf\":\"argon2\"}",
        };

        await HandleRequest(request);

        var capturedCommand = _mediator.GetSentCommand<CreateNoteCommand>();
        Assert.NotNull(capturedCommand);
        Assert.Equal("test content", capturedCommand!.Content);
        Assert.Equal(DeleteAfter.OneWeek, capturedCommand.DeleteAfter);
        Assert.Equal("secret", capturedCommand.Password);
        Assert.Equal("{\"kdf\":\"argon2\"}", capturedCommand.ClientMetadata);
    }
}
