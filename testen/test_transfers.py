import unittest
import requests
import random
from datetime import datetime

class TestTransfersAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/transfers'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.new_transfer = {
            "id": random.randint(1000, 9999),
            "reference": f"TR{random.randint(10000, 99999)}",
            "transfer_from": None,
            "transfer_to": random.randint(1000, 9999),
            "transfer_status": "Pending",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z",
            "items": [
                {
                    "item_id": f"P{random.randint(1000, 9999)}",
                    "amount": random.randint(1, 50)
                }
            ]
        }

    def test_get_transfers(self):
        """Test retrieving all transfers (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /transfers - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_transfer_by_id(self):
        """Test retrieving a transfer by ID (happy path)."""
        # Add a new transfer
        response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # Retrieve the transfer by its ID
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /transfers/{transfer_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    def test_add_transfer(self):
        """Test adding a new transfer (happy path)."""
        response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # Verify transfer was added
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /transfers/{transfer_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    def test_update_transfer(self):
        """Test updating an existing transfer (happy path)."""
        # Add transfer to update
        response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # Update the transfer
        updated_transfer = self.new_transfer.copy()
        updated_transfer.update({
            "transfer_status": "Completed",
            "transfer_to": random.randint(1000, 9999)
        })
        put_response = requests.put(f"{self.base_url}/{transfer_id}", json=updated_transfer, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        # Verify update
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        transfer_data = get_response.json()
        self.assertEqual(transfer_data["transfer_status"], updated_transfer["transfer_status"])

    def test_delete_transfer(self):
        """Test deleting an existing transfer (happy path)."""
        # Add transfer to delete
        response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # Delete transfer
        delete_response = requests.delete(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Verify deletion
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    def test_get_transfer_with_invalid_api_key(self):
        """Test retrieving transfers with invalid API key, should handle properly (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /transfers with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_transfer_missing_fields(self):
        """Test adding transfer with missing fields, expecting a controlled response (unhappy path)."""
        incomplete_transfer = {
            "id": random.randint(1000, 9999),
            "reference": f"TR{random.randint(10000, 99999)}"
            # Missing required fields like transfer_to, transfer_status, items, etc.
        }
        response = requests.post(self.base_url, json=incomplete_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /transfers with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_transfer_invalid_id(self):
        """Test updating a transfer with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_transfer = self.new_transfer.copy()
        updated_transfer["transfer_status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /transfers/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_transfer_invalid_id(self):
        """Test deleting a transfer with invalid ID, expecting controlled response (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /transfers/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
