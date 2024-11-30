using Cryptic.Core.Features.Notes.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.Web.Features.Notes;

public record DestroyNoteRequest(string? ControlToken);

public static class DestroyNoteEndpoint
{
    public static IResult HandleFailure(Result<Nothing>.Fail failureResult)
    {
        if (failureResult.Error == DestroyNoteErrors.NoteNotFound
            || failureResult.Error == DestroyNoteErrors.IncorrectControlToken)
        {
            return Results.Json(new ApiResponseBody
            {
                Errors = [ DestroyNoteErrors.NoteNotFound ]
            }, statusCode: StatusCodes.Status404NotFound);
        }
        
        return ApiResponses.InternalError;
    }
    
    public static async Task<IResult> RequestHandler(
        [FromRoute] Guid noteId,
        [FromBody] DestroyNoteRequest req,
        ISender sender,
        HttpContext ctx)
    {
        var command = new DestroyNoteCommand
        {
            NoteId = noteId,
            ControlToken = req.ControlToken ?? ""
        };
        
        var result = await sender.Send(command);
        
        if (result is Result<Nothing>.Fail failureResult)
        {
            return HandleFailure(failureResult);
        }

        return Results.Ok(new ApiResponseBody());
    }
    
    public static void Map(WebApplication app)
    {
        app.MapDelete("/api/notes/{noteId:guid}", RequestHandler)
            .WithName("DestroyNote")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
