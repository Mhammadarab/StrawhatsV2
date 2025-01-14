import unittest
import requests
from datetime import datetime

class TestInventoriesAPIV2(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/inventories'
        self.headers = {'API_KEY': 'owner'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        inventories = response.json()
        max_id = max([inventory["id"] for inventory in inventories], default=0)

        # Inventory data
        self.test_inventory = {
            "id": max_id + 1,
            "item_id": "P000001",
            "description": "Focused transitional alliance",
            "item_reference": f"ref{max_id + 1}",
            "locations": {2271: 19, 2293: 19},
            "total_on_hand": 100,
            "total_expected": 0,
            "total_ordered": 50,
            "total_allocated": 30,
            "total_available": 70,
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z"
        }

    def test_get_inventories(self):
        """Test retrieving all inventories (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /inventories - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_inventory_by_id(self):
        """Test retrieving an inventory by ID (happy path)."""
        # Add a new inventory
        post_response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        
        # Print the response content for debugging
        print(f"POST /inventories - Status Code: {post_response.status_code}, Response: {post_response.text}")
        
        self.assertEqual(post_response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # GET request for specific inventory
        response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the inventory
        delete_response = requests.delete(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_inventory(self):
        """Test adding a new inventory (happy path)."""
        response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the inventory exists
        inventory_id = self.test_inventory["id"]
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        # Clean up by deleting the inventory
        delete_response = requests.delete(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_inventory(self):
        """Test updating an existing inventory (happy path)."""
        # Add an inventory to update
        post_response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # Update the inventory
        updated_inventory = self.test_inventory.copy()
        updated_inventory.update({
            "description": "Updated description",
            "total_on_hand": 150
        })
        put_response = requests.put(f"{self.base_url}/{inventory_id}", json=updated_inventory, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        inventory_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {inventory_data}")

        # Clean up by deleting the inventory
        delete_response = requests.delete(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_inventory(self):
        """Test deleting an existing inventory (happy path)."""
        # Add an inventory to delete
        post_response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        inventory_id = self.test_inventory["id"]

        # DELETE request to remove the inventory
        response = requests.delete(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the inventory no longer exists
        get_response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_inventory_with_invalid_api_key(self):
        """Test retrieving inventories with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /inventories with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_inventory_missing_fields(self):
        """Test adding inventory with missing fields (unhappy path)."""
        incomplete_inventory = {
            "id": self.test_inventory["id"] + 1,
            "item_id": "P000003"
        }
        response = requests.post(self.base_url, json=incomplete_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /inventories with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_inventory_invalid_id(self):
        """Test updating an inventory with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_inventory = self.test_inventory.copy()
        updated_inventory["description"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /inventories/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_inventory_invalid_id(self):
        """Test deleting an inventory with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /inventories/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()