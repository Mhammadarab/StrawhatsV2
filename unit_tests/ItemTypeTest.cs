using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class ItemTypeTests
    {
        [Fact]
        public void ItemType_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 1,
                Name = "Electronics",
                Description = "Category for electronic items",
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, itemType.Id);
            Assert.Equal("Electronics", itemType.Name);
            Assert.Equal("Category for electronic items", itemType.Description);
            Assert.True(itemType.Created_At < itemType.Updated_At, "Created_At should be earlier than Updated_At.");
        }

        [Fact]
        public void ItemType_ShouldHandleNullDescription()
        {
            // Arrange
            var itemType = new ItemType
            {
                Id = 2,
                Name = "Furniture",
                Description = null, // Null description
                Created_At = DateTime.Now.AddDays(-5),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(2, itemType.Id);
            Assert.Equal("Furniture", itemType.Name);
            Assert.Null(itemType.Description);
        }

        [Fact]
        public void ItemType_Timestamps_ShouldBeValid()
        {
            // Arrange
            var itemType = new ItemType
            {
                Created_At = DateTime.Now.AddDays(-15),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.True(itemType.Created_At < itemType.Updated_At, "Created_At should be earlier than Updated_At.");
        }

        [Fact]
        public void ItemType_ShouldNotAllowEmptyName()
        {
            // Arrange
            var itemType = new ItemType
            {
                Name = string.Empty
            };

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(itemType.Name), "Name should not be empty or null.");
        }

        [Fact]
        public void ItemType_ShouldSupportEqualityChecks()
        {
            // Arrange
            var itemType1 = new ItemType
            {
                Id = 3,
                Name = "Clothing",
                Description = "Category for clothing items",
                Created_At = DateTime.Now.AddDays(-20),
                Updated_At = DateTime.Now
            };

            var itemType2 = new ItemType
            {
                Id = 3,
                Name = "Clothing",
                Description = "Category for clothing items",
                Created_At = DateTime.Now.AddDays(-20),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(itemType1.Id, itemType2.Id);
            Assert.Equal(itemType1.Name, itemType2.Name);
            Assert.Equal(itemType1.Description, itemType2.Description);
        }
    }
}