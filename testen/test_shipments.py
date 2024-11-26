import unittest
import requests
import random
from datetime import datetime

class TestShipmentsAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v1/shipments'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}
        self.new_shipment = {
            "id": random.randint(1000, 9999),
            "order_id": random.randint(1, 100),
            "source_id": random.randint(10, 99),
            "order_date": datetime.now().isoformat().split('T')[0],
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
            "total_package_count": random.randint(1, 50),
            "total_package_weight": round(random.uniform(100, 1000), 2),
            "created_at": datetime.now().isoformat() + "Z",
            "updated_at": datetime.now().isoformat() + "Z",
            "items": [
                {
                    "item_id": f"P{random.randint(1000, 9999)}",
                    "amount": random.randint(1, 50)
                },
                {
                    "item_id": f"P{random.randint(1000, 9999)}",
                    "amount": random.randint(1, 50)
                }
            ]
        }

    # Happy path test
    def test_get_shipments(self):
        """Test retrieving all shipments (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /shipments - Status Code: {response.status_code}, Response: {response.text}")

    # Happy path test
    def test_get_shipment_by_id(self):
        """Test retrieving a shipment by ID (happy path)."""
        response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        shipment_id = self.new_shipment["id"]

        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /shipments/{shipment_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_add_shipment(self):
        """Test adding a new shipment (happy path)."""
        response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        shipment_id = self.new_shipment["id"]

        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        print(f"GET /shipments/{shipment_id} - Status Code: {get_response.status_code}, Response: {get_response.text}")

    # Happy path test
    def test_update_shipment(self):
        """Test updating an existing shipment (happy path)."""
        response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        shipment_id = self.new_shipment["id"]

        updated_shipment = self.new_shipment.copy()
        updated_shipment.update({
            "shipment_status": "Shipped",
            "notes": "Updated shipment notes.",
            "carrier_code": "FedEx",
            "carrier_description": "Federal Express"
        })
        put_response = requests.put(f"{self.base_url}/{shipment_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(put_response.status_code, 200)

        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        shipment_data = get_response.json()
        self.assertEqual(shipment_data["shipment_status"], updated_shipment["shipment_status"])

    # Happy path test
    def test_delete_shipment(self):
        """Test deleting an existing shipment (happy path)."""
        response = requests.post(self.base_url, json=self.new_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        shipment_id = self.new_shipment["id"]

        delete_response = requests.delete(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 200)

        get_response = requests.get(f"{self.base_url}/{shipment_id}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(get_response.text.strip(), "null")

    # Unhappy path test
    def test_get_shipment_with_invalid_api_key(self):
        """Test retrieving shipments with invalid API key, should handle properly (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /shipments with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    # Unhappy path test
    def test_add_shipment_missing_fields(self):
        """Test adding shipment with missing fields, expecting a controlled response (unhappy path)."""
        incomplete_shipment = {
            "id": random.randint(1000, 9999),
            "order_id": random.randint(1, 100)
            # Missing other required fields like shipment_status, items, etc.
        }
        response = requests.post(self.base_url, json=incomplete_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        print(f"POST /shipments with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    # Unhappy path test
    def test_update_shipment_invalid_id(self):
        """Test updating a shipment with invalid ID, should respond correctly (unhappy path)."""
        invalid_id = 999999
        updated_shipment = self.new_shipment.copy()
        updated_shipment["shipment_status"] = "Invalid ID Update"
        response = requests.put(f"{self.base_url}/{invalid_id}", json=updated_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"PUT /shipments/{invalid_id} - Status Code: {response.status_code}")

    # Unhappy path test
    def test_delete_shipment_invalid_id(self):
        """Test deleting a shipment with invalid ID, expecting controlled response (unhappy path)."""
        invalid_id = 999999
        response = requests.delete(f"{self.base_url}/{invalid_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"DELETE /shipments/{invalid_id} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()
