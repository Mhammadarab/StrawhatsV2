import unittest
import requests
import random
from datetime import datetime


class TestItemsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/items'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        items = response.json()
        max_id = max([int(item["uid"][1:]) for item in items], default=0)

        self.test_item = {
            "uid": f"P{max_id + 1:06d}",
            "Code": "codeTEST",
            "description": "Face-to-face clear-thinking complexity",
            "shortdescription": "must",
            "UpcCode": str(random.randint(1000000000000, 9999999999999)),
            "ModelNumber": f"model-{random.randint(1000, 9999)}",
            "CommodityCode": f"comm-{random.randint(1000, 9999)}",
            "ItemLine": 11,
            "ItemGroup": 73,
            "ItemType": 14,
            "UnitPurchaseQuantity": 47,
            "UnitOrderQuantity": 13,
            "PackOrderQuantity": 11,
            "SupplierId": 34,
            "SupplierCode": "SUP423",
            "SupplierPartNumber": "E-86805-uTM",
            "Classifications_Id": [
                1,
                2
            ]
        }

    # Happy path test
    def test_get_items(self):
        """Test retrieving all items (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /items - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_item_by_uid(self):
        """Test retrieving an item by UID (happy path)."""
        response = requests.post(self.base_url, json=self.test_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        item_uid = self.test_item["uid"]

        get_response = requests.get(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /items/{item_uid} - Status Code: {get_response.status_code}, Response: {get_response.text}")

        # Clean up by deleting the item
        delete_response = requests.delete(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    # Happy path test
    def test_add_item(self):
        """Test adding a new item (happy path)."""
        response = requests.post(self.base_url, json=self.test_item, headers=self.headers)
        print(response.content)
        self.assertEqual(response.status_code, 201)

        # GET request for specific item
        item_id = self.test_item["uid"]
        get_response = requests.get(f"{self.base_url}/{item_id}", headers=self.headers)
        print(get_response.content)
        self.assertEqual( get_response.status_code, 200)

        # Clean up by deleting the item
        delete_response = requests.delete(f"{self.base_url}/{item_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_item(self):
        """Test updating an existing item (happy path)."""
        # Add item to update
        response = requests.post(self.base_url, json=self.test_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        item_uid = self.test_item["uid"]

        # Update the item
        updated_item = self.test_item.copy()
        updated_item.update({
            "Code": "codeUpdate",
            "description": "Face-to-face clear-thinking complexity",
            "shortdescription": "must",
            "UpcCode": str(random.randint(1000000000000, 9999999999999)),
            "ModelNumber": f"model-{random.randint(1000, 9999)}",
            "CommodityCode": f"comm-{random.randint(1000, 9999)}",
            "ItemLine": 11,
            "ItemGroup": 73,
            "ItemType": 14,
            "UnitPurchaseQuantity": 47,
            "UnitOrderQuantity": 13,
            "PackOrderQuantity": 11,
            "SupplierId": 34,
            "SupplierCode": "SUP423",
            "SupplierPartNumber": "E-86805-uTM"
        })
        put_response = requests.put(f"{self.base_url}/{item_uid}", json=updated_item, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify update
        get_response = requests.get(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
 

        # Clean up by deleting the item
        delete_response = requests.delete(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    # Happy path test
    def test_delete_item(self):
        """Test deleting an existing item (happy path)."""
        response = requests.post(self.base_url, json=self.test_item, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        item_uid = self.test_item["uid"]
        
        delete_response = requests.delete(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)


        get_response = requests.get(f"{self.base_url}/{item_uid}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

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
        self.assertEqual(response.status_code, 400)
        print(f"POST /items with missing fields - Status Code: {response.status_code}")

    # Unhappy path test
    def test_update_item_invalid_id(self):
        """Test updating a item with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_item = self.test_item.copy()
        updated_item["Code"] = "Updated Invalid ID item"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_item, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /items/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_item_invalid_uid(self):
        """Test deleting an item with an invalid UID, expecting 404 Not Found."""
        invalid_uid = "P999999"
        response = requests.delete(f"{self.base_url}/{invalid_uid}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /items/{invalid_uid} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
