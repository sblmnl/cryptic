namespace Cryptic.Core.Tests.Notes;

public class NoteTests
{
    // --- Note.Create: content validation ---

    [Fact]
    public void Create_WithValidContent_Succeeds()
    {
        var result = TestFixtures.CreateNote("abc");
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    public void Create_WithContentTooShort_Fails(string content)
    {
        var token = TestFixtures.CreateControlToken();
        var hash = TestFixtures.CreateControlTokenHash(token);
        var result = Note.Create(content, DeleteAfter.OneDay, hash, null, null);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteContentTooShortError>());
    }

    [Fact]
    public void Create_WithContentAtMinLength_Succeeds()
    {
        var result = TestFixtures.CreateNote("abc");
        Assert.True(result.IsSuccess);
        Assert.Equal("abc", result.Value.Content);
    }

    [Fact]
    public void Create_WithContentAtMaxLength_Succeeds()
    {
        var content = new string('x', 16_384);
        var result = TestFixtures.CreateNote(content);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithContentExceedingMaxLength_Fails()
    {
        var content = new string('x', 16_385);
        var result = TestFixtures.CreateNote(content);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteContentTooLongError>());
    }

    // --- Note.Create: client metadata validation ---

    [Fact]
    public void Create_WithNullClientMetadata_Succeeds()
    {
        var result = TestFixtures.CreateNote(clientMetadata: null);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.ClientMetadata);
    }

    [Fact]
    public void Create_WithClientMetadataAtMaxLength_Succeeds()
    {
        var metadata = new string('m', 1_024);
        var result = TestFixtures.CreateNote(clientMetadata: metadata);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithClientMetadataExceedingMaxLength_Fails()
    {
        var metadata = new string('m', 1_025);
        var result = TestFixtures.CreateNote(clientMetadata: metadata);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NoteClientMetadataTooLongError>());
    }

    // --- Note.Create: field assignment ---

    [Fact]
    public void Create_SetsContentAndDeleteAfter()
    {
        var result = TestFixtures.CreateNote("test content", DeleteAfter.OneWeek);
        Assert.True(result.IsSuccess);
        Assert.Equal("test content", result.Value.Content);
        Assert.Equal(DeleteAfter.OneWeek, result.Value.DeleteAfter);
    }

    [Fact]
    public void Create_WithPassword_SetsPasswordHash()
    {
        var passwordHash = TestFixtures.CreatePasswordHash("secret");
        var result = TestFixtures.CreateNote(passwordHash: passwordHash);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.PasswordHash);
    }

    [Fact]
    public void Create_WithoutPassword_LeavesPasswordHashNull()
    {
        var result = TestFixtures.CreateNote(passwordHash: null);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.PasswordHash);
    }

    [Theory]
    [InlineData(DeleteAfter.Viewing)]
    [InlineData(DeleteAfter.OneHour)]
    [InlineData(DeleteAfter.OneDay)]
    [InlineData(DeleteAfter.OneWeek)]
    public void Create_SetsDeleteAtFromDeleteAfter(DeleteAfter deleteAfter)
    {
        var before = DateTime.UtcNow;
        var result = TestFixtures.CreateNote(deleteAfter: deleteAfter);
        var after = DateTime.UtcNow;

        Assert.True(result.IsSuccess);
        var expectedSpan = deleteAfter.ToTimeSpan();
        Assert.InRange(result.Value.DeleteAt, before.Add(expectedSpan), after.Add(expectedSpan));
    }

    // --- Note.VerifyPassword ---

    [Fact]
    public void VerifyPassword_NoHash_NoPassword_ReturnsOk()
    {
        var note = TestFixtures.CreateNote(passwordHash: null).Value;
        var result = note.VerifyPassword(null);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void VerifyPassword_NoHash_PasswordProvided_ReturnsOk()
    {
        var note = TestFixtures.CreateNote(passwordHash: null).Value;
        var result = note.VerifyPassword("anything");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void VerifyPassword_HasHash_NoPassword_ReturnsPasswordNotProvided()
    {
        var passwordHash = TestFixtures.CreatePasswordHash("secret");
        var note = TestFixtures.CreateNote(passwordHash: passwordHash).Value;

        var result = note.VerifyPassword(null);

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<NotePasswordNotProvidedError>());
    }

    [Fact]
    public void VerifyPassword_HasHash_CorrectPassword_ReturnsOk()
    {
        var passwordHash = TestFixtures.CreatePasswordHash("correct");
        var note = TestFixtures.CreateNote(passwordHash: passwordHash).Value;

        var result = note.VerifyPassword("correct");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void VerifyPassword_HasHash_WrongPassword_ReturnsIncorrectPassword()
    {
        var passwordHash = TestFixtures.CreatePasswordHash("correct");
        var note = TestFixtures.CreateNote(passwordHash: passwordHash).Value;

        var result = note.VerifyPassword("wrong");

        Assert.True(result.IsFailed);
        Assert.Single(result.Errors.OfType<IncorrectNotePasswordError>());
    }

    // --- Note.HasDeleteAfterPassed ---

    [Fact]
    public void HasDeleteAfterPassed_DeleteAtInFuture_ReturnsFalse()
    {
        var note = TestFixtures.CreateNote(deleteAfter: DeleteAfter.OneWeek).Value;
        Assert.False(note.HasDeleteAfterPassed());
    }
}