using Cryptic.Shared.Features.Notes.DestroyNote;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.WebAPI.Features.Notes;

public static class DestroyNote
{
    public record Request(string? ControlToken);
    
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
        [FromBody] Request req,
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
    
    public static void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/notes/{noteId:guid}", RequestHandler)
            .WithName("DestroyNote")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}