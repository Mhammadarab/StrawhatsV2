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
            "source_id": random.randint(10, 99),
            "order_date": datetime.now().isoformat() + "Z",
            "request_date": datetime.now().isoformat() + "Z",
            "reference": f"ORD{random.randint(10000, 99999)}",
            "reference_extra": "This is a test order.",
            "order_status": "Pending",
            "notes": "These are some test notes.",
            "shipping_notes": "Test shipping notes.",
            "picking_notes": "Test picking notes.",
            "warehouse_id": random.randint(10, 99),
            "ship_to": None,
            "bill_to": None,
            "shipment_id": random.randint(1, 100),
            "total_amount": round(random.uniform(100, 10000), 2),
            "total_discount": round(random.uniform(0, 500), 2),
            "total_tax": round(random.uniform(0, 500), 2),
            "total_surcharge": round(random.uniform(0, 100), 2),
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z",
            "items": [
                {
                    "item_id": f"P{random.randint(1000, 9999)}",
                    "amount": random.randint(1, 50)
                },
                {
                    "item_id": f"P{random.randint(1000, 9999)}",
                    "amount": random.randint(1, 50)
                }
            ],
            "is_backordered": False,
            "shipment_details": "Shipment details here"
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

if __name__ == '__main__':
    unittest.main()