import unittest
import requests
from datetime import datetime

class TestItemGroupsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/item_groups'
        self.headers = {'API_KEY': 'owner'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        item_groups = response.json()
        max_id = max([item_group["id"] for item_group in item_groups], default=0)

        # Item group data
        self.new_item_group = {
            "id": max_id + 1,
            "name": f"Item Group {max_id + 1}",
            "description": "This is a new item group.",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z"
        }

    def tearDown(self):
        # Clean up any item groups created during the tests
        item_group_id = self.new_item_group["id"]
        requests.delete(f"{self.base_url}/{item_group_id}", headers=self.headers)

    def test_get_item_groups(self):
        """Test retrieving all item groups (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /item_groups - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_item_group_by_id(self):
        """Test retrieving an item group by ID (happy path)."""
        # Add a new item group
        post_response = requests.post(self.base_url, json=self.new_item_group, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_group_id = self.new_item_group["id"]

        # GET request for specific item group
        response = requests.get(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_add_item_group(self):
        """Test adding a new item group (happy path)."""
        response = requests.post(self.base_url, json=self.new_item_group, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the item group exists
        item_group_id = self.new_item_group["id"]
        get_response = requests.get(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

    def test_update_item_group(self):
        """Test updating an existing item group (happy path)."""
        # Add an item group to update
        post_response = requests.post(self.base_url, json=self.new_item_group, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_group_id = self.new_item_group["id"]

        # Update the item group
        updated_item_group = self.new_item_group.copy()
        updated_item_group.update({
            "name": "Updated Item Group",
            "description": "This item group has been updated."
        })
        put_response = requests.put(f"{self.base_url}/{item_group_id}", json=updated_item_group, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        item_group_data = get_response.json()
        self.assertEqual(item_group_data["name"], updated_item_group["name"])
        self.assertEqual(item_group_data["description"], updated_item_group["description"])

        # Revert the update
        revert_response = requests.put(f"{self.base_url}/{item_group_id}", json=self.new_item_group, headers=self.headers)
        self.assertEqual(revert_response.status_code, 204)

    def test_delete_item_group(self):
        """Test deleting an existing item group (happy path)."""
        # Add an item group to delete
        post_response = requests.post(self.base_url, json=self.new_item_group, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_group_id = self.new_item_group["id"]

        # DELETE request to remove the item group
        response = requests.delete(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the item group no longer exists
        get_response = requests.get(f"{self.base_url}/{item_group_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_item_group_with_invalid_api_key(self):
        """Test retrieving item groups with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /item_groups with invalid API key - Status Code: {response.status_code}")

    def test_add_item_group_missing_fields(self):
        """Test adding item group with missing fields (unhappy path)."""
        incomplete_item_group = {
            "id": self.new_item_group["id"] + 1,
            "name": f"Item Group {self.new_item_group['id'] + 1}"
        }
        response = requests.post(self.base_url, json=incomplete_item_group, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /item_groups with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_item_group_invalid_id(self):
        """Test updating an item group with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_item_group = self.new_item_group.copy()
        updated_item_group["name"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item_group, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /item_groups/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_item_group_invalid_id(self):
        """Test deleting an item group with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /item_groups/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()