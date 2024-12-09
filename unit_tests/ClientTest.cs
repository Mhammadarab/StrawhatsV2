using System;
using Xunit;
using Cargohub.models;

namespace Cargohub.UnitTests
{
    public class ClientTests
    {
        [Fact]
        public void Client_ShouldInitializeWithCorrectValues()
        {
            // Arrange & Act
            var client = new Client
            {
                Id = 1,
                Name = "Test Client",
                Address = "123 Test Street",
                City = "Testville",
                Zip_Code = "12345",
                Province = "Test Province",
                Country = "Testland",
                Contact_Name = "John Doe",
                Contact_Phone = "123-456-7890",
                Contact_Email = "johndoe@test.com",
                Created_At = DateTime.Now.AddDays(-10),
                Updated_At = DateTime.Now
            };

            // Assert Positive Cases
            Assert.Equal(1, client.Id);
            Assert.Equal("Test Client", client.Name);
            Assert.Equal("123 Test Street", client.Address);
            Assert.Equal("Testville", client.City);
            Assert.Equal("12345", client.Zip_Code);
            Assert.Equal("Test Province", client.Province);
            Assert.Equal("Testland", client.Country);
            Assert.Equal("John Doe", client.Contact_Name);
            Assert.Equal("123-456-7890", client.Contact_Phone);
            Assert.Equal("johndoe@test.com", client.Contact_Email);
            Assert.NotEqual(DateTime.MinValue, client.Created_At);
            Assert.NotEqual(DateTime.MinValue, client.Updated_At);

            // Assert Negative Cases
            Assert.NotEqual(0, client.Id);
            Assert.NotEqual("Wrong Name", client.Name);
            Assert.NotEqual("Wrong Address", client.Address);
            Assert.NotEqual("Wrong City", client.City);
            Assert.NotEqual("00000", client.Zip_Code);
            Assert.NotEqual("Wrong Province", client.Province);
            Assert.NotEqual("Wrongland", client.Country);
            Assert.NotEqual("Jane Doe", client.Contact_Name);
            Assert.NotEqual("000-000-0000", client.Contact_Phone);
            Assert.NotEqual("janedoe@test.com", client.Contact_Email);
        }

        [Fact]
        public void Client_ShouldAllowNullableName()
        {
            // Arrange & Act
            var client = new Client
            {
                Name = null
            };

            // Assert
            Assert.Null(client.Name);
        }
    }
}