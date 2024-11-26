import unittest
import requests
import random
from datetime import datetime

class TestSuppliersAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/suppliers'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.new_supplier = {
            "id": random.randint(1000, 9999),
            "code": f"SUP{random.randint(1000, 9999)}",
            "name": "Test Supplier Ltd",
            "address": f"{random.randint(1, 9999)} Random Street",
            "address_extra": f"Apt. {random.randint(1, 999)}",
            "city": "Test City",
            "zip_code": f"{random.randint(10000, 99999)}",
            "province": "Test Province",
            "country": "Test Country",
            "contact_name": "John Doe",
            "phonenumber": f"001-{random.randint(100, 999)}-{random.randint(100, 999)}-{random.randint(1000, 9999)}x{random.randint(100, 9999)}",
            "reference": f"CL-{random.randint(1000, 9999)}",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z"
        }

    def test_get_suppliers(self):
        """Test retrieving all suppliers (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /suppliers - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_supplier_by_id(self):
        """Test retrieving a supplier by ID (happy path)."""
        # Add a new supplier
        response = requests.post(self.base_url, json=self.new_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        supplier_id = self.new_supplier["id"]

        # Retrieve the supplier by its ID
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /suppliers/{supplier_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    def test_add_supplier(self):
        """Test adding a new supplier (happy path)."""
        response = requests.post(self.base_url, json=self.new_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        supplier_id = self.new_supplier["id"]

        # Verify supplier was added
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /suppliers/{supplier_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    def test_update_supplier(self):
        """Test updating an existing supplier (happy path)."""
        # Add supplier to update
        response = requests.post(self.base_url, json=self.new_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        supplier_id = self.new_supplier["id"]

        # Update the supplier
        updated_supplier = self.new_supplier.copy()
        updated_supplier.update({
            "name": "Updated Supplier Ltd",
            "city": "Updated City",
            "phonenumber": "001-555-555-5555x9999"
        })
        put_response = requests.put(f"{self.base_url}/{supplier_id}", json=updated_supplier, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        # Verify update
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        supplier_data = get_response.json()
        self.assertEqual(supplier_data["name"], updated_supplier["name"])

    def test_delete_supplier(self):
        """Test deleting an existing supplier (happy path)."""
        # Add supplier to delete
        response = requests.post(self.base_url, json=self.new_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        supplier_id = self.new_supplier["id"]

        # Delete supplier
        delete_response = requests.delete(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Verify deletion
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    def test_get_supplier_with_invalid_api_key(self):
        """Test retrieving suppliers with invalid API key, should handle properly (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /suppliers with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_supplier_missing_fields(self):
        """Test adding supplier with missing fields, expecting a controlled response (unhappy path)."""
        incomplete_supplier = {"id": random.randint(1000, 9999), "name": "Incomplete Supplier"}
        response = requests.post(self.base_url, json=incomplete_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /suppliers with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_supplier_invalid_id(self):
        """Test updating a supplier with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_supplier = self.new_supplier.copy()
        updated_supplier["name"] = "Invalid ID Supplier"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /suppliers/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_supplier_invalid_id(self):
        """Test deleting a supplier with invalid ID, expecting controlled response (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /suppliers/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
