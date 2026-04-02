namespace Cryptic.Core.Tests.Notes;

public class ReadNoteHandlerTests
{
    [Fact]
    public async Task HandleAsync_NoteNotFound_ReturnsNoteNotFoundError()
    {
        await using var db = new TestAppDbContext();
        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = NoteId.New() };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteNotFoundError>());
    }

    [Fact]
    public async Task HandleAsync_NoteExists_ReturnsContent()
    {
        await using var db = new TestAppDbContext();
        var note = TestFixtures.CreateNote("Read me!").Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(note.Id, result.Value.NoteId);
        Assert.Equal("Read me!", result.Value.Content);
        Assert.False(result.Value.Destroyed);
    }

    [Fact]
    public async Task HandleAsync_NoteWithPassword_NoPassword_ReturnsPasswordNotProvided()
    {
        await using var db = new TestAppDbContext();
        var passwordHash = TestFixtures.CreatePasswordHash("secret");
        var note = TestFixtures.CreateNote(passwordHash: passwordHash).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id, Password = null };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NotePasswordNotProvidedError>());
    }

    [Fact]
    public async Task HandleAsync_NoteWithPassword_WrongPassword_ReturnsIncorrectPassword()
    {
        await using var db = new TestAppDbContext();
        var passwordHash = TestFixtures.CreatePasswordHash("correct");
        var note = TestFixtures.CreateNote(passwordHash: passwordHash).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id, Password = "wrong" };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<IncorrectNotePasswordError>());
    }

    [Fact]
    public async Task HandleAsync_NoteWithPassword_CorrectPassword_ReturnsContent()
    {
        await using var db = new TestAppDbContext();
        var passwordHash = TestFixtures.CreatePasswordHash("correct");
        var note = TestFixtures.CreateNote(passwordHash: passwordHash).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id, Password = "correct" };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello, world!", result.Value.Content);
    }

    [Fact]
    public async Task HandleAsync_DeleteAfterViewing_DestroysNoteAndSetsDestroyedFlag()
    {
        await using var db = new TestAppDbContext();
        var note = TestFixtures.CreateNote(deleteAfter: DeleteAfter.Viewing).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Destroyed);
        Assert.Null(await db.Notes.FindAsync(note.Id));
    }

    [Fact]
    public async Task HandleAsync_DeleteAfterNonViewing_DoesNotDestroyNote()
    {
        await using var db = new TestAppDbContext();
        var note = TestFixtures.CreateNote(deleteAfter: DeleteAfter.OneDay).Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Destroyed);
        Assert.NotNull(await db.Notes.FindAsync(note.Id));
    }

    [Fact]
    public async Task HandleAsync_NoteWithClientMetadata_ReturnsClientMetadata()
    {
        await using var db = new TestAppDbContext();
        var note = TestFixtures.CreateNote(clientMetadata: "{\"kdf\":\"argon2\"}").Value;
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        var handler = new ReadNoteCommandHandler(db);
        var command = new ReadNoteCommand { NoteId = note.Id };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal("{\"kdf\":\"argon2\"}", result.Value.ClientMetadata);
    }
}