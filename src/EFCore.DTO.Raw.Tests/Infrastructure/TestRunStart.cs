using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("EFCore.DTO.Raw.Tests.Infrastructure.TestRunStart", "EFCore.DTO.Raw.Tests")]

namespace EFCore.DTO.Raw.Tests.Infrastructure;

public class TestRunStart : XunitTestFramework
{
    public TestRunStart(IMessageSink messageSink) : base(messageSink)
    {
        TestHelper.GetContext().Database.Migrate();
    }
}
