import unittest
import requests
import random
from datetime import datetime

class TestCrossdockingAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/cross-docking'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_key'}

        # Get the current max ID
        response = requests.get(self.base_url, headers=self.headers)
        if response.status_code == 200:
            try:
                crossdockings = response.json()
                max_id = max([crossdocking["id"] for crossdocking in crossdockings], default=0)
            except requests.exceptions.JSONDecodeError:
                max_id = 0
        else:
            max_id = 0

        # Crossdocking data
        self.crossdocking = {
            "id": max_id + 1,
            "order_id": random.randint(1, 1000),
            "status": random.choice(["pending", "in_progress", "completed"]),
            "created_at": datetime.now().isoformat()
        }

    def test_match_items(self):
        """Test matching items between shipments and orders (happy path)."""
        response = requests.get(f"{self.base_url}/match", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /cross-docking/match - Status Code: {response.status_code}, Response: {response.text}")

    def test_receive_shipment(self):
        """Test receiving a shipment (happy path)."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/receive", json=shipment_id, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"POST /cross-docking/receive - Status Code: {response.status_code}, Response: {response.text}")

    def test_ship_shipment(self):
        """Test shipping a shipment (happy path)."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/ship", json=shipment_id, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"POST /cross-docking/ship - Status Code: {response.status_code}, Response: {response.text}")

    def test_receive_shipment_invalid_api_key(self):
        """Test receiving a shipment with invalid API key (unhappy path)."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/receive", json=shipment_id, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"POST /cross-docking/receive with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_ship_shipment_invalid_api_key(self):
        """Test shipping a shipment with invalid API key (unhappy path)."""
        shipment_id = 1
        response = requests.post(f"{self.base_url}/ship", json=shipment_id, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"POST /cross-docking/ship with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_match_items_invalid_api_key(self):
        """Test matching items with invalid API key (unhappy path)."""
        response = requests.get(f"{self.base_url}/match", headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /cross-docking/match with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

if __name__ == '__main__':
    unittest.main()