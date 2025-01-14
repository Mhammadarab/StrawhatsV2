using System;
using System.Collections.Generic;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class WarehouseTests
    {
        [Fact]
        public void Warehouse_ShouldInitializeWithCorrectValues()
        {
            // Arrange & Act
            var contact = new Contact
            {
                Name = "John Doe",
                Phone = "123-456-7890",
                Email = "johndoe@test.com"
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
                Contact = contact,
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
            Assert.Equal(contact, warehouse.Contact);
            Assert.NotNull(warehouse.Classifications_Id);
            Assert.Equal(3, warehouse.Classifications_Id.Count);

            // Contact Assertions
            Assert.Equal("John Doe", warehouse.Contact.Name);
            Assert.Equal("123-456-7890", warehouse.Contact.Phone);
            Assert.Equal("johndoe@test.com", warehouse.Contact.Email);

            // Date Assertions
            Assert.NotEqual(DateTime.MinValue, warehouse.Created_At);
            Assert.NotEqual(DateTime.MinValue, warehouse.Updated_At);
        }

        [Fact]
        public void Warehouse_ShouldAllowNullableContact()
        {
            // Arrange & Act
            var warehouse = new Warehouse
            {
                Contact = null
            };

            // Assert
            Assert.Null(warehouse.Contact);
        }

        [Fact]
        public void Warehouse_ClassificationsShouldContainValidIds()
        {
            // Arrange
            var warehouse = new Warehouse
            {
                Classifications_Id = new List<int> { 101, 102, 103 }
            };

            // Assert
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