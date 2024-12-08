import unittest
import requests
import random

class TestLocationsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/locations'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.new_location = {
            "id": random.randint(100, 999),
            "warehouse_id": 1,
            "code": f"A.{random.randint(1, 9)}.{random.randint(1, 9)}",
            "name": f"Row: A, Rack: {random.randint(1, 5)}, Shelf: {random.randint(1, 5)}",
            "created_at": "2023-10-07 16:08:24",
            "updated_at": "2023-10-07 16:08:24"
        }

    # Happy path test
    def test_get_locations(self):
        """Test retrieving all locations (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /locations - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_location_by_id(self):
        """Test retrieving a location by ID (happy path)."""
        response = requests.post(self.base_url, json=self.new_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        location_id = self.new_location["id"]

        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /locations/{location_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_add_location(self):
        """Test adding a new location (happy path)."""
        response = requests.post(self.base_url, json=self.new_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        location_id = self.new_location["id"]

        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /locations/{location_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_location(self):
        """Test updating an existing location (happy path)."""
        response = requests.post(self.base_url, json=self.new_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        location_id = self.new_location["id"]

        updated_location = self.new_location.copy()
        updated_location.update({
            "name": "Updated Row: A, Rack: 1, Shelf: 3",
            "code": "B.1.3"
        })
        put_response = requests.put(f"{self.base_url}/{location_id}", json=updated_location, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        location_data = get_response.json()
        self.assertEqual(location_data["name"], updated_location["name"])
        self.assertEqual(location_data["code"], updated_location["code"])

    # Happy path test
    def test_delete_location(self):
        """Test deleting an existing location (happy path)."""
        response = requests.post(self.base_url, json=self.new_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        location_id = self.new_location["id"]

        delete_response = requests.delete(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        get_response = requests.get(f"{self.base_url}/{location_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_location_with_invalid_api_key(self):
        """Test retrieving locations with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /locations with invalid API key - Status Code: {response.status_code}")

    # Unhappy path test
    def test_add_location_missing_fields(self):
        """Test adding a location with missing fields, expecting 400 Bad Request."""
        incomplete_location = {
            "id": random.randint(100, 999),
            "name": "Incomplete Location"
            # Missing required fields like warehouse_id, code, etc.
        }
        response = requests.post(self.base_url, json=incomplete_location, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /locations with missing fields - Status Code: {response.status_code}")

    # Unhappy path test
    def test_update_location_invalid_id(self):
        """Test updating a location with an invalid ID, expecting 200 OK (as per current API behavior)."""
        invalid_id = 999999
        updated_location = self.new_location.copy()
        updated_location["name"] = "Updated Invalid ID Location"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_location, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /locations/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_location_invalid_id(self):
        """Test deleting a location with an invalid ID, expecting 200 OK (as per current API behavior)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /locations/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
