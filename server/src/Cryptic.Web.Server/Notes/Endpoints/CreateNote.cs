using Cryptic.Core.Notes;
using Cryptic.Core.Notes.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.Web.Server.Notes.Endpoints;

public class CreateNoteHttpRequest
{
    public required string Content { get; init; }
    public DeleteAfter DeleteAfter { get; init; }
    public string? Password { get; init; }
}

public class CreateNoteHttpResponseBody
{
    public required NoteId NoteId { get; init; }
    public required string ControlToken { get; init; }
}

public static class CreateNoteHttpEndpoint
{
    private static IResult HandleFailure(Result<CreateNoteResponse> result)
    {
        var errors = result.Errors.OfType<CodedError>().ToList();

        return errors.Count > 0
            ? HttpResponses.Fail(errors, StatusCodes.Status400BadRequest)
            : HttpResponses.InternalError;
    }

    public static async Task<IResult> HandleRequest(
        [FromBody] CreateNoteHttpRequest request,
        [FromServices] ICommandMediator mediator,
        HttpContext ctx)
    {
        var command = new CreateNoteCommand
        {
            Content = request.Content,
            DeleteAfter = request.DeleteAfter,
            Password = request.Password,
        };

        var result = await mediator.SendAsync(command, ctx.RequestAborted);

        if (result.IsFailed)
        {
            return HandleFailure(result);
        }

        var commandResponse = result.Value;
        var httpResponseBody = new CreateNoteHttpResponseBody
        {
            NoteId = commandResponse.NoteId,
            ControlToken = commandResponse.ControlToken.ToString(),
        };

        return HttpResponses.Ok(httpResponseBody, StatusCodes.Status201Created);
    }

    public static void MapCreateNoteHttpEndpoint(this WebApplication app)
    {
        app.MapPost("/api/notes", HandleRequest)
            .WithName("CreateNote")
            .Produces<OkHttpResponseBody<CreateNoteHttpResponseBody>>(StatusCodes.Status201Created)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status400BadRequest)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
