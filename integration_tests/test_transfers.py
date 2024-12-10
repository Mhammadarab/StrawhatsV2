import unittest
import requests
from datetime import datetime

class TestTransfersAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/transfers'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        transfers = response.json()
        max_id = max([transfer["id"] for transfer in transfers], default=0)

        # Transfer data
        self.new_transfer = {
            "id": max_id + 1,
            "reference": f"TR{max_id + 1}",
            "transfer_from": None,
            "transfer_to": max_id + 2,
            "transfer_status": "Pending",
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z",
            "items": [
                {
                    "item_id": f"P{max_id + 1}",
                    "amount": 10
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
        post_response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # GET request for specific transfer
        response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the transfer
        delete_response = requests.delete(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_transfer(self):
        """Test adding a new transfer (happy path)."""
        response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the transfer exists
        transfer_id = self.new_transfer["id"]
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        # Clean up by deleting the transfer
        delete_response = requests.delete(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_transfer(self):
        """Test updating an existing transfer (happy path)."""
        # Add a transfer to update
        post_response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # Update the transfer
        updated_transfer = self.new_transfer.copy()
        updated_transfer.update({
            "transfer_status": "Completed",
            "transfer_to": transfer_id + 1
        })
        put_response = requests.put(f"{self.base_url}/{transfer_id}", json=updated_transfer, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        transfer_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {transfer_data}")

        # Clean up by deleting the transfer
        delete_response = requests.delete(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_transfer(self):
        """Test deleting an existing transfer (happy path)."""
        # Add a transfer to delete
        post_response = requests.post(self.base_url, json=self.new_transfer, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        transfer_id = self.new_transfer["id"]

        # DELETE request to remove the transfer
        response = requests.delete(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the transfer no longer exists
        get_response = requests.get(f"{self.base_url}/{transfer_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_transfer_with_invalid_api_key(self):
        """Test retrieving transfers with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /transfers with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_transfer_missing_fields(self):
        """Test adding transfer with missing fields (unhappy path)."""
        incomplete_transfer = {
            "id": self.new_transfer["id"] + 1,
            "reference": f"TR{self.new_transfer['id'] + 1}"
        }
        response = requests.post(self.base_url, json=incomplete_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /transfers with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_transfer_invalid_id(self):
        """Test updating a transfer with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_transfer = self.new_transfer.copy()
        updated_transfer["transfer_status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_transfer, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /transfers/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_transfer_invalid_id(self):
        """Test deleting a transfer with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /transfers/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()