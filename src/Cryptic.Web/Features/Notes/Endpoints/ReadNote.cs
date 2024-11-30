using Cryptic.Core.Features.Notes.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.Web.Features.Notes;

public record ReadNoteRequest(string? Password);

public static class ReadNoteEndpoint
{
    public static IResult HandleFailure(Result<ReadNoteResponse>.Fail failureResult)
    {
        var responseBody = new ApiResponseBody
        {
            Errors = [ failureResult.Error ]
        };
        
        if (failureResult.Error == ReadNoteErrors.NoteNotFound)
        {
            return Results.Json(responseBody, statusCode: StatusCodes.Status404NotFound);
        }
        
        if (failureResult.Error == ReadNoteErrors.IncorrectPassword)
        {
            return Results.Json(responseBody, statusCode: StatusCodes.Status403Forbidden);
        }
        
        return ApiResponses.InternalError;
    }
    
    public static async Task<IResult> RequestHandler(
        [FromRoute] Guid noteId,
        [FromBody] ReadNoteRequest req,
        ISender sender,
        HttpContext ctx)
    {
        var command = new ReadNoteCommand
        {
            NoteId = noteId,
            Password = req.Password
        };
        
        var result = await sender.Send(command);
        
        if (result is Result<ReadNoteResponse>.Fail failureResult)
        {
            return HandleFailure(failureResult);
        }
        
        var response = (result as Result<ReadNoteResponse>.Ok)!.Value;

        return Results.Ok(new ApiResponseBody
        {
            Data = response.Content
        });
    }
    
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/notes/{noteId:guid}/read", RequestHandler)
            .WithName("ReadNote")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
