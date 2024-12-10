using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class LocationTests
    {
        [Fact]
        public void Location_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var location = new Location
            {
                Id = 1,
                Warehouse_Id = 101,
                Code = "A.1.1",
                Name = "Row A, Rack 1, Shelf 1",
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, location.Id);
            Assert.Equal(101, location.Warehouse_Id);
            Assert.Equal("A.1.1", location.Code);
            Assert.Equal("Row A, Rack 1, Shelf 1", location.Name);
            Assert.True(location.Created_At < location.Updated_At, "Created_At should be earlier than Updated_At.");
        }

        [Fact]
        public void Location_ShouldHandleEmptyOrNullName()
        {
            // Arrange
            var location = new Location
            {
                Id = 2,
                Warehouse_Id = 102,
                Code = "B.2.3",
                Name = null, // Null name
                Created_At = DateTime.Now.AddDays(-5),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(2, location.Id);
            Assert.Equal(102, location.Warehouse_Id);
            Assert.Equal("B.2.3", location.Code);
            Assert.Null(location.Name);
        }

        [Fact]
        public void Location_ShouldHaveValidTimestamps()
        {
            // Arrange
            var location = new Location
            {
                Created_At = DateTime.Now.AddDays(-20),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.True(location.Created_At < location.Updated_At, "Created_At should be earlier than Updated_At.");
        }

        [Fact]
        public void Location_ShouldNotAllowEmptyCode()
        {
            // Arrange
            var location = new Location
            {
                Code = string.Empty
            };

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(location.Code), "Code should not be empty or null.");
        }

        [Fact]
        public void Location_ShouldSupportEqualityChecks()
        {
            // Arrange
            var location1 = new Location
            {
                Id = 3,
                Warehouse_Id = 103,
                Code = "C.3.2",
                Name = "Row C, Rack 3, Shelf 2",
                Created_At = DateTime.Now.AddDays(-15),
                Updated_At = DateTime.Now
            };

            var location2 = new Location
            {
                Id = 3,
                Warehouse_Id = 103,
                Code = "C.3.2",
                Name = "Row C, Rack 3, Shelf 2",
                Created_At = DateTime.Now.AddDays(-15),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(location1.Id, location2.Id);
            Assert.Equal(location1.Warehouse_Id, location2.Warehouse_Id);
            Assert.Equal(location1.Code, location2.Code);
            Assert.Equal(location1.Name, location2.Name);
        }
    }
}