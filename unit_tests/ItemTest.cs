using System;
using System.Collections.Generic;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class ItemTests
    {
        [Fact]
        public void Item_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var inventoryTotals = new InventoryTotals
            {
                TotalOnHand = 100,
                TotalExpected = 50,
                TotalOrdered = 70,
                TotalAllocated = 30,
                TotalAvailable = 40
            };

            var classifications = new List<int> { 1, 2, 3 };

            // Act
            var item = new Item
            {
                Uid = "UID12345",
                Code = "CODE6789",
                Description = "A test item",
                ShortDescription = "Test item",
                UpcCode = "123456789012",
                ModelNumber = "MODEL001",
                CommodityCode = "COM123",
                ItemLine = 10,
                ItemGroup = 20,
                ItemType = 30,
                UnitPurchaseQuantity = 5,
                UnitOrderQuantity = 10,
                PackOrderQuantity = 15,
                SupplierId = 1001,
                SupplierCode = "SUPP001",
                SupplierPartNumber = "SPN12345",
                Created_At = DateTime.Now.AddDays(-1),
                Updated_At = DateTime.Now,
                InventoryTotals = inventoryTotals,
                Classifications_Id = classifications
            };

            // Assert Positive Cases
            Assert.Equal("UID12345", item.Uid);
            Assert.Equal("CODE6789", item.Code);
            Assert.Equal("A test item", item.Description);
            Assert.Equal("Test item", item.ShortDescription);
            Assert.Equal("123456789012", item.UpcCode);
            Assert.Equal("MODEL001", item.ModelNumber);
            Assert.Equal("COM123", item.CommodityCode);
            Assert.Equal(10, item.ItemLine);
            Assert.Equal(20, item.ItemGroup);
            Assert.Equal(30, item.ItemType);
            Assert.Equal(5, item.UnitPurchaseQuantity);
            Assert.Equal(10, item.UnitOrderQuantity);
            Assert.Equal(15, item.PackOrderQuantity);
            Assert.Equal(1001, item.SupplierId);
            Assert.Equal("SUPP001", item.SupplierCode);
            Assert.Equal("SPN12345", item.SupplierPartNumber);
            Assert.NotEqual(DateTime.MinValue, item.Created_At);
            Assert.NotEqual(DateTime.MinValue, item.Updated_At);
            Assert.NotNull(item.InventoryTotals);
            Assert.Equal(inventoryTotals.TotalOnHand, item.InventoryTotals.TotalOnHand);
            Assert.Equal(inventoryTotals.TotalExpected, item.InventoryTotals.TotalExpected);
            Assert.Equal(inventoryTotals.TotalOrdered, item.InventoryTotals.TotalOrdered);
            Assert.Equal(inventoryTotals.TotalAllocated, item.InventoryTotals.TotalAllocated);
            Assert.Equal(inventoryTotals.TotalAvailable, item.InventoryTotals.TotalAvailable);
            Assert.NotNull(item.Classifications_Id);
            Assert.Equal(classifications.Count, item.Classifications_Id.Count);

            // Assert Negative Cases
            Assert.NotEqual("WRONG_UID", item.Uid);
            Assert.NotEqual(0, item.ItemLine);
            Assert.NotEqual(999, item.ItemGroup);
            Assert.NotEqual("Invalid Description", item.Description);
            Assert.NotEqual(new List<int> { 9, 8, 7 }, item.Classifications_Id);
        }
    }
}