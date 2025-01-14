using Cargohub.models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Cargohub.UnitTests
{
    public class WarehouseTests
    {
        [Fact]
        public void Warehouse_ShouldInitializeWithCorrectValues()
        {
            // Arrange & Act
            var contacts = new List<Contact>
            {
                new Contact
                {
                    Name = "John Doe",
                    Phone = "123-456-7890",
                    Email = "johndoe@test.com"
                },
                new Contact
                {
                    Name = "Jane Smith",
                    Phone = "987-654-3210",
                    Email = "janesmith@test.com"
                }
            };

            var warehouse = new Warehouse
            {
                Id = 1,
                Code = "WH001",
                Name = "Central Warehouse",
                Address = "456 Warehouse Lane",
                Zip = "54321",
                City = "Storageville",
                Province = "Storage Province",
                Country = "Warehouseland",
                Contact = contacts, // Updated to use a list of contacts
                Created_At = DateTime.Now.AddDays(-30),
                Updated_At = DateTime.Now,
                Classifications_Id = new List<int> { 101, 102, 103 }
            };

            // Assert Positive Cases
            Assert.Equal(1, warehouse.Id);
            Assert.Equal("WH001", warehouse.Code);
            Assert.Equal("Central Warehouse", warehouse.Name);
            Assert.Equal("456 Warehouse Lane", warehouse.Address);
            Assert.Equal("54321", warehouse.Zip);
            Assert.Equal("Storageville", warehouse.City);
            Assert.Equal("Storage Province", warehouse.Province);
            Assert.Equal("Warehouseland", warehouse.Country);
            Assert.NotNull(warehouse.Contact);
            Assert.Equal(2, warehouse.Contact.Count);
            Assert.Equal("John Doe", warehouse.Contact[0].Name);
            Assert.Equal("123-456-7890", warehouse.Contact[0].Phone);
            Assert.Equal("johndoe@test.com", warehouse.Contact[0].Email);
            Assert.Equal("Jane Smith", warehouse.Contact[1].Name);
            Assert.Equal("987-654-3210", warehouse.Contact[1].Phone);
            Assert.Equal("janesmith@test.com", warehouse.Contact[1].Email);
            Assert.NotNull(warehouse.Classifications_Id);
            Assert.Equal(3, warehouse.Classifications_Id.Count);
            Assert.Contains(101, warehouse.Classifications_Id);
            Assert.Contains(102, warehouse.Classifications_Id);
            Assert.Contains(103, warehouse.Classifications_Id);
        }

        [Fact]
        public void Warehouse_ClassificationsShouldHandleEmptyList()
        {
            // Arrange & Act
            var warehouse = new Warehouse
            {
                Classifications_Id = new List<int>()
            };

            // Assert
            Assert.NotNull(warehouse.Classifications_Id);
            Assert.Empty(warehouse.Classifications_Id);
        }
    }
}