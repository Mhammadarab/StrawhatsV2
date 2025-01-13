import unittest
import requests
import random
from datetime import datetime

class TestWarehousesAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/warehouses'
        self.headers = {'API_KEY': 'owner'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        print(response.content)
        warehouses = response.json()
        max_id = max([warehouse["id"] for warehouse in warehouses], default=0)
    

        self.test_warehouse = {
            "id": max_id + 1,
            "code": "WAREHOUSE",
            "name": "Test Warehouse",
            "address": "Test Street",
            "zip": "9999 AB",
            "city": "Test City",
            "province": "Test Province",
            "country": "NL",
            "contact": {
                "name": "John Doe",
                "phone": f"(078) {random.randint(1000000, 9999999)}",
                "email": f"test{random.randint(100, 999)}@example.net"
            },
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z",
            "classifications_id": [
                1,
                2
            ]
        }

    def test_get_warehouses(self):
        """Test retrieving all warehouses (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /warehouses - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_warehouse_by_id(self):
        """Test retrieving a warehouse by ID (happy path)."""
        # Add a new warehouse
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        warehouse_id = self.test_warehouse["id"]

        # Retrieve the warehouse by its ID
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_warehouse(self):
        """Test adding a new warehouse (happy path)."""
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify warehouse was added
        warehouse_id = self.test_warehouse["id"]
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        # print(f"GET /warehouses/{warehouse_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_warehouse(self):
        """Test updating an existing warehouse (happy path)."""
        # Add warehouse to update
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        warehouse_id = self.test_warehouse["id"]

        # Update the warehouse
        updated_warehouse = self.test_warehouse.copy()
        updated_warehouse.update({
            "name": "Updated Warehouse",
            "address": "456 Updated Street",
            "city": "Updated City",
            "province": "Updated Province"
        })
        put_response = requests.put(f"{self.base_url}/{warehouse_id}", json=updated_warehouse, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify update
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        warehouse_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {warehouse_data}")

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_warehouse(self):
        """Test deleting an existing warehouse (happy path)."""
        # Add warehouse to delete
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        warehouse_id = self.test_warehouse["id"]

        # Delete warehouse
        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

        # Verify the shipment no longer exists
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_warehouse_with_invalid_api_key(self):
        """Test retrieving warehouses with invalid API key, should handle properly (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /warehouses with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_warehouse_missing_fields(self):
        """Test adding warehouse with missing fields, expecting a controlled response (unhappy path)."""
        incomplete_warehouse = {"id": self.test_warehouse["id"] + 1, "reference": f"SH{self.test_warehouse['id'] + 1}"}
        response = requests.post(self.base_url, json=incomplete_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /warehouses with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_warehouse_invalid_id(self):
        """Test updating a warehouse with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_warehouse = self.test_warehouse.copy()
        updated_warehouse["name"] = "Updated Invalid ID Warehouse"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /warehouses/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_warehouse_invalid_id(self):
        """Test deleting a warehouse with invalid ID, expecting controlled response (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /warehouses/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
