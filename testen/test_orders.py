import unittest
import requests
import random
from datetime import datetime

class TestOrdersAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/orders'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.new_order = {
            "id": random.randint(1000, 9999),
            "source_id": random.randint(10, 99),
            "order_date": datetime.now().isoformat() + "Z",
            "request_date": (datetime.now().isoformat() + "Z"),
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
            ]
        }

    # Happy path test
    def test_get_orders(self):
        """Test retrieving all orders (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /orders - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_order_by_id(self):
        """Test retrieving an order by ID (happy path)."""
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        order_id = self.new_order["id"]

        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /orders/{order_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_add_order(self):
        """Test adding a new order (happy path)."""
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        order_id = self.new_order["id"]

        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /orders/{order_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_order(self):
        """Test updating an existing order (happy path)."""
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        order_id = self.new_order["id"]

        updated_order = self.new_order.copy()
        updated_order.update({
            "order_status": "Shipped",
            "notes": "Updated test notes.",
            "shipping_notes": "Updated shipping notes.",
            "picking_notes": "Updated picking notes.",
        })
        put_response = requests.put(f"{self.base_url}/{order_id}", json=updated_order, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        order_data = get_response.json()
        self.assertEqual(order_data["order_status"], updated_order["order_status"])

    # Happy path test
    def test_delete_order(self):
        """Test deleting an existing order (happy path)."""
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        order_id = self.new_order["id"]

        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        get_response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_order_with_invalid_api_key(self):
        """Test retrieving orders with invalid API key, should handle properly (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /orders with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    # Unhappy path test
    def test_add_order_missing_fields(self):
        """Test adding order with missing fields, expecting a controlled response (unhappy path)."""
        incomplete_order = {
            "id": random.randint(1000, 9999),
            "reference": f"ORD{random.randint(10000, 99999)}"
            # Missing required fields like order_status, items, etc.
        }
        response = requests.post(self.base_url, json=incomplete_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /orders with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    # Unhappy path test
    def test_update_order_invalid_id(self):
        """Test updating an order with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_order = self.new_order.copy()
        updated_order["order_status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_order, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /orders/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_order_invalid_id(self):
        """Test deleting an order with invalid ID, expecting controlled response (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /orders/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
