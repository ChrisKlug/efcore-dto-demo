using EFCore.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace EFCore.Domain.Tests.Infrastructure
{
    public class TestBase<TSvc>
    {
        protected static async Task RunTest(Func<DemoContext, TSvc> service, Func<DbCommand, Task> prepare, Func<TSvc, Task> execute, Func<DbCommand, Task>? validate = null)
        {
            var context = TestHelper.GetContext() ?? throw new Exception("Invalid context type");

            IDbContextTransaction? transaction = null;
            try
            {
                transaction = context.Database.BeginTransaction();

                var conn = context.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction.GetDbTransaction();

                    await prepare(cmd);

                    await execute(service(context));

                    if (validate != null)
                        await validate(cmd);
                }
            }
            finally
            {
                transaction?.Rollback();
            }
        }

    }
}