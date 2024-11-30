using Cryptic.Core.Features.Notes.Commands;

namespace Cryptic.Web.Features.Notes;

public record CreateNoteRequest(
    string? Content,
    DateTimeOffset? DeleteAfterTime,
    bool DeleteOnReceipt,
    string? Password);

public record CreateNoteResponseBody
{
    public required Guid Id { get; init; }
    public required DateTimeOffset DeleteAfterTime { get; init; }
    public required bool DeleteOnReceipt { get; init; }
    public required string ControlToken { get; init; }
}

public static class CreateNoteEndpoint
{
    public static IResult HandleFailure(Result<CreateNoteResponse>.Fail failureResult)
    {
        if (failureResult.Error == CreateNoteErrors.DeleteAfterAlreadyPassed)
        {
            return Results.BadRequest(new ApiResponseBody
            {
                Errors = [CreateNoteErrors.DeleteAfterAlreadyPassed]
            });
        }

        return ApiResponses.InternalError;
    }

    public static async Task<IResult> CreateNoteRequestHandler(CreateNoteRequest req, ISender sender, HttpContext ctx)
    {
        var command = new CreateNoteCommand
        {
            Content = req.Content ?? "",
            DeleteAfterTime = req.DeleteAfterTime ?? DateTimeOffset.UtcNow + TimeSpan.FromDays(30),
            DeleteOnReceipt = req.DeleteOnReceipt,
            Password = req.Password
        };

        var result = await sender.Send(command, ctx.RequestAborted);

        if (result is Result<CreateNoteResponse>.Fail failureResult)
        {
            return HandleFailure(failureResult);
        }

        var response = (result as Result<CreateNoteResponse>.Ok)!.Value;
        var note = response.Note;

        return Results.Created(new Uri(note.Id.ToString(), UriKind.Relative), new ApiResponseBody
        {
            Data = new CreateNoteResponseBody
            {
                Id = note.Id,
                DeleteAfterTime = note.DeleteAfter.Time,
                DeleteOnReceipt = note.DeleteAfter.Receipt,
                ControlToken = response.ControlToken
            },
            Meta = new MetaInformation()
            {
                Links =
                [
                    note.GetReadNoteLink(ctx.Request),
                    note.GetDestroyNoteLink(ctx.Request)
                ]
            }
        });
    }

    public static void Map(WebApplication app)
    {
        app.MapPost("/api/notes", CreateNoteRequestHandler)
            .WithName("CreateNote")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }
}
