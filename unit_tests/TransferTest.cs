using System;
using System.Collections.Generic;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class TransferTests
    {
        [Fact]
        public void Transfer_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var items = new List<ItemDetail>
            {
                new ItemDetail { Item_Id = "ITEM001", Amount = 10, CrossDockingStatus = "In Transit" },
                new ItemDetail { Item_Id = "ITEM002", Amount = 20, CrossDockingStatus = "Matched" }
            };

            var transfer = new Transfer
            {
                Id = 1,
                Reference = "TR12345",
                Transfer_From = 100,
                Transfer_To = 200,
                Transfer_Status = "Pending",
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now,
                Items = items
            };

            // Act & Assert
            Assert.Equal(1, transfer.Id);
            Assert.Equal("TR12345", transfer.Reference);
            Assert.Equal(100, transfer.Transfer_From);
            Assert.Equal(200, transfer.Transfer_To);
            Assert.Equal("Pending", transfer.Transfer_Status);
            Assert.NotNull(transfer.Items);
            Assert.Equal(2, transfer.Items.Count);

            // Check specific items
            Assert.Equal("ITEM001", transfer.Items[0].Item_Id);
            Assert.Equal(10, transfer.Items[0].Amount);
            Assert.Equal("In Transit", transfer.Items[0].CrossDockingStatus);

            Assert.Equal("ITEM002", transfer.Items[1].Item_Id);
            Assert.Equal(20, transfer.Items[1].Amount);
            Assert.Equal("Matched", transfer.Items[1].CrossDockingStatus);

            // Date Assertions
            Assert.NotEqual(DateTime.MinValue, transfer.Created_At);
            Assert.NotEqual(DateTime.MinValue, transfer.Updated_At);
        }

        [Fact]
        public void Transfer_ShouldAllowNullableTransferFrom()
        {
            // Arrange
            var transfer = new Transfer
            {
                Transfer_From = null
            };

            // Act & Assert
            Assert.Null(transfer.Transfer_From);
        }

        [Fact]
        public void Transfer_ShouldInitializeWithEmptyItemsList()
        {
            // Arrange
            var transfer = new Transfer
            {
                Items = new List<ItemDetail>()
            };

            // Act & Assert
            Assert.NotNull(transfer.Items);
            Assert.Empty(transfer.Items);
        }

        [Fact]
        public void Transfer_ItemsList_ShouldContainValidEntries()
        {
            // Arrange
            var items = new List<ItemDetail>
            {
                new ItemDetail { Item_Id = "ITEM001", Amount = 10, CrossDockingStatus = "In Transit" },
                new ItemDetail { Item_Id = "ITEM002", Amount = 20, CrossDockingStatus = "Matched" }
            };

            var transfer = new Transfer
            {
                Items = items
            };

            // Act & Assert
            Assert.Contains(transfer.Items, item => item.Item_Id == "ITEM001" && item.Amount == 10 && item.CrossDockingStatus == "In Transit");
            Assert.Contains(transfer.Items, item => item.Item_Id == "ITEM002" && item.Amount == 20 && item.CrossDockingStatus == "Matched");
        }

        [Fact]
        public void Transfer_ItemDetail_ShouldAllowNullCrossDockingStatus()
        {
            // Arrange
            var itemDetail = new ItemDetail
            {
                Item_Id = "ITEM003",
                Amount = 5,
                CrossDockingStatus = null
            };

            // Act & Assert
            Assert.Null(itemDetail.CrossDockingStatus);
        }
    }
}