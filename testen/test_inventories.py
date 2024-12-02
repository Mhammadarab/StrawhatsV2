import random
import unittest
import requests

class TestInventoriesAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/inventories'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.test_inventory = {
            "id": random.randint(1000, 9999),  # Random ID
            "item_id": f"P{random.randint(100000, 999999)}",  # Random item_id
            "description": "Face-to-face clear-thinking complexity",
            "item_reference": f"ref{random.randint(1000, 9999)}",
            "locations": [3211, 24700, 14123],
            "total_on_hand": 100,
            "total_expected": 0,
            "total_ordered": 50,
            "total_allocated": 30,
            "total_available": 70,
            "created_at": "2023-10-07 16:08:24",
            "updated_at": "2023-10-07 16:08:24"
        }

    # Happy path test
    def test_get_inventories(self):
        """Test retrieving all inventories (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /inventories - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_add_inventory(self):
        """Test adding a new inventory (happy path)."""
        response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # Verify by GET request
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"POST /inventories - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_inventory_by_id(self):
        """Test retrieving an inventory by ID (happy path)."""
        response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # Retrieve by ID
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /inventories/{inventory_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_inventory(self):
        """Test updating an existing inventory (happy path)."""
        response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # Update inventory
        updated_inventory = self.test_inventory.copy()
        updated_inventory.update({
            "description": "Updated inventory description",
            "total_on_hand": 150,
            "total_available": 100
        })
        put_response = requests.put(f"{self.base_url}/{inventory_id}", json=updated_inventory, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        inventory_data = get_response.json()
        self.assertEqual(inventory_data["description"], updated_inventory["description"])

    # Happy path test
    def test_delete_inventory(self):
        """Test deleting an inventory (happy path)."""
        response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # Delete inventory
        delete_response = requests.delete(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Verify deletion
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_inventory_with_invalid_api_key(self):
        """Test retrieving inventories with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /inventories with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_add_inventory_missing_fields(self):
        """Test adding an inventory with missing fields (unhappy path)."""
        incomplete_inventory = {
            "id": random.randint(1000, 9999),
            "description": "Incomplete inventory entry"
        }
        response = requests.post(self.base_url, json=incomplete_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /inventories with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    # Unhappy path test
    def test_update_inventory_with_invalid_id(self):
        """Test updating an inventory with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_inventory = self.test_inventory.copy()
        updated_inventory["description"] = "Invalid ID test"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /inventories/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_inventory_with_invalid_id(self):
        """Test deleting an inventory with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /inventories/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
