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
        print(self.headers)
        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        if response.status_code == 200 and response.content:
            warehouses = response.json()
            for warehouse in warehouses:
                if isinstance(warehouse.get("contact"), dict):
                    warehouse["contacts"] = [warehouse.pop("contact")]
            max_id = max([warehouse["id"] for warehouse in warehouses], default=0)
        else:
            max_id = 0
        self.test_warehouse = {
            "id": max_id + 1,
            "code": "WAREHOUSE",
            "name": "Test Warehouse",
            "address": "Test Street",
            "zip": "9999 AB",
            "city": "Test City",
            "province": "Test Province",
            "country": "NL",
            "contact": [  # Updated to use a list of contacts
                {
                    "name": "John Doe",
                    "phone": f"(078) {random.randint(1000000, 9999999)}",
                    "email": f"test{random.randint(100, 999)}@example.net"
                },
                {
                    "name": "Jane Smith",
                    "phone": f"(078) {random.randint(1000000, 9999999)}",
                    "email": f"test{random.randint(100, 999)}@example.net"
                }
            ],
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
        print("WE IN THE TEST GET WAREHOUSES")

    def test_get_warehouse_by_id(self):
        """Test retrieving a warehouse by ID (happy path)."""
        # Add a new warehouse
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        warehouse_id = self.test_warehouse["id"]
        print("WE IN THE TEST GET WAREHOUSES ID [ CREATE ] ")


        # Retrieve the warehouse by its ID
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print("WE IN THE TEST GET WAREHOUSES ID [ GET ] ")

        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)
        print("WE IN THE TEST GET WAREHOUSES ID [ DELETE ] ")


    def test_add_warehouse(self):
        """Test adding a new warehouse (happy path)."""
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print("WE IN THE TEST ADD WAREHOUSES [ CREATE ] ")


        # Verify warehouse was added
        warehouse_id = self.test_warehouse["id"]
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print("WE IN THE TEST ADD WAREHOUSES [ GET ] ")

        # print(f"GET /warehouses/{warehouse_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)
        print("WE IN THE TEST ADD WAREHOUSES [ DELETE ] ")

    def test_update_warehouse(self):
        """Test updating an existing warehouse (happy path)."""
        # Add warehouse to update
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        warehouse_id = self.test_warehouse["id"]
        print("WE IN THE TEST UPDATE WAREHOUSES [ CREATE ] ")


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
        print("WE IN THE TEST UPDATE WAREHOUSES [ PUT ] ")


        # Verify update
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        warehouse_data = get_response.json()
        print("WE IN THE TEST ADD WAREHOUSES [ GET ] ")


        # Debugging step to print the response data
        print(f"GET Response Data: {warehouse_data}")

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)
        print("WE IN THE TEST UPDATE WAREHOUSES [ DELETE ] ")


    def test_delete_warehouse(self):
        """Test deleting an existing warehouse (happy path)."""
        # Add warehouse to delete
        response = requests.post(self.base_url, json=self.test_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        warehouse_id = self.test_warehouse["id"]
        print("WE IN THE TEST DELETE WAREHOUSES [ POST ] ")


        # Delete warehouse
        delete_response = requests.delete(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)
        print("WE IN THE TEST DELETE WAREHOUSES [ DELETE ] ")


        # Verify the shipment no longer exists
        get_response = requests.get(f"{self.base_url}/{warehouse_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)
        print("WE IN THE TEST DELETE WAREHOUSES [ GET ] ")


    def test_get_warehouse_with_invalid_api_key(self):
        """Test retrieving warehouses with invalid API key, should handle properly (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print("WE IN THE TEST GET WAREHOUSE WITH INVALID API KEY [ GET ] ")

        print(f"GET /warehouses with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_warehouse_missing_fields(self):
        """Test adding warehouse with missing fields, expecting a controlled response (unhappy path)."""
        incomplete_warehouse = {"id": self.test_warehouse["id"] + 1, "reference": f"SH{self.test_warehouse['id'] + 1}"}
        response = requests.post(self.base_url, json=incomplete_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print("WE IN THE TEST ADD WAREHOUSE WITH MISSING FIELDS [ POST ] ")

        print(f"POST /warehouses with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_warehouse_invalid_id(self):
        """Test updating a warehouse with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_warehouse = self.test_warehouse.copy()
        updated_warehouse["name"] = "Updated Invalid ID Warehouse"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_warehouse, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /warehouses/{invalid_id} - Status Code: {response.status_code}")
        print("WE IN THE TEST UPDATE WAREHOUSE WITH INVALID ID [ PUT ] ")


    def test_delete_warehouse_invalid_id(self):
        """Test deleting a warehouse with invalid ID, expecting controlled response (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print("WE IN THE TEST DELETE WAREHOUSE WITH INVALID ID [ DELETE ] ")

        print(f"DELETE /warehouses/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
