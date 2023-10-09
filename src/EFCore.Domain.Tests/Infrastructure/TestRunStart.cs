using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("EFCore.Domain.Tests.Infrastructure.TestRunStart", "EFCore.Domain.Tests")]

namespace EFCore.Domain.Tests.Infrastructure;

public class TestRunStart : XunitTestFramework
{
    public TestRunStart(IMessageSink messageSink) : base(messageSink)
    {
        TestHelper.GetContext().Database.Migrate();
    }
}
