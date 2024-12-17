import unittest
import requests
from datetime import datetime

class TestStockLogAPI(unittest.TestCase):

    def setUp(self):
        self.base_url = 'http://localhost:3000/api/v2/stocklogs'
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.invalid_headers = {'API_KEY': 'invalid_key'}

        # Get the current max timestamp
        response = requests.get(self.base_url, headers=self.headers)
        audits = response.json()
        max_timestamp = max([audit["Timestamp"] for audit in audits if "Timestamp" in audit], default="1970-01-01T00:00:00Z")

        # Inventory Audit data
        self.test_stocklog = {
            "Timestamp": (datetime.now().isoformat() + "Z"),
            "PerformedBy": "a1b2c3d4e5",
            "Status": "Live",
            "AuditData": {
                "1": {
                    "3211": 999,
                    "24700": 999
                },
                "2": {
                    "14123": 999
                }
            },
            "Discrepancies": [
                "Location 14123 not found for Inventory ID 2."
            ]
        }

    def test_get_inventory_audits(self):
        """Test retrieving all inventory audits (happy path)."""
        response = requests.get(self.base_url, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /inventory_audit - Status Code: {response.status_code}, Response: {response.text}")

    def test_get_inventory_audit_by_timestamp(self):
        """Test retrieving an inventory audit by timestamp (happy path)."""
        # Add a new inventory audit
        post_response = requests.post(self.base_url, json=self.test_stocklog, headers=self.headers)
        
        # Print the response content for debugging
        print(f"POST /inventory_audit - Status Code: {post_response.status_code}, Response: {post_response.text}")
        
        self.assertEqual(post_response.status_code, 201)
        timestamp = self.test_stocklog["Timestamp"]

        # GET request for specific inventory audit
        response = requests.get(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(response.status_code, 200)

        # Clean up by deleting the inventory audit
        delete_response = requests.delete(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)  # Expecting 204 No Content

    def test_add_inventory_audit(self):
        """Test adding a new inventory audit (happy path)."""
        response = requests.post(self.base_url, json=self.test_stocklog, headers=self.headers)
        self.assertEqual(response.status_code, 201)

        # Verify the inventory audit exists
        timestamp = self.test_stocklog["Timestamp"]
        get_response = requests.get(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)

        # Clean up by deleting the inventory audit
        delete_response = requests.delete(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)  # Expecting 204 No Content

    def test_update_inventory_audit(self):
        """Test updating an existing inventory audit (happy path)."""
        # Add an inventory audit to update
        post_response = requests.post(self.base_url, json=self.test_stocklog, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        timestamp = self.test_stocklog["Timestamp"]

        # Update the inventory audit
        updated_audit = self.test_stocklog.copy()
        updated_audit.update({
            "Status": "Completed",
            "Discrepancies": ["Updated discrepancy"]
        })
        put_response = requests.put(f"{self.base_url}/{timestamp}", json=updated_audit, headers=self.headers)
        self.assertEqual(put_response.status_code, 204)  # Expecting 204 No Content

        # Verify the update
        get_response = requests.get(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(get_response.status_code, 200)
        audit_data = get_response.json()

        # Debugging step to print the response data
        print(f"GET Response Data: {audit_data}")

        # Normalize keys to lowercase
        audit_data = {k.lower(): v for k, v in audit_data.items()}

        # Check if 'status' exists in the response
        self.assertIn("status", audit_data, "Response is missing 'status'")
        self.assertEqual(audit_data["status"], updated_audit["Status"])

        # Clean up by deleting the inventory audit
        delete_response = requests.delete(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(delete_response.status_code, 204)  # Expecting 204 No Content

    def test_delete_inventory_audit(self):
        """Test deleting an existing inventory audit (happy path)."""
        # Add an inventory audit to delete
        post_response = requests.post(self.base_url, json=self.test_stocklog, headers=self.headers)
        self.assertEqual(post_response.status_code, 201)
        timestamp = self.test_stocklog["Timestamp"]

        # DELETE request to remove the inventory audit
        response = requests.delete(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(response.status_code, 204)  # Expecting 204 No Content
        print(f"DELETE /inventory_audit/{timestamp} - Status Code: {response.status_code}")

        # Verify the inventory audit no longer exists
        get_response = requests.get(f"{self.base_url}/{timestamp}", headers=self.headers)
        self.assertEqual(get_response.status_code, 404)
        print(f"GET /inventory_audit/{timestamp} after delete - Status Code: {get_response.status_code}")

    def test_get_inventory_audit_with_invalid_api_key(self):
        """Test retrieving inventory audits with invalid API key (unhappy path)."""
        response = requests.get(self.base_url, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /inventory_audit with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_add_inventory_audit_missing_fields(self):
        """Test adding an inventory audit with missing fields (unhappy path)."""
        incomplete_audit = {
            "timestamp": (datetime.now().isoformat() + "Z")
        }
        response = requests.post(self.base_url, json=incomplete_audit, headers=self.headers)
        self.assertEqual(response.status_code, 400)
        print(f"POST /inventory_audit with missing fields - Status Code: {response.status_code}, Response: {response.text}")

    def test_update_inventory_audit_invalid_timestamp(self):
        """Test updating an inventory audit with an invalid timestamp (unhappy path)."""
        invalid_timestamp = "9999-12-31T23:59:59Z"
        updated_audit = self.test_stocklog.copy()
        updated_audit["Status"] = "Invalid Timestamp Audit"
        response = requests.put(f"{self.base_url}/{invalid_timestamp}", json=updated_audit, headers=self.headers)
        
        # Print the response content for debugging
        print(f"PUT /inventory_audit/{invalid_timestamp} - Status Code: {response.status_code}, Response: {response.text}")
        
        self.assertEqual(response.status_code, 400, f"Expected status code 400, got {response.status_code}. Response: {response.text}")

    def test_delete_inventory_audit_invalid_timestamp(self):
        """Test deleting an inventory audit with an invalid timestamp (unhappy path)."""
        invalid_timestamp = "9999-12-31T23:59:59Z"
        response = requests.delete(f"{self.base_url}/{invalid_timestamp}", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        print(f"DELETE /inventory_audit/{invalid_timestamp} - Status Code: {response.status_code}")

if __name__ == '__main__':
    unittest.main()