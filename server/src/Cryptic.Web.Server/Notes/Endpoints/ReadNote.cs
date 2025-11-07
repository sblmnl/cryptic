using Cryptic.Core.Notes;
using Cryptic.Core.Notes.Commands;
using Cryptic.Core.Notes.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.Web.Server.Notes.Endpoints;

public class ReadNoteHttpRequest
{
    public required NoteId NoteId { get; init; }
    public string? Password { get; init; }
}

public class ReadNoteHttpResponseBody
{
    public required NoteId NoteId { get; init; }
    public required string Content { get; init; }
    public required bool Destroyed { get; init; }
}

public static class ReadNoteHttpEndpoint
{
    private static IResult HandleFailure(Result<ReadNoteResponse> result)
    {
        var errors = result.Errors.OfType<CodedError>().ToList();

        if (errors.Any(x => x is NoteNotFoundError))
        {
            return HttpResponses.Fail(errors, StatusCodes.Status404NotFound);
        }

        if (errors.Any(x => x is NotePasswordNotProvidedError or IncorrectNotePasswordError))
        {
            return HttpResponses.Fail(errors, StatusCodes.Status401Unauthorized);
        }

        return HttpResponses.Fail(errors, StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleRequest(
        [FromBody] ReadNoteHttpRequest request,
        [FromServices] ICommandMediator mediator,
        HttpContext ctx)
    {
        var command = new ReadNoteCommand
        {
            NoteId = request.NoteId,
            Password = request.Password,
        };

        var result = await mediator.SendAsync(command, ctx.RequestAborted);

        if (result.IsFailed)
        {
            return HandleFailure(result);
        }

        var commandResponse = result.Value;

        return HttpResponses.Ok(new ReadNoteHttpResponseBody
        {
            NoteId = commandResponse.NoteId,
            Content = commandResponse.Content,
            Destroyed = commandResponse.Destroyed,
        });
    }

    public static void MapReadNoteHttpEndpoint(this WebApplication app)
    {
        app.MapPost("/api/notes/read", HandleRequest)
            .WithName("ReadNote")
            .Produces<OkHttpResponseBody<ReadNoteHttpResponseBody>>()
            .Produces<FailedHttpResponseBody>(StatusCodes.Status404NotFound)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status401Unauthorized)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
