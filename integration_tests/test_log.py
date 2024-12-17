import unittest
import requests
from datetime import datetime

class TestLogAPI(unittest.TestCase):

    def setUp(self):
        # Set up the base URL and headers
        self.base_url = 'http://localhost:3000/api/v2/logs'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_api_key'}

    def test_get_logs(self):
        """Test retrieving all logs (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        logs = response.json()
        self.assertIsInstance(logs, list)

    def test_get_logs_by_date(self):
        """Test retrieving logs by date (happy path)."""
        date = "09-12-2024"
        response = requests.get(f"{self.base_url}/date/{date}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        logs = response.json()
        self.assertIsInstance(logs, list)
        expected_date = datetime.strptime(date, "%d-%m-%Y").strftime("%Y-%m-%d")
        for log in logs:
            self.assertIn(expected_date, log['timestamp'])

    def test_get_logs_by_apikey(self):
        """Test retrieving logs by API key (happy path)."""
        api_key = "admin12345"
        response = requests.get(f"{self.base_url}/apikey/{api_key}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        logs = response.json()
        self.assertIsInstance(logs, list)
        for log in logs:
            self.assertEqual(log['apiKey'], api_key)

    def test_get_logs_by_performedby(self):
        """Test retrieving logs by performed by (happy path)."""
        performed_by = "a1b2c3d4e5"
        response = requests.get(f"{self.base_url}/performedby/{performed_by}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        logs = response.json()
        self.assertIsInstance(logs, list)
        for log in logs:
            self.assertEqual(log['performedBy'], performed_by)

    def test_get_logs_with_invalid_api_key(self):
        """Test retrieving logs with an invalid API key, expecting 401 Unauthorized."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)

if __name__ == '__main__':
    unittest.main()