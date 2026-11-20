
namespace Ideon.API.Exceptions;
using Xunit;

public class IdeaNotFoundExceptionTests
{
    [Fact]
    public void IdeaNotFoundException_ContainsIdeaId()
    {
        var exception = new IdeaNotFoundException(42);

        Assert.Equal(42, exception.IdeaId);
        Assert.Contains("42", exception.Message);
    }
}