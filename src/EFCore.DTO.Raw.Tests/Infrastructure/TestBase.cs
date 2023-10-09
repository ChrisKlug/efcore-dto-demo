using EFCore.DTO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace EFCore.DTO.Raw.Tests.Infrastructure
{
    public class TestBase
    {
        protected static async Task RunTest<T>(Func<VehicleRegistryContext, T> service, Func<DbCommand, Task> prepare, Func<T, Task> execute, Func<DbCommand, Task>? validate = null)
        {
            var context = TestHelper.GetContext();

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