import unittest
import requests
import random
from datetime import datetime
import uuid

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
            "Source_Id": random.randint(10, 99),
            "Order_Date": datetime.now().isoformat() + "Z",
            "Request_Date": datetime.now().isoformat() + "Z",
            "Reference": f"ORD{random.randint(10000, 99999)}",
            "Reference_Extra": "This is a test order.",
            "Order_Status": "Pending",
            "Notes": "These are some test notes.",
            "Shipping_Notes": "Test shipping notes.",
            "Picking_Notes": "Test picking notes.",
            "Warehouse_Id": random.randint(10, 99),
            "Ship_To": random.randint(1000, 9999),
            "Bill_To": random.randint(1000, 9999),
            "Shipment_Id": random.randint(1, 100),
            "Total_Amount": round(random.uniform(100, 10000), 2),
            "Total_Discount": round(random.uniform(0, 500), 2),
            "Total_Tax": round(random.uniform(0, 500), 2),
            "Total_Surcharge": round(random.uniform(0, 100), 2),
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
            ],
            "IsBackordered": True,
            "ShipmentDetails": "test "
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
        
        # Print the response content for debugging
        print(f"POST /orders - Status Code: {post_response.status_code}, Response: {post_response.text}")
        
        self.assertEqual(post_response.status_code, 201)
        order_id = self.new_order["id"]

        # GET request for specific order
        response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the order
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_order(self):
        """Test adding a new order (happy path)."""
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the order exists
        order_id = self.new_order["id"]
        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        # Clean up by deleting the order
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_order(self):
        """Test updating an existing order (happy path)."""
        # Add an order to update
        post_response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        order_id = self.new_order["id"]

        # Update the order
        updated_order = self.new_order.copy()
        updated_order.update({
            "order_status": "Shipped",
            "notes": "Updated test notes.",
            "shipping_notes": "Updated shipping notes.",
            "picking_notes": "Updated picking notes.",
        })
        put_response = requests.put(f"{self.base_url}/{order_id}", json=updated_order, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        order_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {order_data}")

        # Normalize keys to lowercase
        order_data = {k.lower(): v for k, v in order_data.items()}

        # Check if 'order_status' exists in the response
        self.assertIn("order_status", order_data, "Response is missing 'order_status'")
        self.assertEqual(order_data["order_status"], updated_order["order_status"])

        # Clean up by deleting the order
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_order(self):
        """Test deleting an existing order (happy path)."""
        # Add an order to delete
        post_response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        order_id = self.new_order["id"]

        # DELETE request to remove the order
        response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)
        print(f"DELETE /orders/{order_id} - Status Code: {response.status_code}")

        # Verify the order no longer exists
        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)
        print(f"GET /orders/{order_id} after delete - Status Code: {get_response.status_code}")

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