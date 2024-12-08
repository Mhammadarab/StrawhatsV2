using System;
using System.Collections.Generic;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class InventoryTests
    {
        [Fact]
        public void Inventory_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var locations = new Dictionary<string, int>
            {
                { "LocationA", 50 },
                { "LocationB", 30 }
            };

            // Act
            var inventory = new Inventory
            {
                Id = 1,
                Item_Id = "ITEM12345",
                Description = "Test Inventory Item",
                Item_Reference = "REF001",
                Locations = locations,
                Total_On_Hand = 100,
                Total_Expected = 20,
                Total_Ordered = 50,
                Total_Allocated = 30,
                Total_Available = 70,
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now
            };

            // Assert Positive Cases
            Assert.Equal(1, inventory.Id);
            Assert.Equal("ITEM12345", inventory.Item_Id);
            Assert.Equal("Test Inventory Item", inventory.Description);
            Assert.Equal("REF001", inventory.Item_Reference);
            Assert.NotNull(inventory.Locations);
            Assert.Equal(50, inventory.Locations["LocationA"]);
            Assert.Equal(30, inventory.Locations["LocationB"]);
            Assert.Equal(100, inventory.Total_On_Hand);
            Assert.Equal(20, inventory.Total_Expected);
            Assert.Equal(50, inventory.Total_Ordered);
            Assert.Equal(30, inventory.Total_Allocated);
            Assert.Equal(70, inventory.Total_Available);
            Assert.NotEqual(DateTime.MinValue, inventory.Created_At);
            Assert.NotEqual(DateTime.MinValue, inventory.Updated_At);

            // Assert Negative Cases
            Assert.NotEqual(0, inventory.Id);
            Assert.NotEqual("WRONG_ITEM", inventory.Item_Id);
            Assert.NotEqual("Incorrect Description", inventory.Description);
            Assert.NotEqual(999, inventory.Total_On_Hand);
            Assert.DoesNotContain("LocationC", inventory.Locations.Keys);
        }

        [Fact]
        public void Inventory_Locations_ShouldHandleEmptyOrNullDictionary()
        {
            // Arrange & Act
            var inventory = new Inventory
            {
                Locations = null
            };

            // Assert
            Assert.Null(inventory.Locations);

            // Arrange & Act
            inventory.Locations = new Dictionary<string, int>();

            // Assert
            Assert.NotNull(inventory.Locations);
            Assert.Empty(inventory.Locations);
        }
    }
}