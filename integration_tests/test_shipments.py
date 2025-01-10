import requests
import unittest
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
            "Order_Id": [1, 2],  # List of order IDs
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
            "total_package_weight": 100.0,
            "created_at": datetime.now().isoformat(),
            "updated_at": datetime.now().isoformat(),
            "items": [
                {
                    "item_id": "P001",
                    "amount": 5,
                    "cross_docking_status": None
                },
                {
                    "item_id": "P002",
                    "amount": 5,
                    "cross_docking_status": None
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

        # Debugging: Print the response JSON
        print(f"GET /shipments/{shipment_id} - Response: {response.json()}")

        # Check if 'order_Id' exists in the response JSON
        self.assertIn("order_Id", response.json(), "order_Id key not found in the response")
        self.assertEqual(response.json()["order_Id"], self.test_shipment["Order_Id"])

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)

    def test_add_shipment(self):
        """Test adding a new shipment (happy path)."""
        response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /shipments - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_shipment(self):
        """Test updating a shipment (happy path)."""
        # Add a new shipment
        post_response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.test_shipment["id"]

        # Update the shipment
        updated_shipment = self.test_shipment.copy()
        updated_shipment["notes"] = "Updated test shipment."
        put_response = requests.put(f"{self.base_url}/{shipment_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)

        # Verify the update
        response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()["notes"], updated_shipment["notes"])

        # Clean up by deleting the shipment
        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

    def test_delete_shipment(self):
        """Test deleting a shipment (happy path)."""
        # Add a new shipment
        post_response = requests.post(self.base_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        shipment_id = self.test_shipment["id"]

        # Delete the shipment
        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)

        # Verify the deletion
        response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 404)

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