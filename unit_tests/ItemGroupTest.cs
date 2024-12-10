using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class ItemGroupTests
    {
        [Fact]
        public void ItemGroup_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 1,
                Name = "Electronics",
                Description = "Group for electronic items",
                Created_At = DateTime.Now.AddDays(-7),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, itemGroup.Id);
            Assert.Equal("Electronics", itemGroup.Name);
            Assert.Equal("Group for electronic items", itemGroup.Description);
            Assert.True(itemGroup.Created_At < itemGroup.Updated_At);
        }

        [Fact]
        public void ItemGroup_ShouldHandleNullDescription()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Id = 2,
                Name = "Stationery",
                Description = null, // Null description
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(2, itemGroup.Id);
            Assert.Equal("Stationery", itemGroup.Name);
            Assert.Null(itemGroup.Description);
        }

        [Fact]
        public void ItemGroup_ShouldHaveValidTimestamps()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Created_At = DateTime.Now.AddDays(-5),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.True(itemGroup.Created_At < itemGroup.Updated_At, "Created_At should be earlier than Updated_At");
        }

        [Fact]
        public void ItemGroup_Name_ShouldNotBeEmpty()
        {
            // Arrange
            var itemGroup = new ItemGroup
            {
                Name = string.Empty
            };

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(itemGroup.Name), "Name should not be empty or null.");
        }

        [Fact]
        public void ItemGroup_ShouldSupportEqualityChecks()
        {
            // Arrange
            var itemGroup1 = new ItemGroup
            {
                Id = 3,
                Name = "Furniture",
                Description = "Group for furniture items",
                Created_At = DateTime.Now.AddDays(-15),
                Updated_At = DateTime.Now
            };

            var itemGroup2 = new ItemGroup
            {
                Id = 3,
                Name = "Furniture",
                Description = "Group for furniture items",
                Created_At = DateTime.Now.AddDays(-15),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(itemGroup1.Id, itemGroup2.Id);
            Assert.Equal(itemGroup1.Name, itemGroup2.Name);
            Assert.Equal(itemGroup1.Description, itemGroup2.Description);
        }
    }
}