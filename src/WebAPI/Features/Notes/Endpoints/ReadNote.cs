using System.Net;
using System.Text.Json;
using Cryptic.Shared.Features.Notes.ReadNote;
using Microsoft.AspNetCore.Mvc;

namespace Cryptic.WebAPI.Features.Notes;

public static class ReadNote
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
        [FromQuery] string? password,
        ISender sender,
        HttpContext ctx)
    {
        var command = new ReadNoteCommand
        {
            NoteId = noteId,
            Password = password
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
    
    public static void MapEndpoint(WebApplication app)
    {
        app.MapGet("/notes/{noteId:guid}", RequestHandler)
            .WithName("ReadNote")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}