import json
import os
import random
import uuid
import unittest
import tempfile
import requests
from datetime import datetime

class TestSuppliersAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v1/suppliers'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Use a temporary file for the mock data
        self.temp_mock_file = tempfile.NamedTemporaryFile(delete=False, suffix=".json")
        self.mock_file_path = self.temp_mock_file.name

        # Supplier data
        self.test_supplier = {
            "id": uuid.uuid4().int % 10000,  # Unique ID
            "code": f"SUP{uuid.uuid4().int % 10000}",
            "name": "Test Supplier Ltd",
            "address": f"{random.randint(1, 9999)} Random Street",
            "address_extra": f"Apt. {random.randint(1, 999)}",
            "city": "Test City",
            "zip_code": f"{random.randint(10000, 99999)}",
            "province": "Test Province",
            "country": "Test Country",
            "contact_name": "John Doe",
            "phonenumber": f"001-{random.randint(100, 999)}-{random.randint(100, 999)}-{random.randint(1000, 9999)}x{random.randint(100, 9999)}",
            "reference": f"CL-{uuid.uuid4().int % 10000}",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z"
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

    def test_add_supplier_to_mock(self):
        """Test adding supplier and verifying its existence."""
        # POST request to add supplier
        response = requests.post(self.base_url, json=self.test_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Simulate adding supplier to mock file
        mock_data = self._read_mock_file()
        mock_data.append(self.test_supplier)
        self._write_mock_file(mock_data)

        # Validate the supplier in the mock file
        self.assertTrue(any(sup["id"] == self.test_supplier["id"] for sup in self._read_mock_file()))

    def test_delete_supplier_from_mock(self):
        """Test deleting a supplier and ensuring it is removed from the mock."""
        # Add supplier to mock file first
        mock_data = self._read_mock_file()
        mock_data.append(self.test_supplier)
        self._write_mock_file(mock_data)

        # DELETE request to remove the supplier
        supplier_id = self.test_supplier["id"]
        delete_response = requests.delete(f"{self.base_url}/{supplier_id}", headers=self.headers)
        if delete_response.status_code != 200:
            print(f"Unexpected status code: {delete_response.status_code}, Response: {delete_response.text}")
        self.assertEqual(delete_response.status_code, 200)

        # Simulate deletion from the mock file
        mock_data = [sup for sup in self._read_mock_file() if sup["id"] != supplier_id]
        self._write_mock_file(mock_data)

        # Verify supplier no longer exists in the mock file
        self.assertFalse(any(sup["id"] == supplier_id for sup in self._read_mock_file()))

    def test_get_suppliers(self):
        """Test retrieving all suppliers (happy path)."""
        # Ensure mock file has at least one supplier
        mock_data = self._read_mock_file()
        if not mock_data:
            mock_data.append(self.test_supplier)
            self._write_mock_file(mock_data)

        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /suppliers - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_supplier_by_id(self):
        """Test retrieving a supplier by ID (happy path)."""
        # Ensure mock file has the supplier
        mock_data = self._read_mock_file()
        if not any(sup["id"] == self.test_supplier["id"] for sup in mock_data):
            mock_data.append(self.test_supplier)
            self._write_mock_file(mock_data)

        # GET request for specific supplier
        supplier_id = self.test_supplier["id"]
        response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_update_supplier(self):
        """Test updating an existing supplier (happy path)."""
        # Ensure mock file has the supplier
        mock_data = self._read_mock_file()
        if not any(sup["id"] == self.test_supplier["id"] for sup in mock_data):
            mock_data.append(self.test_supplier)
            self._write_mock_file(mock_data)

        # Verify the supplier exists in the mock file before updating
        self.assertTrue(any(sup["id"] == self.test_supplier["id"] for sup in self._read_mock_file()))

        # Update supplier
        updated_supplier = self.test_supplier.copy()
        updated_supplier.update({
            "name": "Updated Supplier Ltd",
            "city": "Updated City",
            "phonenumber": "001-555-555-5555x9999"
        })
        supplier_id = self.test_supplier["id"]
        response = requests.put(f"{self.base_url}/{supplier_id}", json=updated_supplier, headers=self.headers)
        if response.status_code != 200:
            print(f"Unexpected status code: {response.status_code}, Response: {response.text}")
        self.assertEqual(response.status_code, 200)

        # Simulate update in the mock file
        for sup in mock_data:
            if sup["id"] == supplier_id:
                sup.update(updated_supplier)
        self._write_mock_file(mock_data)

        # Verify update in mock file
        self.assertTrue(any(sup["name"] == "Updated Supplier Ltd" for sup in self._read_mock_file()))

    def test_get_supplier_with_invalid_api_key(self):
        """Test retrieving suppliers with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /suppliers with invalid API key - Status Code: {response.status_code}")

    def test_add_supplier_missing_fields(self):
        """Test adding a supplier with missing fields (unhappy path)."""
        incomplete_supplier = {
            "id": uuid.uuid4().int % 10000,
            "name": "Incomplete Supplier"
        }
        response = requests.post(self.base_url, json=incomplete_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /suppliers with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_supplier_with_invalid_id(self):
        """Test updating a supplier with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_supplier = self.test_supplier.copy()
        updated_supplier["id"] = invalid_id  # Match the ID in the route
        updated_supplier["name"] = "Invalid ID Supplier"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"PUT /suppliers/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_supplier_with_invalid_id(self):
        """Test deleting a supplier with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /suppliers/{invalid_id} - Status Code: {response.status_code}")


if __name__ == '__main__':
    unittest.main()