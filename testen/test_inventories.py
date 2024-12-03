import json
import os
import uuid
import unittest
import tempfile
import requests


class TestInventoriesAPIV2(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/inventories'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Use a temporary file for the mock data
        self.temp_mock_file = tempfile.NamedTemporaryFile(delete=False, suffix=".json")
        self.mock_file_path = self.temp_mock_file.name

        # Inventory data
        self.test_inventory = {
            "id": uuid.uuid4().int % 10000,  # Unique ID
            "item_Id": "P000002",
            "description": "Focused transitional alliance",
            "item_Reference": f"ref{uuid.uuid4().int % 10000}",
            "locations": {2271: 19, 2293: 19},
            "total_On_Hand": 100,
            "total_Expected": 0,
            "total_Ordered": 50,
            "total_Allocated": 30,
            "total_Available": 70,
            "created_At": "2023-10-07T16:08:24",
            "updated_At": "2023-10-07T16:08:24"
        }

        # Initialize the temporary mock file with an empty JSON array
        with open(self.mock_file_path, 'w') as mock_file:
            json.dump([], mock_file)

    def tearDown(self):
        # Delete the temporary file after the test run
        os.unlink(self.mock_file_path)

    def _read_mock_file(self):
        """Reads data from the mock file."""
        with open(self.mock_file_path, 'r') as mock_file:
            return json.load(mock_file)

    def _write_mock_file(self, data):
        """Writes data to the mock file."""
        with open(self.mock_file_path, 'w') as mock_file:
            json.dump(data, mock_file, indent=4)

    def test_add_inventory_to_mock(self):
        """Test adding inventory and verifying its existence."""
        # POST request to add inventory
        response = requests.post(self.base_url, json=self.test_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Simulate adding inventory to mock file
        mock_data = self._read_mock_file()
        mock_data.append(self.test_inventory)
        self._write_mock_file(mock_data)

        # Validate the inventory in the mock file
        self.assertTrue(any(inv["id"] == self.test_inventory["id"] for inv in self._read_mock_file()))

    def test_delete_inventory_from_mock(self):
        """Test deleting an inventory and ensuring it is removed from the mock."""
        # Add inventory to mock file first
        mock_data = self._read_mock_file()
        mock_data.append(self.test_inventory)
        self._write_mock_file(mock_data)

        # DELETE request to remove the inventory
        inventory_id = self.test_inventory["id"]
        delete_response = requests.delete(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Simulate deletion from the mock file
        mock_data = [inv for inv in self._read_mock_file() if inv["id"] != inventory_id]
        self._write_mock_file(mock_data)

        # Verify inventory no longer exists in the mock file
        self.assertFalse(any(inv["id"] == inventory_id for inv in self._read_mock_file()))

    def test_get_inventories(self):
        """Test retrieving all inventories (happy path)."""
        # Ensure mock file has at least one inventory
        mock_data = self._read_mock_file()
        if not mock_data:
            mock_data.append(self.test_inventory)
            self._write_mock_file(mock_data)

        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /v2/inventories - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_inventory_by_id(self):
        """Test retrieving an inventory by ID (happy path)."""
        # Ensure mock file has the inventory
        mock_data = self._read_mock_file()
        if not any(inv["id"] == self.test_inventory["id"] for inv in mock_data):
            mock_data.append(self.test_inventory)
            self._write_mock_file(mock_data)

        # GET request for specific inventory
        inventory_id = self.test_inventory["id"]
        response = requests.get(f"{self.base_url}/{inventory_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_update_inventory(self):
        """Test updating an existing inventory (happy path)."""
        # Ensure mock file has the inventory
        mock_data = self._read_mock_file()
        if not any(inv["id"] == self.test_inventory["id"] for inv in mock_data):
            mock_data.append(self.test_inventory)
            self._write_mock_file(mock_data)

        # Update inventory
        updated_inventory = self.test_inventory.copy()
        updated_inventory.update({
            "description": "Updated inventory description",
            "total_On_Hand": 150,
            "total_Available": 100
        })
        inventory_id = self.test_inventory["id"]
        response = requests.put(f"{self.base_url}/{inventory_id}", json=updated_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Simulate update in the mock file
        for inv in mock_data:
            if inv["id"] == inventory_id:
                inv.update(updated_inventory)
        self._write_mock_file(mock_data)

        # Verify update in mock file
        self.assertTrue(any(inv["description"] == "Updated inventory description" for inv in self._read_mock_file()))

    def test_get_inventory_with_invalid_api_key(self):
        """Test retrieving inventories with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /v2/inventories with invalid API key - Status Code: {response.status_code}")

    def test_add_inventory_missing_fields(self):
        """Test adding an inventory with missing fields (unhappy path)."""
        incomplete_inventory = {
            "id": uuid.uuid4().int % 10000,
            "description": "Incomplete inventory entry"
        }
        response = requests.post(self.base_url, json=incomplete_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /v2/inventories with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_inventory_with_invalid_id(self):
        """Test updating an inventory with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_inventory = self.test_inventory.copy()
        updated_inventory["id"] = invalid_id  # Match the ID in the route
        updated_inventory["description"] = "Invalid ID test"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_inventory, headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"PUT /v2/inventories/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_inventory_with_invalid_id(self):
        """Test deleting an inventory with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /v2/inventories/{invalid_id} - Status Code: {response.status_code}")


if __name__ == '__main__':
    unittest.main()