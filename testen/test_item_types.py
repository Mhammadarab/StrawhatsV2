import random
import unittest
import requests

class TestItemTypesAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/item_types'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.existing_item_type_id = random.randint(1, 99)  # Assuming an existing item type ID for testing
        self.updated_item_type = {
            "id": self.existing_item_type_id,
            "name": "Updated Desktop",
            "description": "This item type has been updated."
        }

    # Happy path test
    def test_get_item_types(self):
        """Test retrieving all item types (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /item_types - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_item_type_by_id(self):
        """Test retrieving an item type by ID (happy path)."""
        item_type_id = self.existing_item_type_id
        get_response = requests.get(f"{self.base_url}/{item_type_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /item_types/{item_type_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_item_type(self):
        """Test updating an existing item type (happy path)."""
        put_response = requests.put(f"{self.base_url}/{self.existing_item_type_id}", json=self.updated_item_type, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)
        print(f"PUT /item_types/{self.existing_item_type_id} - Status Code: {put_response.status_code}")

        # Verify that the item type was updated successfully
        get_response = requests.get(f"{self.base_url}/{self.existing_item_type_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        item_type_data = get_response.json()
        self.assertEqual(item_type_data["name"], self.updated_item_type["name"])
        self.assertEqual(item_type_data["description"], self.updated_item_type["description"])

    # Happy path test
    def test_delete_item_type(self):
        """Test deleting an existing item type (happy path)."""
        delete_response = requests.delete(f"{self.base_url}/{self.existing_item_type_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)
        print(f"DELETE /item_types/{self.existing_item_type_id} - Status Code: {delete_response.status_code}")

        # Verify that the item type was deleted
        get_response = requests.get(f"{self.base_url}/{self.existing_item_type_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_item_type_with_invalid_api_key(self):
        """Test retrieving item types with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /item_types with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_update_item_type_with_invalid_id(self):
        """Test updating an item type with an invalid ID, expecting controlled response."""
        invalid_id = 999999
        updated_item_type = self.updated_item_type.copy()
        updated_item_type["name"] = "Invalid ID Update"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item_type, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /item_types/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_item_type_with_invalid_id(self):
        """Test deleting an item type with an invalid ID, expecting controlled response."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /item_types/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
