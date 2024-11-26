import unittest
import requests
import random

class TestClientsAPI(unittest.TestCase):

    def setUp(self):
        # Base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/clients'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_key'}
        self.test_client = {
            "id": random.randint(1000, 9999),
            "name": "Ali Inc",
            "address": "1296 Daniel Road Apt. 349",
            "city": "Pierceview",
            "zip_code": "28301",
            "province": "Colorado",
            "country": "United States",
            "contact_name": "Bryan Clark",
            "contact_phone": "242.732.3483x2573",
            "contact_email": "robertcharles@example.net"
        }

    # Happy path test
    def test_get_clients(self):
        """Test retrieving all clients (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /clients - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_add_client(self):
        """Test adding a new client (happy path)."""
        response = requests.post(self.base_url, json=self.test_client, headers=self.headers)
        self.assertEqual(response.status_code, 201)  # Expecting "Created"
        client_id = self.test_client["id"]

        # Verify the client was added
        get_response = requests.get(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"POST /clients - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_client_by_id(self):
        """Test retrieving a client by ID (happy path)."""
        # Add client to retrieve
        response = requests.post(self.base_url, json=self.test_client, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        client_id = self.test_client["id"]

        # Retrieve by ID
        get_response = requests.get(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /clients/{client_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_client(self):
        """Test updating an existing client (happy path)."""
        # Add client to update
        response = requests.post(self.base_url, json=self.test_client, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        client_id = self.test_client["id"]

        # Update client
        updated_client = self.test_client.copy()
        updated_client.update({
            "name": "Ali Inc Updated",
            "contact_name": "Updated Contact",
            "contact_email": "updated_email@example.com"
        })
        put_response = requests.put(f"{self.base_url}/{client_id}", json=updated_client, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{client_id}", headers=self.headers)
        client_data = get_response.json()
        self.assertEqual(client_data["name"], updated_client["name"])
        self.assertEqual(client_data["contact_name"], updated_client["contact_name"])

    # Happy path test
    def test_delete_client(self):
        """Test deleting a client (happy path)."""
        response = requests.post(self.base_url, json=self.test_client, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        client_id = self.test_client["id"]

        # Delete client
        delete_response = requests.delete(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Verify deletion
        get_response = requests.get(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_client_with_invalid_api_key(self):
        """Test retrieving clients with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /clients with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_add_client_missing_fields(self):
        """Test adding a client with missing fields (unhappy path)."""
        incomplete_client = {"id": random.randint(1000, 9999), "name": "Incomplete Client"}
        response = requests.post(self.base_url, json=incomplete_client, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /clients with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    # Unhappy path test
    def test_update_client_invalid_id(self):
        """Test updating a client with an invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_client = self.test_client.copy()
        updated_client["name"] = "Invalid ID Client"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_client, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /clients/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_client_invalid_id(self):
        """Test deleting a client with an invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /clients/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
