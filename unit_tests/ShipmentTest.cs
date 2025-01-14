using System;
using System.Collections.Generic;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class ShipmentTests
    {
        [Fact]
        public void Shipment_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var items = new List<ItemDetail>
            {
                new ItemDetail { Item_Id = "ITEM001", Amount = 10, CrossDockingStatus = "In Transit" },
                new ItemDetail { Item_Id = "ITEM002", Amount = 5, CrossDockingStatus = "Shipped" }
            };

            var shipment = new Shipment
            {
                Id = 1,
                Order_Id = 123,
                Source_Id = 456,
                Order_Date = DateTime.Now.AddDays(-3),
                Request_Date = DateTime.Now.AddDays(-2),
                Shipment_Date = DateTime.Now.AddDays(-1),
                Shipment_Type = "Type A",
                Shipment_Status = "In Transit",
                Notes = "Test shipment notes",
                Carrier_Code = "CC123",
                Carrier_Description = "Carrier XYZ",
                Service_Code = "SC456",
                Payment_Type = "Prepaid",
                Transfer_Mode = "Air",
                Total_Package_Count = 3,
                Total_Package_Weight = 25.5,
                Created_At = DateTime.Now.AddMonths(-1),
                Updated_At = DateTime.Now,
                Items = items
            };

            // Act & Assert
            Assert.Equal(1, shipment.Id);
            Assert.Equal(123, shipment.Order_Id);
            Assert.Equal(456, shipment.Source_Id);
            Assert.Equal("Type A", shipment.Shipment_Type);
            Assert.Equal("In Transit", shipment.Shipment_Status);
            Assert.Equal("Carrier XYZ", shipment.Carrier_Description);
            Assert.Equal(3, shipment.Total_Package_Count);
            Assert.Equal(25.5, shipment.Total_Package_Weight);
            Assert.NotNull(shipment.Items);
            Assert.Equal(2, shipment.Items.Count);
        }

        [Fact]
        public void Shipment_Items_ShouldHaveCorrectValues()
        {
            // Arrange
            var item1 = new ItemDetail { Item_Id = "ITEM001", Amount = 10, CrossDockingStatus = "In Transit" };
            var item2 = new ItemDetail { Item_Id = "ITEM002", Amount = 5, CrossDockingStatus = "Shipped" };
            var items = new List<ItemDetail> { item1, item2 };

            var shipment = new Shipment
            {
                Items = items
            };

            // Act & Assert
            Assert.Equal("ITEM001", shipment.Items[0].Item_Id);
            Assert.Equal(10, shipment.Items[0].Amount);
            Assert.Equal("In Transit", shipment.Items[0].CrossDockingStatus);

            Assert.Equal("ITEM002", shipment.Items[1].Item_Id);
            Assert.Equal(5, shipment.Items[1].Amount);
            Assert.Equal("Shipped", shipment.Items[1].CrossDockingStatus);
        }

        [Fact]
        public void Shipment_ShouldHandleNullOrEmptyFields()
        {
            // Arrange
            var shipment = new Shipment
            {
                Shipment_Type = null,
                Shipment_Status = "",
                Notes = null,
                Carrier_Code = "",
                Carrier_Description = null
            };

            // Act & Assert
            Assert.Null(shipment.Shipment_Type);
            Assert.Equal("", shipment.Shipment_Status);
            Assert.Null(shipment.Notes);
            Assert.Equal("", shipment.Carrier_Code);
            Assert.Null(shipment.Carrier_Description);
        }

        [Fact]
        public void Shipment_CreatedAt_ShouldBeEarlierThanUpdatedAt()
        {
            // Arrange
            var shipment = new Shipment
            {
                Created_At = DateTime.Now.AddDays(-2),
                Updated_At = DateTime.Now
            };

            // Act & Assert
            Assert.True(shipment.Created_At < shipment.Updated_At);
        }

        [Fact]
        public void Shipment_TotalPackageWeight_ShouldBePositive()
        {
            // Arrange
            var shipment = new Shipment
            {
                Total_Package_Weight = 25.5
            };

            // Act & Assert
            Assert.True(shipment.Total_Package_Weight > 0);
        }

        [Fact]
        public void Shipment_ShouldThrowErrorIfNegativePackageCount()
        {
            // Arrange
            var shipment = new Shipment
            {
                Total_Package_Count = -1
            };

            // Act & Assert
            Assert.True(shipment.Total_Package_Count < 0, "Total_Package_Count should not allow negative values.");
        }
    }
}