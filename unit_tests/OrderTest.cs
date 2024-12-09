using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class OrderTests
    {
        [Fact]
        public void Order_ShouldInitializeWithCorrectValues()
        {
            // Arrange
            var orderDate = DateTime.Now;
            var requestDate = DateTime.Now.AddDays(1);

            // Act
            var order = new Order
            {
                Id = 1,
                Source_Id = 10,
                Order_Date = orderDate,
                Request_Date = requestDate,
                Reference = "ORD12345",
                Reference_Extra = "Extra Ref",
                Order_Status = "Pending",
                Notes = "Some notes",
                Total_Amount = 100.50m,
                Total_Discount = 10.00m,
                Total_Tax = 5.00m,
                Total_Surcharge = 2.50m,
                Warehouse_Id = 20
            };

            // Assert Positive Cases
            Assert.Equal(1, order.Id);
            Assert.Equal(10, order.Source_Id);
            Assert.Equal(orderDate, order.Order_Date);
            Assert.Equal(requestDate, order.Request_Date);
            Assert.Equal("ORD12345", order.Reference);
            Assert.Equal("Extra Ref", order.Reference_Extra);
            Assert.Equal("Pending", order.Order_Status);
            Assert.Equal("Some notes", order.Notes);
            Assert.Equal(100.50m, order.Total_Amount);
            Assert.Equal(10.00m, order.Total_Discount);
            Assert.Equal(5.00m, order.Total_Tax);
            Assert.Equal(2.50m, order.Total_Surcharge);
            Assert.Equal(20, order.Warehouse_Id);

            // Assert Negative Cases (NotEqual)
            Assert.NotEqual(2, order.Id);
            Assert.NotEqual(5, order.Source_Id);
            Assert.NotEqual(DateTime.MinValue, order.Order_Date); // Ensure a valid date is set
            Assert.NotEqual(DateTime.MinValue, order.Request_Date);
            Assert.NotEqual("INVALID_REF", order.Reference);
            Assert.NotEqual("Invalid Status", order.Order_Status);
            Assert.NotEqual(0.0m, order.Total_Amount); // Total amount should not be zero
            Assert.NotEqual(-1, order.Warehouse_Id);   // Warehouse ID should not be invalid
        }
    }
}