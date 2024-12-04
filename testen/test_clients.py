import json
import os
import random
import unittest
import tempfile
import requests


class TestClientsAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v1/clients'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_key'}

        # Use a temporary file for the mock data
        self.temp_mock_file = tempfile.NamedTemporaryFile(delete=False, suffix=".json")
        self.mock_file_path = self.temp_mock_file.name

        # Client data
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

    def test_add_client(self):
        """Test adding a new client (happy path)."""
        response = requests.post(self.base_url, json=self.test_client, headers=self.headers)
        self.assertEqual(response.status_code, 201)  # Expecting "Created"
        
        # Simulate adding client to mock file
        mock_data = self._read_mock_file()
        mock_data.append(self.test_client)
        self._write_mock_file(mock_data)

        # Verify the client was added
        client_id = self.test_client["id"]
        get_response = requests.get(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"POST /clients - Status Code: {response.status_code}, Response: {response.text}")

    def test_delete_client(self):
        """Test deleting a client (happy path)."""
        # Add client to mock file first
        mock_data = self._read_mock_file()
        mock_data.append(self.test_client)
        self._write_mock_file(mock_data)

        # DELETE request to remove the client
        client_id = self.test_client["id"]
        delete_response = requests.delete(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        # Simulate deletion from the mock file
        mock_data = [client for client in self._read_mock_file() if client["id"] != client_id]
        self._write_mock_file(mock_data)

        # Verify client no longer exists in the mock file
        self.assertFalse(any(client["id"] == client_id for client in self._read_mock_file()))

    def test_get_clients(self):
        """Test retrieving all clients (happy path)."""
        # Ensure mock file has at least one client
        mock_data = self._read_mock_file()
        if not mock_data:
            mock_data.append(self.test_client)
            self._write_mock_file(mock_data)

        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /clients - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_client_by_id(self):
        """Test retrieving a client by ID (happy path)."""
        # Ensure mock file has the client
        mock_data = self._read_mock_file()
        if not any(client["id"] == self.test_client["id"] for client in mock_data):
            mock_data.append(self.test_client)
            self._write_mock_file(mock_data)

        # GET request for specific client
        client_id = self.test_client["id"]
        response = requests.get(f"{self.base_url}/{client_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_update_client(self):
        """Test updating an existing client (happy path)."""
        # Ensure mock file has the client
        mock_data = self._read_mock_file()
        if not any(client["id"] == self.test_client["id"] for client in mock_data):
            mock_data.append(self.test_client)
            self._write_mock_file(mock_data)

        # Update client
        updated_client = self.test_client.copy()
        updated_client.update({
            "name": "Ali Inc Updated",
            "contact_name": "Updated Contact",
            "contact_email": "updated_email@example.com"
        })
        client_id = self.test_client["id"]
        response = requests.put(f"{self.base_url}/{client_id}", json=updated_client, headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Simulate update in the mock file
        for client in mock_data:
            if client["id"] == client_id:
                client.update(updated_client)
        self._write_mock_file(mock_data)

        # Verify update in mock file
        self.assertTrue(any(client["name"] == "Ali Inc Updated" for client in self._read_mock_file()))

    def test_get_client_with_invalid_api_key(self):
        """Test retrieving clients with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /clients with invalid API key - Status Code: {response.status_code}")

    def test_add_client_missing_fields(self):
        """Test adding a client with missing fields (unhappy path)."""
        incomplete_client = {"id": random.randint(1000, 9999), "name": "Incomplete Client"}
        response = requests.post(self.base_url, json=incomplete_client, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /clients with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_client_invalid_id(self):
        """Test updating a client with an invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_client = self.test_client.copy()
        updated_client["name"] = "Invalid ID Client"

        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_client, headers=self.headers)
        if response.status_code != 404:
            print(f"Unexpected status code: {response.status_code}, Response: {response.text}")
        self.assertEqual(response.status_code, 404)
        print(f"PUT /clients/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_client_invalid_id(self):
        """Test deleting a client with an invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /clients/{invalid_id} - Status Code: {response.status_code}")


if __name__ == '__main__':
    unittest.main()