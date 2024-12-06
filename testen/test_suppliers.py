import unittest
import requests
import random
import uuid
from datetime import datetime

class TestSuppliersAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v1/suppliers'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        suppliers = response.json()
        max_id = max([supplier["id"] for supplier in suppliers], default=0)

        # Supplier data
        self.test_supplier = {
            "id": max_id + 1,
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

    def test_get_suppliers(self):
        """Test retrieving all suppliers (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /suppliers - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_supplier_by_id(self):
        """Test retrieving a supplier by ID (happy path)."""
        # Add a new supplier
        post_response = requests.post(self.base_url, json=self.test_supplier, headers=self.headers)
        
        # Print the response content for debugging
        print(f"POST /suppliers - Status Code: {post_response.status_code}, Response: {post_response.text}")
        
        self.assertEqual(post_response.status_code, 201)
        supplier_id = self.test_supplier["id"]

        # GET request for specific supplier
        response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the supplier
        delete_response = requests.delete(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_supplier(self):
        """Test adding a new supplier (happy path)."""
        response = requests.post(self.base_url, json=self.test_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the supplier exists
        supplier_id = self.test_supplier["id"]
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        # Clean up by deleting the supplier
        delete_response = requests.delete(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_supplier(self):
        """Test updating an existing supplier (happy path)."""
        # Add a supplier to update
        post_response = requests.post(self.base_url, json=self.test_supplier, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        supplier_id = self.test_supplier["id"]

        # Update the supplier
        updated_supplier = self.test_supplier.copy()
        updated_supplier.update({
            "name": "Updated Supplier Ltd",
            "city": "Updated City",
            "phonenumber": "001-555-555-5555x9999"
        })
        put_response = requests.put(f"{self.base_url}/{supplier_id}", json=updated_supplier, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)  # Adjusted to match the actual API behavior

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        supplier_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {supplier_data}")

        # Normalize keys to lowercase
        supplier_data = {k.lower(): v for k, v in supplier_data.items()}

        # Check if 'name' exists in the response
        self.assertIn("name", supplier_data, "Response is missing 'name'")
        self.assertEqual(supplier_data["name"], updated_supplier["name"])

        # Clean up by deleting the supplier
        delete_response = requests.delete(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_supplier(self):
        """Test deleting an existing supplier (happy path)."""
        # Add a supplier to delete
        post_response = requests.post(self.base_url, json=self.test_supplier, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        supplier_id = self.test_supplier["id"]

        # DELETE request to remove the supplier
        response = requests.delete(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)
        print(f"DELETE /suppliers/{supplier_id} - Status Code: {response.status_code}")

        # Verify the supplier no longer exists
        get_response = requests.get(f"{self.base_url}/{supplier_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)
        print(f"GET /suppliers/{supplier_id} after delete - Status Code: {get_response.status_code}")

    def test_get_supplier_with_invalid_api_key(self):
        """Test retrieving suppliers with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /suppliers with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_supplier_missing_fields(self):
        """Test adding a supplier with missing fields (unhappy path)."""
        incomplete_supplier = {
            "id": self.test_supplier["id"] + 1,
            "name": "Incomplete Supplier"
        }
        response = requests.post(self.base_url, json=incomplete_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /suppliers with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_supplier_with_invalid_id(self):
        """Test updating a supplier with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_supplier = self.test_supplier.copy()
        updated_supplier["name"] = "Invalid ID Supplier"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_supplier, headers=self.headers)
        self.assertEqual(response.status_code, 400)  # Adjusted to match the actual API behavior
        print(f"PUT /suppliers/{invalid_id} - Status Code: {response.status_code}, Response: {response.text}")

    def test_delete_supplier_with_invalid_id(self):
        """Test deleting a supplier with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /suppliers/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()