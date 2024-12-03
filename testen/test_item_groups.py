import random
import unittest
import requests

class TestItemGroupsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/item_groups'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        # Assume an existing item group ID for testing
        self.existing_item_group_id = random.randint(1, 99)  
        self.updated_item_group = {
            "id": self.existing_item_group_id,
            "name": "Updated Stationery Group",
            "description": "This item group has been updated."
        }

    # Happy path test
    def test_get_item_groups(self):
        """Test retrieving all item groups (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /item_groups - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_item_group_by_id(self):
        """Test retrieving an item group by ID (happy path)."""
        item_group_id = self.existing_item_group_id
        get_response = requests.get(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /item_groups/{item_group_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_item_group(self):
        """Test updating an existing item group (happy path)."""
        put_response = requests.put(f"{self.base_url}/{self.existing_item_group_id}", json=self.updated_item_group, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)
        print(f"PUT /item_groups/{self.existing_item_group_id} - Status Code: {put_response.status_code}")

        # Verify the item group update by fetching it again
        get_response = requests.get(f"{self.base_url}/{self.existing_item_group_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        item_group_data = get_response.json()
        self.assertEqual(item_group_data["name"], self.updated_item_group["name"])
        self.assertEqual(item_group_data["description"], self.updated_item_group["description"])

    # Happy path test
    def test_delete_item_group(self):
        """Test deleting an existing item group (happy path)."""
        item_group_id = self.existing_item_group_id
        delete_response = requests.delete(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)
        print(f"DELETE /item_groups/{item_group_id} - Status Code: {delete_response.status_code}")

        # Verify that the item line was deleted
        get_response = requests.get(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)
        self.assertEqual(get_response.text.strip(), f"ItemGroup with ID {item_group_id} not found.")

    # Unhappy path test
    def test_get_item_group_with_invalid_api_key(self):
        """Test retrieving item groups with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /item_groups with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_update_item_group_with_invalid_id(self):
        """Test updating an item group with an invalid ID, expecting controlled response."""
        invalid_id = 999999
        updated_item_group = self.updated_item_group.copy()
        updated_item_group["name"] = "Invalid ID Group"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item_group, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /item_groups/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_item_group_with_invalid_id(self):
        """Test deleting an item group with an invalid ID, expecting controlled response."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /item_groups/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
