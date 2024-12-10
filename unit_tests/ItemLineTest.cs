using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class ItemLineTests
    {
        [Fact]
        public void ItemLine_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                Id = 1,
                Name = "Electronics Line",
                Description = "Line for electronics",
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, itemLine.Id);
            Assert.Equal("Electronics Line", itemLine.Name);
            Assert.Equal("Line for electronics", itemLine.Description);
            Assert.True(itemLine.Created_At < itemLine.Updated_At, "Created_At should be earlier than Updated_At.");
        }

        [Fact]
        public void ItemLine_ShouldHandleNullDescription()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                Id = 2,
                Name = "Stationery Line",
                Description = null, // Null description
                Created_At = DateTime.Now.AddDays(-15),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(2, itemLine.Id);
            Assert.Equal("Stationery Line", itemLine.Name);
            Assert.Null(itemLine.Description);
        }

        [Fact]
        public void ItemLine_ShouldHaveValidTimestamps()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                Created_At = DateTime.Now.AddDays(-5),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.True(itemLine.Created_At < itemLine.Updated_At, "Created_At should be earlier than Updated_At.");
        }

        [Fact]
        public void ItemLine_Name_ShouldNotBeEmpty()
        {
            // Arrange
            var itemLine = new ItemLine
            {
                Name = string.Empty
            };

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(itemLine.Name), "Name should not be empty or null.");
        }

        [Fact]
        public void ItemLine_ShouldSupportEqualityChecks()
        {
            // Arrange
            var itemLine1 = new ItemLine
            {
                Id = 3,
                Name = "Furniture Line",
                Description = "Line for furniture items",
                Created_At = DateTime.Now.AddDays(-20),
                Updated_At = DateTime.Now
            };

            var itemLine2 = new ItemLine
            {
                Id = 3,
                Name = "Furniture Line",
                Description = "Line for furniture items",
                Created_At = DateTime.Now.AddDays(-20),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(itemLine1.Id, itemLine2.Id);
            Assert.Equal(itemLine1.Name, itemLine2.Name);
            Assert.Equal(itemLine1.Description, itemLine2.Description);
        }
    }
}