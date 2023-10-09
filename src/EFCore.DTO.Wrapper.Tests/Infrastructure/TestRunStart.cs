using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("EFCore.DTO.Wrapper.Tests.Infrastructure.TestRunStart", "EFCore.DTO.Wrapper.Tests")]

namespace EFCore.DTO.Wrapper.Tests.Infrastructure;

public class TestRunStart : XunitTestFramework
{
    public TestRunStart(IMessageSink messageSink) : base(messageSink)
    {
        TestHelper.GetContext().Database.Migrate();
    }
}
