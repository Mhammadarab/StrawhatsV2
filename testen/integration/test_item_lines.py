import unittest
import requests
from datetime import datetime

class TestItemLinesAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/item_lines'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        item_lines = response.json()
        max_id = max([item_line["id"] for item_line in item_lines], default=0)

        # Item line data
        self.new_item_line = {
            "id": max_id + 1,
            "name": f"Item Line {max_id + 1}",
            "description": "This is a new item line.",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z"
        }

    def tearDown(self):
        # Clean up any item lines created during the tests
        item_line_id = self.new_item_line["id"]
        requests.delete(f"{self.base_url}/{item_line_id}", headers=self.headers)

    def test_get_item_lines(self):
        """Test retrieving all item lines (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /item_lines - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_item_line_by_id(self):
        """Test retrieving an item line by ID (happy path)."""
        # Add a new item line
        post_response = requests.post(self.base_url, json=self.new_item_line, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_line_id = self.new_item_line["id"]

        # GET request for specific item line
        response = requests.get(f"{self.base_url}/{item_line_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_add_item_line(self):
        """Test adding a new item line (happy path)."""
        response = requests.post(self.base_url, json=self.new_item_line, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the item line exists
        item_line_id = self.new_item_line["id"]
        get_response = requests.get(f"{self.base_url}/{item_line_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

    def test_update_item_line(self):
        """Test updating an existing item line (happy path)."""
        # Add an item line to update
        post_response = requests.post(self.base_url, json=self.new_item_line, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_line_id = self.new_item_line["id"]

        # Update the item line
        updated_item_line = self.new_item_line.copy()
        updated_item_line.update({
            "name": "Updated Item Line",
            "description": "This item line has been updated."
        })
        put_response = requests.put(f"{self.base_url}/{item_line_id}", json=updated_item_line, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{item_line_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        item_line_data = get_response.json()
        self.assertEqual(item_line_data["name"], updated_item_line["name"])
        self.assertEqual(item_line_data["description"], updated_item_line["description"])

        # Revert the update
        revert_response = requests.put(f"{self.base_url}/{item_line_id}", json=self.new_item_line, headers=self.headers)
        self.assertEqual(revert_response.status_code, 204)

    def test_delete_item_line(self):
        """Test deleting an existing item line (happy path)."""
        # Add an item line to delete
        post_response = requests.post(self.base_url, json=self.new_item_line, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        item_line_id = self.new_item_line["id"]

        # DELETE request to remove the item line
        response = requests.delete(f"{self.base_url}/{item_line_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the item line no longer exists
        get_response = requests.get(f"{self.base_url}/{item_line_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_item_line_with_invalid_api_key(self):
        """Test retrieving item lines with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /item_lines with invalid API key - Status Code: {response.status_code}")

    def test_add_item_line_missing_fields(self):
        """Test adding item line with missing fields (unhappy path)."""
        incomplete_item_line = {
            "id": self.new_item_line["id"] + 1,
            "name": f"Item Line {self.new_item_line['id'] + 1}"
        }
        response = requests.post(self.base_url, json=incomplete_item_line, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /item_lines with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_item_line_invalid_id(self):
        """Test updating an item line with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_item_line = self.new_item_line.copy()
        updated_item_line["name"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item_line, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /item_lines/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_item_line_invalid_id(self):
        """Test deleting an item line with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /item_lines/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()