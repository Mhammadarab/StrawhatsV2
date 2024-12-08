using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class SupplierTests
    {
        [Fact]
        public void Supplier_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 1,
                Code = "SUP123",
                Name = "Supplier Name",
                Address = "123 Main Street",
                Address_Extra = "Suite 100",
                City = "Test City",
                Zip_Code = "12345",
                Province = "Test Province",
                Country = "Test Country",
                Contact_Name = "John Doe",
                PhoneNumber = "123-456-7890",
                Reference = "REF123",
                Created_At = DateTime.Now.AddMonths(-1),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, supplier.Id);
            Assert.Equal("SUP123", supplier.Code);
            Assert.Equal("Supplier Name", supplier.Name);
            Assert.Equal("123 Main Street", supplier.Address);
            Assert.Equal("Suite 100", supplier.Address_Extra);
            Assert.Equal("Test City", supplier.City);
            Assert.Equal("12345", supplier.Zip_Code);
            Assert.Equal("Test Province", supplier.Province);
            Assert.Equal("Test Country", supplier.Country);
            Assert.Equal("John Doe", supplier.Contact_Name);
            Assert.Equal("123-456-7890", supplier.PhoneNumber);
            Assert.Equal("REF123", supplier.Reference);
            Assert.NotEqual(DateTime.MinValue, supplier.Created_At);
            Assert.NotEqual(DateTime.MinValue, supplier.Updated_At);
        }

        [Fact]
        public void Supplier_Fields_ShouldBeNotEqualToOtherValues()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 1,
                Code = "SUP123",
                Name = "Supplier Name",
                Address = "123 Main Street",
                Address_Extra = "Suite 100",
                City = "Test City",
                Zip_Code = "12345",
                Province = "Test Province",
                Country = "Test Country",
                Contact_Name = "John Doe",
                PhoneNumber = "123-456-7890",
                Reference = "REF123",
                Created_At = DateTime.Now.AddMonths(-1),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.NotEqual(2, supplier.Id);
            Assert.NotEqual("SUP456", supplier.Code);
            Assert.NotEqual("Another Supplier", supplier.Name);
            Assert.NotEqual("456 Another Street", supplier.Address);
            Assert.NotEqual("Suite 200", supplier.Address_Extra);
            Assert.NotEqual("Another City", supplier.City);
            Assert.NotEqual("67890", supplier.Zip_Code);
            Assert.NotEqual("Another Province", supplier.Province);
            Assert.NotEqual("Another Country", supplier.Country);
            Assert.NotEqual("Jane Smith", supplier.Contact_Name);
            Assert.NotEqual("987-654-3210", supplier.PhoneNumber);
            Assert.NotEqual("REF456", supplier.Reference);
        }

        [Fact]
        public void Supplier_CreatedAt_ShouldBeEarlierThanUpdatedAt()
        {
            // Arrange
            var supplier = new Supplier
            {
                Created_At = DateTime.Now.AddDays(-1),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.True(supplier.Created_At < supplier.Updated_At);
        }

        [Fact]
        public void Supplier_AllFields_ShouldAllowEmptyStrings()
        {
            // Arrange
            var supplier = new Supplier
            {
                Code = "",
                Name = "",
                Address = "",
                Address_Extra = "",
                City = "",
                Zip_Code = "",
                Province = "",
                Country = "",
                Contact_Name = "",
                PhoneNumber = "",
                Reference = ""
            };

            // Act & Assert
            Assert.Equal("", supplier.Code);
            Assert.Equal("", supplier.Name);
            Assert.Equal("", supplier.Address);
            Assert.Equal("", supplier.Address_Extra);
            Assert.Equal("", supplier.City);
            Assert.Equal("", supplier.Zip_Code);
            Assert.Equal("", supplier.Province);
            Assert.Equal("", supplier.Country);
            Assert.Equal("", supplier.Contact_Name);
            Assert.Equal("", supplier.PhoneNumber);
            Assert.Equal("", supplier.Reference);
        }

        [Fact]
        public void Supplier_AllFields_ShouldHandleNullValues()
        {
            // Arrange
            var supplier = new Supplier
            {
                Code = null,
                Name = null,
                Address = null,
                Address_Extra = null,
                City = null,
                Zip_Code = null,
                Province = null,
                Country = null,
                Contact_Name = null,
                PhoneNumber = null,
                Reference = null
            };

            // Act & Assert
            Assert.Null(supplier.Code);
            Assert.Null(supplier.Name);
            Assert.Null(supplier.Address);
            Assert.Null(supplier.Address_Extra);
            Assert.Null(supplier.City);
            Assert.Null(supplier.Zip_Code);
            Assert.Null(supplier.Province);
            Assert.Null(supplier.Country);
            Assert.Null(supplier.Contact_Name);
            Assert.Null(supplier.PhoneNumber);
            Assert.Null(supplier.Reference);
        }
    }
}