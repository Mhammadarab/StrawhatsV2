import unittest
import requests
import random
from datetime import datetime

class TestLocationsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/locations'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        locations = response.json()
        max_id = max([location["id"] for location in locations], default=0)

        self.test_location = {
            "id": max_id + 1,
            "location_id": 1,
            "code": f"A.{random.randint(1, 9)}.{random.randint(1, 9)}",
            "name": f"Row: A, Rack: {random.randint(1, 5)}, Shelf: {random.randint(1, 5)}"
        }

    def test_get_locations(self):
        """Test retrieving all locations (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /locations - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_location_by_id(self):
        """Test retrieving a location by ID (happy path)."""
        # Add a new location
        post_response = requests.post(self.base_url, json=self.test_location, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        location_id = self.test_location["id"]

        # GET request for specific location
        response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the location
        delete_response = requests.delete(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_location(self):
        """Test adding a new location (happy path)."""
        response = requests.post(self.base_url, json=self.test_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        location_id = self.test_location["id"]
        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        delete_response = requests.delete(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_location(self):
        """Test updating an existing location (happy path)."""
        response = requests.post(self.base_url, json=self.test_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        location_id = self.test_location["id"]

        updated_location = self.test_location.copy()
        updated_location.update({
            "name": "Updated Row: A, Rack: 1, Shelf: 3",
            "code": "B.1.3"
        })
        put_response = requests.put(f"{self.base_url}/{location_id}", json=updated_location, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        location_data = get_response.json()
        print(f"GET Response Data: {location_data}")

        delete_response = requests.delete(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_location(self):
        """Test deleting an existing location (happy path)."""
        response = requests.post(self.base_url, json=self.test_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        location_id = self.test_location["id"]

        delete_response = requests.delete(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_location_with_invalid_api_key(self):
        """Test retrieving locations with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /locations with invalid API key - Status Code: {response.status_code}")

    def test_add_location_missing_fields(self):
        # Test adding a location with missing fields (unhappy path).
        incomplete_location = {"id": self.test_location["id"] + 1, "name": "Incomplete Location"}
        response = requests.post(self.base_url, json=incomplete_location, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /locations with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_location_invalid_id(self):
        # Test updating a location with invalid ID (unhappy path).
        invalid_id = 999999
        updated_location = self.test_location.copy()
        updated_location["name"] = "Updated Invalid ID Location"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_location, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /locations/{invalid_id} - Status Code: {response.status_code}, Response: {response.text}")

    def test_delete_location_invalid_id(self):
        """Test deleting a location with an invalid ID, expecting 404 Not Found."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /locations/{invalid_id} - Status Code: {response.status_code}, Response: {response.text}")

if __name__ == '__main__':
    unittest.main()
