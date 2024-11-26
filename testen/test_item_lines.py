import random
import unittest
import requests

class TestItemLinesAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/item_lines'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.existing_item_line_id = random.randint(1, 99)  # Assume an existing item line ID for testing
        self.updated_item_line = {
            "id": self.existing_item_line_id,
            "name": "Updated Health & Wellness",
            "description": "This item line has been updated."
        }

    # Happy path test
    def test_get_item_lines(self):
        """Test retrieving all item lines (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /item_lines - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_item_line_by_id(self):
        """Test retrieving an item line by ID (happy path)."""
        item_line_id = self.existing_item_line_id
        get_response = requests.get(f"{self.base_url}/{item_line_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /item_lines/{item_line_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_item_line(self):
        """Test updating an existing item line (happy path)."""
        put_response = requests.put(f"{self.base_url}/{self.existing_item_line_id}", json=self.updated_item_line, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)
        print(f"PUT /item_lines/{self.existing_item_line_id} - Status Code: {put_response.status_code}")

        # Verify update by fetching the item line again
        get_response = requests.get(f"{self.base_url}/{self.existing_item_line_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        item_line_data = get_response.json()
        self.assertEqual(item_line_data["name"], self.updated_item_line["name"])
        self.assertEqual(item_line_data["description"], self.updated_item_line["description"])

    # Happy path test
    def test_delete_item_line(self):
        """Test deleting an existing item line (happy path)."""
        delete_response = requests.delete(f"{self.base_url}/{self.existing_item_line_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)
        print(f"DELETE /item_lines/{self.existing_item_line_id} - Status Code: {delete_response.status_code}")

        # Verify that the item line was deleted
        get_response = requests.get(f"{self.base_url}/{self.existing_item_line_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_item_line_with_invalid_api_key(self):
        """Test retrieving item lines with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /item_lines with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_update_item_line_with_invalid_id(self):
        """Test updating an item line with an invalid ID, expecting controlled response."""
        invalid_id = 999999
        updated_item_line = self.updated_item_line.copy()
        updated_item_line["name"] = "Invalid ID Update"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item_line, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /item_lines/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_item_line_with_invalid_id(self):
        """Test deleting an item line with an invalid ID, expecting controlled response."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /item_lines/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
