namespace Cryptic.Core.Tests.Notes;

public class CreateNoteHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsNoteIdAndControlToken()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "Hello, world!",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(default, result.Value.NoteId);
        Assert.NotNull(result.Value.ControlToken);
        Assert.Equal(16, result.Value.ControlToken.Value.Length);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PersistsNoteToDatabase()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "Persisted note",
            DeleteAfter = DeleteAfter.OneWeek,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var saved = await db.Notes.FindAsync(result.Value.NoteId);
        Assert.NotNull(saved);
        Assert.Equal("Persisted note", saved.Content);
    }

    [Fact]
    public async Task HandleAsync_ContentTooShort_ReturnsFailure()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "ab",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteContentTooShortError>());
    }

    [Fact]
    public async Task HandleAsync_ContentTooLong_ReturnsFailure()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = new string('x', 16_385),
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteContentTooLongError>());
    }

    [Fact]
    public async Task HandleAsync_ClientMetadataTooLong_ReturnsFailure()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "Valid content",
            DeleteAfter = DeleteAfter.OneDay,
            ClientMetadata = new string('m', 1_025),
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteClientMetadataTooLongError>());
    }

    [Fact]
    public async Task HandleAsync_WithPassword_PersistsPasswordHash()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "Secret note",
            DeleteAfter = DeleteAfter.OneDay,
            Password = "hunter2",
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var saved = await db.Notes.FindAsync(result.Value.NoteId);
        Assert.NotNull(saved!.PasswordHash);
    }

    [Fact]
    public async Task HandleAsync_WithoutPassword_LeavesPasswordHashNull()
    {
        await using var db = new TestAppDbContext();
        var handler = new CreateNoteCommandHandler(db);
        var command = new CreateNoteCommand
        {
            Content = "Public note",
            DeleteAfter = DeleteAfter.OneDay,
        };

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var saved = await db.Notes.FindAsync(result.Value.NoteId);
        Assert.Null(saved!.PasswordHash);
    }
}