import unittest
import requests
from datetime import datetime

class TestItemTypesAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/item_types'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        item_types = response.json()
        max_id = max([item_type["id"] for item_type in item_types], default=0)

        # Item type data
        self.new_item_type = {
            "id": max_id + 1,
            "name": f"Item Type {max_id + 1}",
            "description": "This is a new item type.",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z"
        }

    def tearDown(self):
        # Clean up any item types created during the tests
        item_type_id = self.new_item_type["id"]
        requests.delete(f"{self.base_url}/{item_type_id}", headers=self.headers)

    def test_get_item_types(self):
        """Test retrieving all item types (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /item_types - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_item_type_by_id(self):
        """Test retrieving an item type by ID (happy path)."""
        # Add a new item type
        post_response = requests.post(self.base_url, json=self.new_item_type, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_type_id = self.new_item_type["id"]

        # GET request for specific item type
        response = requests.get(f"{self.base_url}/{item_type_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_add_item_type(self):
        """Test adding a new item type (happy path)."""
        response = requests.post(self.base_url, json=self.new_item_type, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the item type exists
        item_type_id = self.new_item_type["id"]
        get_response = requests.get(f"{self.base_url}/{item_type_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

    def test_update_item_type(self):
        """Test updating an existing item type (happy path)."""
        # Add an item type to update
        post_response = requests.post(self.base_url, json=self.new_item_type, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_type_id = self.new_item_type["id"]

        # Update the item type
        updated_item_type = self.new_item_type.copy()
        updated_item_type.update({
            "name": "Updated Item Type",
            "description": "This item type has been updated."
        })
        put_response = requests.put(f"{self.base_url}/{item_type_id}", json=updated_item_type, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{item_type_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        item_type_data = get_response.json()
        self.assertEqual(item_type_data["name"], updated_item_type["name"])
        self.assertEqual(item_type_data["description"], updated_item_type["description"])

        # Revert the update
        revert_response = requests.put(f"{self.base_url}/{item_type_id}", json=self.new_item_type, headers=self.headers)
        self.assertEqual(revert_response.status_code, 204)

    def test_delete_item_type(self):
        """Test deleting an existing item type (happy path)."""
        # Add an item type to delete
        post_response = requests.post(self.base_url, json=self.new_item_type, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_type_id = self.new_item_type["id"]

        # DELETE request to remove the item type
        response = requests.delete(f"{self.base_url}/{item_type_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the item type no longer exists
        get_response = requests.get(f"{self.base_url}/{item_type_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_item_type_with_invalid_api_key(self):
        """Test retrieving item types with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /item_types with invalid API key - Status Code: {response.status_code}")

    def test_add_item_type_missing_fields(self):
        """Test adding item type with missing fields (unhappy path)."""
        incomplete_item_type = {
            "id": self.new_item_type["id"] + 1,
            "name": f"Item Type {self.new_item_type['id'] + 1}"
        }
        response = requests.post(self.base_url, json=incomplete_item_type, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /item_types with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_item_type_with_invalid_id(self):
        """Test updating an item type with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_item_type = self.new_item_type.copy()
        updated_item_type["name"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item_type, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /item_types/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_item_type_with_invalid_id(self):
        """Test deleting an item type with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /item_types/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()