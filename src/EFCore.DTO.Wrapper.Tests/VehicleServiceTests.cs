using EFCore.DTO.Wrapper.Exceptions;
using EFCore.DTO.Wrapper.Tests.Extensions;
using EFCore.DTO.Wrapper.Tests.Infrastructure;

namespace EFCore.DTO.Wrapper.Tests;

public class VehicleServiceTests : TestBase
{
    private const string VALID_VIN = "ABC1234567890ADFG";
    private const string INVALID_VIN = "ABC";

    public class AddVehicle
    {
        [Fact]
        public Task Adds_vehicle_to_database_and_returns_VehicleDTO_if_everything_is_ok()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.AddVehicle(VALID_VIN, 1);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal(VALID_VIN, result.Result!.VIN);
                        Assert.NotNull(result.Result!.Owner);

                        var owner = result.Result!.Owner;
                        Assert.Equal(1, owner.Id);
                        Assert.Equal(DateTime.Today, owner.From);
                        Assert.Null(owner.To);
                    },
                    validate: async cmd =>
                    {
                        cmd.CommandText = $"SELECT * FROM Vehicles";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(VALID_VIN, (string)rs["VIN"]);
                            Assert.False(await rs.ReadAsync());
                        }

                        cmd.CommandText = $"SELECT * FROM VehicleOwners";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal(DateTime.Today, (DateTime)rs["From"]);
                            Assert.Equal(DBNull.Value, rs["To"]);
                            Assert.False(await rs.ReadAsync());
                        }
                    }
                );

        [Fact]
        public Task Returns_ArgumentNullException_if_empty_VIN_is_used()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.AddVehicle("", 1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        var err = Assert.IsAssignableFrom<ArgumentNullException>(result.Error);
                        Assert.Equal("vin", err.ParamName);
                    }
                );

        [Fact]
        public Task Returns_InvalidVinException_if_invalid_VIN_is_used()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.AddVehicle(INVALID_VIN, 1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<InvalidVinException>(result.Error);
                    }
                );

        [Fact]
        public Task Returns_DuplicateVinException_if_VIN_is_already_in_use()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN);
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.AddVehicle(VALID_VIN, 1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<DuplicateVinException>(result.Error);
                    }
                );

        [Fact]
        public Task Returns_PersonNotFoundException_personId_does_not_exist()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.AddVehicle(VALID_VIN, 1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<PersonNotFoundException>(result.Error);
                    }
                );
    }

    public class GetVehicleByVin
    {
        [Fact]
        public Task Returns_VehicleDTO_if_VIN_exists()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new [] {
                            (Id: 2, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetVehicleByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal(VALID_VIN, result.Result!.VIN);
                        Assert.NotNull(result.Result!.Owner);

                        var owner = result.Result!.Owner;
                        Assert.Equal(2, owner.Id);
                        Assert.Equal("John", owner.FirstName);
                        Assert.Equal("Doe", owner.LastName);
                        Assert.Equal(DateTime.Today.AddYears(-10), owner.From);
                        Assert.Null(owner.To);

                        Assert.Empty(result.Result!.PreviousOwners);
                    }
                );

        [Fact]
        public Task Returns_VehicleDTO_with_all_previous_owners_if_VIN_exists()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 1, FirstName: "Lisa", LastName: "Doe", From: DateTime.Today.AddYears(-15), To:  DateTime.Today.AddYears(-12)),
                            (Id: 2, FirstName: "Jane", LastName: "Doe", From: DateTime.Today.AddYears(-12), To:  DateTime.Today.AddYears(-10)),
                            (Id: 3, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetVehicleByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);

                        Assert.Equal(2, result.Result!.PreviousOwners.Length);

                        var previousOwners = result.Result!.PreviousOwners;
                        Assert.Equal(1, previousOwners[0].Id);
                        Assert.Equal("Lisa", previousOwners[0].FirstName);
                        Assert.Equal("Doe", previousOwners[0].LastName);
                        Assert.Equal(DateTime.Today.AddYears(-15), previousOwners[0].From);
                        Assert.Equal(DateTime.Today.AddYears(-12), previousOwners[0].To);

                        Assert.Equal(2, previousOwners[1].Id);
                        Assert.Equal("Jane", previousOwners[1].FirstName);
                        Assert.Equal("Doe", previousOwners[1].LastName);
                        Assert.Equal(DateTime.Today.AddYears(-12), previousOwners[1].From);
                        Assert.Equal(DateTime.Today.AddYears(-10), previousOwners[1].To);
                    }
                );

        [Fact]
        public Task Returns_VehicleNotFoundException_if_VIN_does_not_exist()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.GetVehicleByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<VehicleNotFoundException>(result.Error);
                    }
                );
    }

    public class GetVehicleById
    {
        [Fact]
        public Task Returns_VehicleDTO_if_ID_exists()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 2, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetVehicleById(1);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal(VALID_VIN, result.Result!.VIN);
                        Assert.NotNull(result.Result!.Owner);

                        var owner = result.Result!.Owner;
                        Assert.Equal(2, owner.Id);
                        Assert.Equal("John", owner.FirstName);
                        Assert.Equal("Doe", owner.LastName);
                        Assert.Equal(DateTime.Today.AddYears(-10), owner.From);
                        Assert.Null(owner.To);

                        Assert.Empty(result.Result!.PreviousOwners);
                    }
                );

        [Fact]
        public Task Returns_VehicleDTO_with_all_previous_owners_if_ID_exists()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 1, FirstName: "Lisa", LastName: "Doe", From: DateTime.Today.AddYears(-15), To:  DateTime.Today.AddYears(-12)),
                            (Id: 2, FirstName: "Jane", LastName: "Doe", From: DateTime.Today.AddYears(-12), To:  DateTime.Today.AddYears(-10)),
                            (Id: 3, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetVehicleById(1);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);

                        Assert.Equal(2, result.Result!.PreviousOwners.Length);

                        var previousOwners = result.Result!.PreviousOwners;
                        Assert.Equal(1, previousOwners[0].Id);
                        Assert.Equal("Lisa", previousOwners[0].FirstName);
                        Assert.Equal("Doe", previousOwners[0].LastName);
                        Assert.Equal(DateTime.Today.AddYears(-15), previousOwners[0].From);
                        Assert.Equal(DateTime.Today.AddYears(-12), previousOwners[0].To);

                        Assert.Equal(2, previousOwners[1].Id);
                        Assert.Equal("Jane", previousOwners[1].FirstName);
                        Assert.Equal("Doe", previousOwners[1].LastName);
                        Assert.Equal(DateTime.Today.AddYears(-12), previousOwners[1].From);
                        Assert.Equal(DateTime.Today.AddYears(-10), previousOwners[1].To);
                    }
                );

        [Fact]
        public Task Returns_VehicleNotFoundException_if_ID_does_not_exist()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.GetVehicleById(1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<VehicleNotFoundException>(result.Error);
                    }
                );
    }

    public class GetCurrentOwnerByVin
    {
        [Fact]
        public Task Returns_VehicleOwnerDTO_if_VIN_exists()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 1, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-8), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetCurrentOwnerByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal("John", result.Result!.FirstName);
                        Assert.Equal("Doe", result.Result!.LastName);
                        Assert.Equal(DateTime.Today.AddYears(-8), result.Result!.From);
                        Assert.Null(result.Result!.To);
                    }
                );

        [Fact]
        public Task Returns_VehicleNotFoundException_if_VIN_does_not_exist()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.GetCurrentOwnerByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<VehicleNotFoundException>(result.Error);
                    }
                );

        [Fact]
        public Task Returns_OwnerNotFoundException_if_vehicle_has_no_owner()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN);
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetCurrentOwnerByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<OwnerNotFoundException>(result.Error);
                    }
                );

        [Fact]
        public Task Returns_OwnerNotFoundException_if_vehicle_has_no_current_owner()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 1, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)DateTime.Today.AddDays(-2))
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetCurrentOwnerByVin(VALID_VIN);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<OwnerNotFoundException>(result.Error);
                    }
                );
    }

    public class SetCurrentOwner
    {

        [Fact]
        public Task Returns_VehicleNotFoundException_if_VIN_does_not_exist()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.SetCurrentOwner(VALID_VIN, 0);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<VehicleNotFoundException>(result.Error);
                    }
                );

        [Fact]
        public Task Returns_DuplicateOwnerException_if_new_owner_is_same_as_current()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 1, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetCurrentOwner(VALID_VIN, 1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<DuplicateOwnerException>(result.Error);
                    }
                );

        [Fact]
        public Task Returns_PersonNotFoundException_if_new_owner_ID_does_not_exist()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddVehicle(1, VALID_VIN, new[] {
                            (Id: 1, FirstName: "John", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetCurrentOwner(VALID_VIN, 2);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<PersonNotFoundException>(result.Error);
                    }
                );

        [Fact]
        public Task Adds_owner_to_database_and_returns_OwnerDTO()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(7, "John", "Doe");
                        await cmd.AddVehicle(9, VALID_VIN);
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetCurrentOwner(VALID_VIN, 7);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal(7, result.Result!.Id);
                        Assert.Equal("John", result.Result!.FirstName);
                        Assert.Equal("Doe", result.Result!.LastName);
                        Assert.Equal(DateTime.Today, result.Result!.From);
                        Assert.Null(result.Result!.To);
                    },
                    validate: async cmd =>
                    {
                        cmd.CommandText = $"SELECT * FROM VehicleOwners";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(9, (int)rs["VehicleId"]);
                            Assert.Equal(7, (int)rs["PersonId"]);
                            Assert.Equal(DateTime.Today, (DateTime)rs["From"]);
                            Assert.Equal(DBNull.Value, rs["To"]);
                            Assert.False(await rs.ReadAsync());
                        }
                    }
                );

        [Fact]
        public Task Adds_ends_current_ownership()
            => RunTest(
                    service: x => new VehicleService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(5, "John", "Doe");
                        await cmd.AddVehicle(4, VALID_VIN, new[] {
                            (Id: 4, FirstName: "Jane", LastName: "Doe", From: DateTime.Today.AddYears(-10), To: (DateTime?)null)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetCurrentOwner(VALID_VIN, 5);
                    },
                    validate: async cmd =>
                    {
                        cmd.CommandText = $"SELECT * FROM VehicleOwners ORDER BY [From]";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(4, (int)rs["VehicleId"]);
                            Assert.Equal(4, (int)rs["PersonId"]);
                            Assert.Equal(DateTime.Today.AddYears(-10), (DateTime)rs["From"]);
                            Assert.Equal(DateTime.Today, rs["To"]);

                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(4, (int)rs["VehicleId"]);
                            Assert.Equal(5, (int)rs["PersonId"]);
                            Assert.Equal(DateTime.Today, (DateTime)rs["From"]);
                            Assert.Equal(DBNull.Value, rs["To"]);
                            
                            Assert.False(await rs.ReadAsync());
                        }
                    }
                );
    }
}
