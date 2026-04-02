namespace Cryptic.Core.Tests.Notes;

public class DestroyNoteHandlerTests
{
    [Fact]
    public async Task HandleAsync_NoteNotFound_ReturnsNoteNotFoundError()
    {
        await using var db = new TestAppDbContext();
        var handler = new DestroyNoteCommandHandler(db);
        var command = new DestroyNoteCommand
        {
            NoteId = NoteId.New(),
            ControlToken = TestFixtures.CreateControlToken(),
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteNotFoundError>());
    }

    [Fact]
    public async Task HandleAsync_WrongControlToken_ReturnsIncorrectControlTokenError()
    {
        await using var db = new TestAppDbContext();
        var token = TestFixtures.CreateControlToken();
        var hash = TestFixtures.CreateControlTokenHash(token);
        var note = TestFixtures.CreateNote(controlTokenHash: hash).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new DestroyNoteCommandHandler(db);
        var command = new DestroyNoteCommand
        {
            NoteId = note.Id,
            ControlToken = TestFixtures.CreateControlToken(), // different token
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<IncorrectNoteControlTokenError>());
    }

    [Fact]
    public async Task HandleAsync_CorrectControlToken_ReturnsOk()
    {
        await using var db = new TestAppDbContext();
        var token = TestFixtures.CreateControlToken();
        var hash = TestFixtures.CreateControlTokenHash(token);
        var note = TestFixtures.CreateNote(controlTokenHash: hash).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new DestroyNoteCommandHandler(db);
        var command = new DestroyNoteCommand
        {
            NoteId = note.Id,
            ControlToken = token,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_CorrectControlToken_RemovesNoteFromDatabase()
    {
        await using var db = new TestAppDbContext();
        var token = TestFixtures.CreateControlToken();
        var hash = TestFixtures.CreateControlTokenHash(token);
        var note = TestFixtures.CreateNote(controlTokenHash: hash).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new DestroyNoteCommandHandler(db);
        var command = new DestroyNoteCommand
        {
            NoteId = note.Id,
            ControlToken = token,
        };

        await handler.HandleAsync(command);

        Assert.Null(await db.Notes.FindAsync(note.Id));
    }
}