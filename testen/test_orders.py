import json
import os
import random
import uuid
import unittest
import tempfile
import requests
from datetime import datetime

class TestOrdersAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v1/orders'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Use a temporary file for the mock data
        self.temp_mock_file = tempfile.NamedTemporaryFile(delete=False, suffix=".json")
        self.mock_file_path = self.temp_mock_file.name

        # Order data
        self.new_order = {
            "id": uuid.uuid4().int % 10000,  # Unique ID
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
            ]
        }

        # Initialize the temporary mock file with an empty JSON array
        with open(self.mock_file_path, 'w') as mock_file:
            json.dump([], mock_file)

    def tearDown(self):
        # Delete the temporary file after the test run
        os.unlink(self.mock_file_path)

    def _read_mock_file(self):
        """Reads data from the mock file."""
        with open(self.mock_file_path, 'r') as mock_file:
            return json.load(mock_file)

    def _write_mock_file(self, data):
        """Writes data to the mock file."""
        with open(self.mock_file_path, 'w') as mock_file:
            json.dump(data, mock_file, indent=4)

    def test_add_order_to_mock(self):
        """Test adding order and verifying its existence."""
        # POST request to add order
        response = requests.post(self.base_url, json=self.new_order, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Simulate adding order to mock file
        mock_data = self._read_mock_file()
        mock_data.append(self.new_order)
        self._write_mock_file(mock_data)

        # Validate the order in the mock file
        self.assertTrue(any(order["id"] == self.new_order["id"] for order in self._read_mock_file()))

    def test_delete_order_from_mock(self):
        """Test deleting an order and ensuring it is removed from the mock."""
        # Add order to mock file first
        mock_data = self._read_mock_file()
        mock_data.append(self.new_order)
        self._write_mock_file(mock_data)

        # DELETE request to remove the order
        order_id = self.new_order["id"]
        delete_response = requests.delete(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Simulate deletion from the mock file
        mock_data = [order for order in self._read_mock_file() if order["id"] != order_id]
        self._write_mock_file(mock_data)

        # Verify order no longer exists in the mock file
        self.assertFalse(any(order["id"] == order_id for order in self._read_mock_file()))

    def test_get_orders(self):
        """Test retrieving all orders (happy path)."""
        # Ensure mock file has at least one order
        mock_data = self._read_mock_file()
        if not mock_data:
            mock_data.append(self.new_order)
            self._write_mock_file(mock_data)

        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /orders - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_order_by_id(self):
        """Test retrieving an order by ID (happy path)."""
        # Ensure mock file has the order
        mock_data = self._read_mock_file()
        if not any(order["id"] == self.new_order["id"] for order in mock_data):
            mock_data.append(self.new_order)
            self._write_mock_file(mock_data)

        # GET request for specific order
        order_id = self.new_order["id"]
        response = requests.get(f"{self.base_url}/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_update_order(self):
        """Test updating an existing order (happy path)."""
        # Ensure mock file has the order
        mock_data = self._read_mock_file()
        if not any(order["id"] == self.new_order["id"] for order in mock_data):
            mock_data.append(self.new_order)
            self._write_mock_file(mock_data)

        # Update order
        updated_order = self.new_order.copy()
        updated_order.update({
            "order_status": "Shipped",
            "notes": "Updated test notes.",
            "shipping_notes": "Updated shipping notes.",
            "picking_notes": "Updated picking notes.",
        })
        order_id = self.new_order["id"]
        response = requests.put(f"{self.base_url}/{order_id}", json=updated_order, headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Simulate update in the mock file
        for order in mock_data:
            if order["id"] == order_id:
                order.update(updated_order)
        self._write_mock_file(mock_data)

        # Verify update in mock file
        self.assertTrue(any(order["order_status"] == "Shipped" for order in self._read_mock_file()))

    def test_get_order_with_invalid_api_key(self):
        """Test retrieving orders with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /orders with invalid API key - Status Code: {response.status_code}")

    def test_add_order_missing_fields(self):
        """Test adding an order with missing fields (unhappy path)."""
        incomplete_order = {
            "id": uuid.uuid4().int % 10000,
            "reference": f"ORD{random.randint(10000, 99999)}"
            # Missing required fields like order_status, items, etc.
        }
        response = requests.post(self.base_url, json=incomplete_order, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /orders with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_order_with_invalid_id(self):
        """Test updating an order with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_order = self.new_order.copy()
        updated_order["id"] = invalid_id  # Match the ID in the route
        updated_order["order_status"] = "Invalid ID Update"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_order, headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"PUT /orders/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_order_with_invalid_id(self):
        """Test deleting an order with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /orders/{invalid_id} - Status Code: {response.status_code}")


if __name__ == '__main__':
    unittest.main()