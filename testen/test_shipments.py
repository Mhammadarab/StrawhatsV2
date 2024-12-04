import unittest
import random
from datetime import datetime
import requests


class TestShipmentsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/shipments'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Shipment data
        self.new_shipment = {
            "Id": random.randint(1000, 9999),
            "Order_Id": random.randint(1, 100),
            "Source_Id": random.randint(10, 99),
            "Order_Date": datetime.now().isoformat().split('T')[0],
            "Request_Date": datetime.now().isoformat().split('T')[0],
            "Shipment_Date": datetime.now().isoformat().split('T')[0],
            "Shipment_Type": "I",
            "Shipment_Status": "Pending",
            "Notes": "This is a test shipment.",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Express",
            "Payment_Type": "Manual",
            "Transfer_Mode": "Air",
            "Total_Package_Count": random.randint(1, 50),
            "Total_Package_Weight": round(random.uniform(100, 1000), 2),
            "Created_At": datetime.now().isoformat() + "Z",
            "Updated_At": datetime.now().isoformat() + "Z",
            "Items": [
                {
                    "Item_Id": f"P{random.randint(1000, 9999)}",
                    "Amount": random.randint(1, 50),
                    "CrossDockingStatus": None
                },
                {
                    "Item_Id": f"P{random.randint(1000, 9999)}",
                    "Amount": random.randint(1, 50),
                    "CrossDockingStatus": None
                }
            ]
        }

    def test_get_shipments(self):
        """Test retrieving all shipments (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /shipments - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_shipment_by_id(self):
        """Test retrieving a shipment by ID (happy path)."""
        # Add a new shipment
        post_response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.new_shipment["Id"]

        # GET request for specific shipment
        response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_add_shipment(self):
        """Test adding a new shipment (happy path)."""
        response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the shipment exists
        shipment_id = self.new_shipment["Id"]
        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

    def test_update_shipment(self):
        """Test updating an existing shipment (happy path)."""
        # Add a shipment to update
        post_response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.new_shipment["Id"]

        # Update the shipment
        updated_shipment = self.new_shipment.copy()
        updated_shipment.update({
            "Shipment_Status": "Shipped",
            "Notes": "Updated shipment notes.",
            "Carrier_Code": "FedEx",
            "Carrier_Description": "Federal Express"
        })
        response = requests.put(f"{self.base_url}/{shipment_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        shipment_data = get_response.json()
        print(f"GET Response Data: {shipment_data}")  # Debugging step to print the response data
        self.assertEqual(shipment_data["shipment_Status"], updated_shipment["Shipment_Status"])

    def test_delete_shipment(self):
        """Test deleting an existing shipment (happy path)."""
        # Add a shipment to delete
        post_response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.new_shipment["Id"]

        # DELETE request to remove the shipment
        response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the shipment no longer exists
        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_update_shipment_invalid_id(self):
        """Test updating a shipment with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_shipment = self.new_shipment.copy()
        updated_shipment["Shipment_Status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 400)

    def test_delete_shipment_invalid_id(self):
        """Test deleting a shipment with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)

    def test_get_shipment_with_invalid_api_key(self):
        """Test retrieving shipments with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /shipments with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_shipment_missing_fields(self):
        """Test adding shipment with missing fields (unhappy path)."""
        incomplete_shipment = {
            "Id": random.randint(1000, 9999),
            "Order_Id": random.randint(1, 100)
        }
        response = requests.post(self.base_url, json=incomplete_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 400)


if __name__ == '__main__':
    unittest.main()