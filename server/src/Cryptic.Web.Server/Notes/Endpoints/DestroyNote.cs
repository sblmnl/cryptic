using Cryptic.Core.Notes;
using Cryptic.Core.Notes.Commands;
using Cryptic.Core.Notes.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.Web.Server.Notes.Endpoints;

public class DestroyNoteHttpRequest
{
    public NoteId NoteId { get; init; }
    public string? ControlToken { get; init; }
}

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
        [FromBody] DestroyNoteHttpRequest request,
        [FromServices] ICommandMediator mediator,
        HttpContext ctx)
    {
        if (request.ControlToken is null
            || !ControlToken.TryParse(request.ControlToken, out var controlToken)
            || controlToken is null)
        {
            return HttpResponses.Fail(
                new IncorrectNoteControlTokenError(request.NoteId),
                StatusCodes.Status403Forbidden);
        }

        var command = new DestroyNoteCommand
        {
            NoteId = request.NoteId,
            ControlToken = controlToken,
        };

        var result = await mediator.SendAsync(command, ctx.RequestAborted);

        return result.IsSuccess ? HttpResponses.Ok() : HandleFailure(result);
    }

    public static void MapDestroyNoteHttpEndpoint(this WebApplication app)
    {
        app.MapDelete("/api/notes", HandleRequest)
            .WithName("DestroyNote")
            .Produces<OkHttpResponseBody>()
            .Produces<FailedHttpResponseBody>(StatusCodes.Status404NotFound)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status401Unauthorized)
            .Produces<FailedHttpResponseBody>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
