namespace Cryptic.Web.Server.Tests.Common;

public class HttpResponseBodyTests
{
    [Fact]
    public void Ok_WithData_CreatesOkResponseWithStatus()
    {
        object data = "test data";
        var body = HttpResponseBody.Ok(data);

        var okBody = Assert.IsType<OkHttpResponseBody>(body);
        Assert.Equal("ok", okBody.Status);
        Assert.Equal(data, okBody.Data);
    }

    [Fact]
    public void Ok_WithNull_CreatesOkResponseWithNullData()
    {
        var body = HttpResponseBody.Ok();

        var okBody = Assert.IsType<OkHttpResponseBody>(body);
        Assert.Equal("ok", okBody.Status);
        Assert.Null(okBody.Data);
    }

    [Fact]
    public void Ok_Generic_CreatesTypedOkResponse()
    {
        var body = HttpResponseBody.Ok("typed data");

        var okBody = Assert.IsType<OkHttpResponseBody<string>>(body);
        Assert.Equal("ok", okBody.Status);
        Assert.Equal("typed data", okBody.Data);
    }

    [Fact]
    public void Fail_WithSingleError_CreatesFailedResponse()
    {
        var error = new InternalError();
        var body = HttpResponseBody.Fail(error);

        var failedBody = Assert.IsType<FailedHttpResponseBody>(body);
        Assert.Equal("failed", failedBody.Status);
        Assert.Single(failedBody.Errors);
        Assert.Same(error, failedBody.Errors.First());
    }

    [Fact]
    public void Fail_WithMultipleErrors_CreatesFailedResponse()
    {
        var errors = new List<CodedError>
        {
            new InternalError(),
            new BadRequestError("bad input"),
        };

        var body = HttpResponseBody.Fail(errors);

        var failedBody = Assert.IsType<FailedHttpResponseBody>(body);
        Assert.Equal("failed", failedBody.Status);
        Assert.Equal(2, failedBody.Errors.Count);
    }
}
