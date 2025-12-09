using Cryptic.Core.Notes;
using Cryptic.Core.Notes.Commands;
using Cryptic.Core.Notes.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.Web.Server.Notes.Endpoints;

public static class DestroyNoteHttpEndpoint
{
    private static IResult HandleFailure(Result result)
    {
        var errors = result.Errors.OfType<CodedError>().ToList();

        if (errors.Any(x => x is NoteNotFoundError))
        {
            return HttpResponses.Fail(errors, StatusCodes.Status404NotFound);
        }

        if (errors.Any(x => x is IncorrectNoteControlTokenError))
        {
            return HttpResponses.Fail(errors, StatusCodes.Status403Forbidden);
        }

        return HttpResponses.Fail(errors, StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleRequest(
        [FromRoute] Guid id,
        [FromQuery] string? controlToken,
        [FromServices] ICommandMediator mediator,
        HttpContext ctx)
    {
        var noteId = new NoteId(id);

        if (controlToken is null
            || !ControlToken.TryParse(controlToken, out var parsedControlToken)
            || parsedControlToken is null)
        {
            return HttpResponses.Fail(
                new IncorrectNoteControlTokenError(noteId),
                StatusCodes.Status403Forbidden);
        }

        var command = new DestroyNoteCommand
        {
            NoteId = noteId,
            ControlToken = parsedControlToken,
        };

        var result = await mediator.SendAsync(command, ctx.RequestAborted);

        return result.IsSuccess ? HttpResponses.Ok() : HandleFailure(result);
    }

    public static void MapDestroyNoteHttpEndpoint(this WebApplication app)
    {
        app.MapDelete("/api/notes/{id:guid}", HandleRequest)
            .WithName("DestroyNote")
            .Produces<OkHttpResponseBody>()
            .Produces<FailedHttpResponseBody>(StatusCodes.Status404NotFound)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status401Unauthorized)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
