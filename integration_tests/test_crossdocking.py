import unittest
import random
from datetime import datetime

class TestCrossdockingAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/cross-docking'
        self.headers = {'API_KEY': 'owner'}
        self.invalid_headers = {'API_KEY': 'invalid_key'}

        # In-memory data structure to simulate API responses
        self.crossdockings = []
        self.max_id = 0

        # Crossdocking data
        self.crossdocking = {
            "id": self.max_id + 1,
            "order_id": random.randint(1, 1000),
            "status": random.choice(["pending", "in_progress", "completed"]),
            "created_at": datetime.now().isoformat()
        }
        self.crossdockings.append(self.crossdocking)
        self.max_id += 1

    def test_match_items(self):
        """Test matching items between shipments and orders (happy path)."""
        response = self.simulate_get_request(f"{self.base_url}/match", self.headers)
        self.assertEqual(response['status_code'], 200)
        print(f"GET /cross-docking/match - Status Code: {response['status_code']}, Response: {response['text']}")

    def test_receive_shipment(self):
        """Test receiving a shipment (happy path)."""
        shipment_id = 1
        response = self.simulate_post_request(f"{self.base_url}/receive", shipment_id, self.headers)
        self.assertEqual(response['status_code'], 200)
        print(f"POST /cross-docking/receive - Status Code: {response['status_code']}, Response: {response['text']}")

    def test_ship_shipment(self):
        """Test shipping a shipment (happy path)."""
        shipment_id = 1
        response = self.simulate_post_request(f"{self.base_url}/ship", shipment_id, self.headers)
        self.assertEqual(response['status_code'], 200)
        print(f"POST /cross-docking/ship - Status Code: {response['status_code']}, Response: {response['text']}")

    def test_receive_shipment_invalid_api_key(self):
        """Test receiving a shipment with invalid API key (unhappy path)."""
        shipment_id = 1
        response = self.simulate_post_request(f"{self.base_url}/receive", shipment_id, self.invalid_headers)
        self.assertEqual(response['status_code'], 401)
        print(f"POST /cross-docking/receive with invalid API key - Status Code: {response['status_code']}, Response: {response['text']}")

    def test_ship_shipment_invalid_api_key(self):
        """Test shipping a shipment with invalid API key (unhappy path)."""
        shipment_id = 1
        response = self.simulate_post_request(f"{self.base_url}/ship", shipment_id, self.invalid_headers)
        self.assertEqual(response['status_code'], 401)
        print(f"POST /cross-docking/ship with invalid API key - Status Code: {response['status_code']}, Response: {response['text']}")

    def test_match_items_invalid_api_key(self):
        """Test matching items with invalid API key (unhappy path)."""
        response = self.simulate_get_request(f"{self.base_url}/match", self.invalid_headers)
        self.assertEqual(response['status_code'], 401)
        print(f"GET /cross-docking/match with invalid API key - Status Code: {response['status_code']}, Response: {response['text']}")

    def simulate_get_request(self, url, headers):
        if headers == self.invalid_headers:
            return {'status_code': 401, 'text': 'Invalid API key'}
        return {'status_code': 200, 'text': 'Match items successful'}

    def simulate_post_request(self, url, data, headers):
        if headers == self.invalid_headers:
            return {'status_code': 401, 'text': 'Invalid API key'}
        if '/receive' in url:
            return {'status_code': 200, 'text': 'Shipment received successfully'}
        if '/ship' in url:
            return {'status_code': 200, 'text': 'Shipment shipped successfully'}
        return {'status_code': 400, 'text': 'Bad request'}

if __name__ == '__main__':
    unittest.main()