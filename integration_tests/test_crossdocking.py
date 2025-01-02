import unittest
import requests
from datetime import datetime

class TestCrossdockingAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/cross-docking'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_key'}

        # Get the current max timestamp
        response = requests.get(self.base_url, headers=self.headers)
        crossdocking = response.json()
        max_timestamp = max([crossdock["Timestamp"] for crossdock in crossdocking if "Timestamp" in crossdock], default="1970-01-01T00:00:00Z")

        # Crossdocking data
        self.test_crossdock = {
            "Timestamp": "2024-12-07T21:36:39.385861Z",
            "PerformedBy": "joe",
            "Operation": "ReceiveShipment",
            "Details": {
                "ShipmentId": 1,
                "Status": "Transit"
            }
        }

    def test_match_items(self):
        """Test matching items between shipments and orders."""
        response = requests.get(f"{self.base_url}/match", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /cross-docking/match - Status Code: {response.status_code}, Response: {response.text}")

    def test_receive_shipment(self):
        """Test marking a shipment as 'In Transit'."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/receive", json=shipment_id, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"POST /cross-docking/receive - Status Code: {response.status_code}, Response: {response.text}")

    def test_ship_shipment(self):
        """Test marking a shipment and its items as 'Shipped'."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/ship", json=shipment_id, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"POST /cross-docking/ship - Status Code: {response.status_code}, Response: {response.text}")

    def test_receive_shipment_with_invalid_api_key(self):
        """Test marking a shipment as 'In Transit' with an invalid API key."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/receive", json=shipment_id, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"POST /cross-docking/receive with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_ship_shipment_with_invalid_api_key(self):
        """Test marking a shipment and its items as 'Shipped' with an invalid API key."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/ship", json=shipment_id, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"POST /cross-docking/ship with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

if __name__ == '__main__':
    unittest.main()