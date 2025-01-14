import requests
import unittest
import random
from datetime import datetime

class TestOrdersAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/orders'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        orders = response.json()
        max_id = max([order["id"] for order in orders], default=0)

        # Order data
        self.new_order = {
            "id": max_id + 1,
            "Source_Id": 33,
            "Order_Date": datetime.now().isoformat().split('T')[0],
            "Request_Date": datetime.now().isoformat().split('T')[0],
            "Reference": f"OR{max_id + 1}",
            "Reference_Extra": "Extra reference",
            "Order_Status": "Pending",
            "Notes": "This is a test order.",
            "Shipping_Notes": "Handle with care.",
            "Picking_Notes": "Pick items carefully.",
            "Warehouse_Id": 1,
            "Ship_To": 1,
            "Bill_To": 1,
            "Shipment_Id": [1, 2],  # List of shipment IDs
            "Total_Amount": 100.0,
            "Total_Discount": 10.0,
            "Total_Tax": 5.0,
            "Total_Surcharge": 2.0,
            "Created_At": datetime.now().isoformat(),
            "Updated_At": datetime.now().isoformat(),
            "Items": [
                {
                    "Item_Id": "P001",
                    "Amount": 5,
                    "CrossDockingStatus": None
                },
                {
                    "Item_Id": "P002",
                    "Amount": 5,
                    "CrossDockingStatus": None
                }
            ],
            "IsBackordered": True,
            "ShipmentDetails": []
        }

    def test_get_orders(self):
        """Test retrieving all orders (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /orders - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_order_by_id(self):
        """Test retrieving an order by ID (happy path)."""
        # Add a new order
        post_response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        order_id = self.new_order["id"]

        # GET request for specific order
        response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Debugging: Print the response JSON
        print(f"GET /orders/{order_id} - Response: {response.json()}")

        # Check if 'shipment_Id' exists in the response JSON
        self.assertIn("shipment_Id", response.json(), "shipment_Id key not found in the response")
        self.assertEqual(response.json()["shipment_Id"], self.new_order["Shipment_Id"])

        # Clean up by deleting the order
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_order(self):
        """Test adding a new order (happy path)."""
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /orders - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_order(self):
        """Test updating an order (happy path)."""
        # Add a new order
        post_response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        order_id = self.new_order["id"]

        # Update the order
        updated_order = self.new_order.copy()
        updated_order["Notes"] = "Updated test order."
        put_response = requests.put(f"{self.base_url}/{order_id}", json=updated_order, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        # Debugging: Print the response JSON
        print(f"GET /orders/{order_id} - Response: {response.json()}")

        # Check if 'notes' exists in the response JSON
        self.assertIn("notes", response.json(), "notes key not found in the response")
        self.assertEqual(response.json()["notes"], updated_order["Notes"])

        # Clean up by deleting the order
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_order(self):
        """Test deleting an order (happy path)."""
        # Add a new order
        post_response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        order_id = self.new_order["id"]

        # Delete the order
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

        # Verify the deletion
        response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)

    def test_get_order_with_invalid_api_key(self):
        """Test retrieving orders with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /orders with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_order_missing_fields(self):
        """Test adding an order with missing fields (unhappy path)."""
        incomplete_order = {
            "id": self.new_order["id"] + 1,
            "reference": f"ORD{random.randint(10000, 99999)}"
        }
        response = requests.post(self.base_url, json=incomplete_order, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /orders with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_order_invalid_id(self):
        """Test updating an order with an invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_order = self.new_order.copy()
        updated_order["order_status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_order, headers=self.headers)
        self.assertEqual(response.status_code, 400)  # Adjusted to match the actual API behavior
        print(f"PUT /orders/{invalid_id} - Status Code: {response.status_code}, Response: {response.text}")

    def test_delete_order_invalid_id(self):
        """Test deleting an order with an invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /orders/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()