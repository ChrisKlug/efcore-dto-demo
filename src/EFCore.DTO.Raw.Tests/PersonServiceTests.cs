using EFCore.DTO.Data.Models;
using EFCore.DTO.Raw.Exceptions;
using EFCore.DTO.Raw.Tests.Extensions;
using EFCore.DTO.Raw.Tests.Infrastructure;

namespace EFCore.DTO.Raw.Tests;

public class PersonServiceTests : TestBase
{
    public static (AddressType Type, string AddressLine1, string AddressLine2, string PostalCode, string City, string Country, bool IsCurrent) GetAddress(
        AddressType Type, string AddressLine1, string AddressLine2, string PostalCode, string City, string Country, bool IsCurrent
    ) => (Type, AddressLine1, AddressLine2, PostalCode, City, Country, IsCurrent);

    public class GetPersonById
    {
        [Fact]
        public Task Returns_PersonDTO_if_ID_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe", new[] {
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 1", "12345", "The City", "Sweden", true)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetPersonById(1);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal(1, result.Result!.Id);
                        Assert.Equal("John", result.Result!.FirstName);
                        Assert.Equal("Doe", result.Result!.LastName);
                        Assert.Null(result.Result!.InvoiceAddress);
                        Assert.NotNull(result.Result!.DeliveryAddress);
                        var address = result.Result!.DeliveryAddress!;
                        Assert.Equal("c/o Jane Doe", address.AddressLine1);
                        Assert.Equal("Street 1", address.AddressLine2);
                        Assert.Equal("12345", address.PostalCode);
                        Assert.Equal("The City", address.City);
                        Assert.Equal("Sweden", address.Country);
                        Assert.True(address.IsCurrent);
                    }
                );

        [Fact]
        public Task Returns_PersonNotFoundException_if_id_does_not_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.GetPersonById(1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<PersonNotFoundException>(result.Error);
                    }
                );

        [Fact]
        public Task Includes_current_addresses_in_response()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe", new[] {
                            GetAddress(AddressType.Delivery, "c/o Jimmy Doe", "Street 2", "98765", "The Other City", "Denmark", false),
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 1", "12345", "The City", "Sweden", true)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetPersonById(1);

                        Assert.NotNull(result.Result!.DeliveryAddress);
                        var address = result.Result!.DeliveryAddress!;
                        Assert.Equal("c/o Jane Doe", address.AddressLine1);
                        Assert.Equal("Street 1", address.AddressLine2);
                        Assert.Equal("12345", address.PostalCode);
                        Assert.Equal("The City", address.City);
                        Assert.Equal("Sweden", address.Country);
                        Assert.True(address.IsCurrent);
                    }
                );
    }

    public class GetPersonsAddresses
    {
        [Fact]
        public Task Returns_empty_AddressDTO_array_if_no_addresses_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetPersonsAddresses(1);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Empty(result.Result!);
                    }
                );

        [Fact]
        public Task Returns_all_addresses_in_AddressDTO_array_if_addresses_exist()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe", new[] {
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 1", "12345", "The City", "Sweden", true),
                            GetAddress(AddressType.Invoice, "c/o Jimmy Doe", "Street 2", "98765", "The Other City", "Denmark", false)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.GetPersonsAddresses(1);

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal(2, result.Result!.Length);
                    }
                );

        [Fact]
        public Task Returns_PersonNotFoundException_if_id_does_not_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.GetPersonsAddresses(1);

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<PersonNotFoundException>(result.Error);
                    }
                );
    }

    public class SetPersonsAddress
    {
        [Fact]
        public Task Adds_new_address_to_database_and_returns_AddressDTO()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Invoice,
                                                                "Line 1",
                                                                "Line 2",
                                                                "12345",
                                                                "City",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.Equal("Invoice", result.Result!.Type);
                        Assert.Equal("Line 1", result.Result!.AddressLine1);
                        Assert.Equal("Line 2", result.Result!.AddressLine2);
                        Assert.Equal("12345", result.Result!.PostalCode);
                        Assert.Equal("City", result.Result!.City);
                        Assert.Equal("Country", result.Result!.Country);
                        Assert.True(result.Result!.IsCurrent);
                    },
                    validate: async cmd =>
                    {
                        cmd.CommandText = $"SELECT * FROM Addresses";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Invoice", rs["Type"]);
                            Assert.Equal("Line 1", rs["AddressLine1"]);
                            Assert.Equal("Line 2", rs["AddressLine2"]);
                            Assert.Equal("12345", rs["PostalCode"]);
                            Assert.Equal("City", rs["City"]);
                            Assert.Equal("Country", rs["Country"]);
                            Assert.True((bool)rs["IsCurrent"]);
                            Assert.False(await rs.ReadAsync());
                        }
                    }
                );

        [Fact]
        public Task Unsets_the_current_address_if_one_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe", new[] {
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 1", "12345", "The City", "Sweden", true),
                            GetAddress(AddressType.Invoice, "c/o Jane Doe", "Street 1", "12345", "The City", "Sweden", true)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Delivery,
                                                                "Line 1",
                                                                "Line 2",
                                                                "12345",
                                                                "City",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.True(result.Result!.IsCurrent);
                    },
                    validate: async cmd =>
                    {
                        cmd.CommandText = $"SELECT * FROM Addresses ORDER BY Id";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            // Old Delivery should not be current
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Delivery", rs["Type"]);
                            Assert.Equal("c/o Jane Doe", rs["AddressLine1"]);
                            Assert.False((bool)rs["IsCurrent"]);

                            // Old Invoice should be current
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Invoice", rs["Type"]);
                            Assert.Equal("c/o Jane Doe", rs["AddressLine1"]);
                            Assert.True((bool)rs["IsCurrent"]);

                            // New Delivery should be current
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Delivery", rs["Type"]);
                            Assert.Equal("Line 1", rs["AddressLine1"]);
                            Assert.True((bool)rs["IsCurrent"]);

                            Assert.False(await rs.ReadAsync());
                        }
                    }
                );

        [Fact]
        public Task Removes_oldest_addresses_if_more_than_3_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe", new[] {
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 1", "12345", "The City", "Sweden", true),
                            GetAddress(AddressType.Invoice, "c/o Jane Doe", "Other Street 1", "12345", "The City", "Sweden", true),
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 2", "12345", "The City", "Sweden", true),
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 3", "12345", "The City", "Sweden", true),
                            GetAddress(AddressType.Delivery, "c/o Jane Doe", "Street 4", "12345", "The City", "Sweden", true)
                        });
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Delivery,
                                                                "Line 1",
                                                                "Line 2",
                                                                "12345",
                                                                "City",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        Assert.True(result.Result!.IsCurrent);
                    },
                    validate: async cmd =>
                    {
                        cmd.CommandText = $"SELECT * FROM Addresses ORDER BY Id";
                        using (var rs = await cmd.ExecuteReaderAsync())
                        {
                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Invoice", rs["Type"]);
                            Assert.Equal("Other Street 1", rs["AddressLine2"]);

                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Delivery", rs["Type"]);
                            Assert.Equal("Street 3", rs["AddressLine2"]);

                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Delivery", rs["Type"]);
                            Assert.Equal("Street 4", rs["AddressLine2"]);

                            Assert.True(await rs.ReadAsync());
                            Assert.Equal(1, (int)rs["PersonId"]);
                            Assert.Equal("Delivery", rs["Type"]);
                            Assert.Equal("Line 2", rs["AddressLine2"]);

                            Assert.False(await rs.ReadAsync());
                        }
                    }
                );

        [Fact]
        public Task Returns_ArgumentNullException_if_addressLine1_is_missing()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Invoice,
                                                                "",
                                                                "Line 2",
                                                                "12345",
                                                                "City",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        var err = Assert.IsAssignableFrom<ArgumentNullException>(result.Error);
                        Assert.Equal("addressLine1", err.ParamName);
                    }
                );

        [Fact]
        public Task Returns_ArgumentNullException_if_postalCode_is_missing()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Invoice,
                                                                "Line 1",
                                                                "Line 2",
                                                                "",
                                                                "City",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        var err = Assert.IsAssignableFrom<ArgumentNullException>(result.Error);
                        Assert.Equal("postalCode", err.ParamName);
                    }
                );

        [Fact]
        public Task Returns_ArgumentNullException_if_city_is_missing()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Invoice,
                                                                "Line 1",
                                                                "Line 2",
                                                                "12345",
                                                                "",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        var err = Assert.IsAssignableFrom<ArgumentNullException>(result.Error);
                        Assert.Equal("city", err.ParamName);
                    }
                );

        [Fact]
        public Task Returns_ArgumentNullException_if_country_is_missing()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: async cmd =>
                    {
                        await cmd.AddPerson(1, "John", "Doe");
                    },
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Invoice,
                                                                "Line 1",
                                                                "Line 2",
                                                                "12345",
                                                                "City",
                                                                "");

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        var err = Assert.IsAssignableFrom<ArgumentNullException>(result.Error);
                        Assert.Equal("country", err.ParamName);
                    }
                );

        [Fact]
        public Task Returns_PersonNotFoundException_if_id_does_not_exists()
            => RunTest(
                    service: x => new PersonService(x),
                    prepare: cmd => Task.CompletedTask,
                    execute: async svc =>
                    {
                        var result = await svc.SetPersonsAddress(1,
                                                                AddressType.Invoice,
                                                                "Line 1",
                                                                "Line 2",
                                                                "12345",
                                                                "City",
                                                                "Country");

                        Assert.NotNull(result);
                        Assert.False(result.IsSuccess);
                        Assert.IsAssignableFrom<PersonNotFoundException>(result.Error);
                    }
                );
    }
}