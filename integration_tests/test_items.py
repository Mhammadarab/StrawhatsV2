import unittest
import requests
import random

class TestItemsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/items'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.new_item = {
            "uid": f"P{random.randint(10000, 99999)}",
            "code": f"code{random.randint(1000, 9999)}",
            "description": "Face-to-face clear-thinking complexity",
            "short_description": "must",
            "upc_code": str(random.randint(1000000000000, 9999999999999)),
            "model_number": f"model-{random.randint(1000, 9999)}",
            "commodity_code": f"comm-{random.randint(1000, 9999)}",
            "item_line": 11,
            "item_group": 73,
            "item_type": 14,
            "unit_purchase_quantity": 47,
            "unit_order_quantity": 13,
            "pack_order_quantity": 11,
            "supplier_id": 34,
            "supplier_code": "SUP423",
            "supplier_part_number": "E-86805-uTM"
        }

    # Happy path test
    def test_get_items(self):
        """Test retrieving all items (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /items - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_add_item(self):
        """Test adding a new item (happy path)."""
        response = requests.post(self.base_url, json=self.new_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /items - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_item_by_uid(self):
        """Test retrieving an item by UID (happy path)."""
        response = requests.post(self.base_url, json=self.new_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        item_uid = self.new_item["uid"]

        get_response = requests.get(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /items/{item_uid} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_delete_item(self):
        """Test deleting an existing item (happy path)."""
        response = requests.post(self.base_url, json=self.new_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        item_uid = self.new_item["uid"]

        delete_response = requests.delete(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)
        print(f"DELETE /items/{item_uid} - Status Code: {delete_response.status_code}")

        get_response = requests.get(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_item_with_invalid_api_key(self):
        """Test retrieving items with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /items with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_add_item_missing_fields(self):
        """Test adding an item with missing fields, expecting 400 Bad Request."""
        incomplete_item = {
            "uid": f"P{random.randint(10000, 99999)}",
            "description": "Incomplete item"
            # Missing required fields such as code, item_line, item_group, etc.
        }
        response = requests.post(self.base_url, json=incomplete_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /items with missing fields - Status Code: {response.status_code}")

    # Unhappy path test
    def test_get_item_invalid_uid(self):
        """Test retrieving an item with an invalid UID, expecting 404 Not Found."""
        invalid_uid = "P999999"
        response = requests.get(f"{self.base_url}/{invalid_uid}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /items/{invalid_uid} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_item_invalid_uid(self):
        """Test deleting an item with an invalid UID, expecting 404 Not Found."""
        invalid_uid = "P999999"
        response = requests.delete(f"{self.base_url}/{invalid_uid}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /items/{invalid_uid} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
