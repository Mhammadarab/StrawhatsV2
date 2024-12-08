import unittest
import requests
from datetime import datetime

class TestShipmentsAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/shipments'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        shipments = response.json()
        max_id = max([shipment["id"] for shipment in shipments], default=0)

        # Shipment data
        self.test_shipment = {
            "id": max_id + 1,
            "reference": f"SH{max_id + 1}",
            "request_date": datetime.now().isoformat().split('T')[0],
            "shipment_date": datetime.now().isoformat().split('T')[0],
            "shipment_type": "I",
            "shipment_status": "Pending",
            "notes": "This is a test shipment.",
            "carrier_code": "UPS",
            "carrier_description": "United Parcel Service",
            "service_code": "Express",
            "payment_type": "Manual",
            "transfer_mode": "Air",
            "total_package_count": 10,
            "total_package_weight": 150.5,
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z",
            "items": [
                {
                    "item_id": f"P000001",
                    "amount": 10,
                    "crossDockingStatus": None
                },
                {
                    "item_id": f"P000001",
                    "amount": 20,
                    "crossDockingStatus": None
                }
            ]
        }

    def test_get_shipments(self):
        """Test retrieving all shipments (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /shipments - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_shipment_by_id(self):
        """Test retrieving a shipment by ID (happy path)."""
        # Add a new shipment
        post_response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.test_shipment["id"]

        # GET request for specific shipment
        response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_add_shipment(self):
        """Test adding a new shipment (happy path)."""
        response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the shipment exists
        shipment_id = self.test_shipment["id"]
        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_update_shipment(self):
        """Test updating an existing shipment (happy path)."""
        # Add a shipment to update
        post_response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.test_shipment["id"]

        # Update the shipment
        updated_shipment = self.test_shipment.copy()
        updated_shipment.update({
            "shipment_status": "Completed",
            "total_package_count": 15
        })
        put_response = requests.put(f"{self.base_url}/{shipment_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        shipment_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {shipment_data}")

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_shipment(self):
        """Test deleting an existing shipment (happy path)."""
        # Add a shipment to delete
        post_response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.test_shipment["id"]

        # DELETE request to remove the shipment
        response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

        # Verify the shipment no longer exists
        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)

    def test_get_shipment_with_invalid_api_key(self):
        """Test retrieving shipments with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /shipments with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_shipment_missing_fields(self):
        """Test adding shipment with missing fields (unhappy path)."""
        incomplete_shipment = {
            "id": self.test_shipment["id"] + 1,
            "reference": f"SH{self.test_shipment['id'] + 1}"
        }
        response = requests.post(self.base_url, json=incomplete_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /shipments with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_shipment_invalid_id(self):
        """Test updating a shipment with invalid ID (unhappy path)."""
        invalid_id = 999999
        updated_shipment = self.test_shipment.copy()
        updated_shipment["shipment_status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"PUT /shipments/{invalid_id} - Status Code: {response.status_code}")

    def test_delete_shipment_invalid_id(self):
        """Test deleting a shipment with invalid ID (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /shipments/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()