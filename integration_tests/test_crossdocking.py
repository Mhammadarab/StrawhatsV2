import unittest
import requests
import random
from datetime import datetime

class TestCrossdockingAPI(unittest.TestCase):
    """Test the Crossdocking API."""

    def setUp(self):
        self.base_url = "http://localhost:3000/api/v2/cross-docking"
        self.shipments_url = "http://localhost:3000/api/v2/shipments"
        self.headers = {"API_KEY": "a1b2c3d4e5"}
        self.invalid_headers = {"API_KEY": "invalid_key"}

        # Get the current max ID
        response = requests.get(self.shipments_url, headers=self.headers)
        shipments = response.json()
        max_id = max([shipment["id"] for shipment in shipments], default=0)

        # Shipment data
        self.test_shipment = {
            "Id": max_id + 1,
            "Order_Id": 1,
            "Source_Id": 33,
            "Order_Date": "2000-03-09T00:00:00",
            "Request_Date": "2000-03-11T00:00:00",
            "Shipment_Date": "2000-03-13T00:00:00",
            "Shipment_Type": "I",
            "Shipment_Status": "Pending",
            "Notes": "Zee vertrouwen klas rots heet lachen oneven begrijpen.",
            "Carrier_Code": "DPD",
            "Carrier_Description": "Dynamic Parcel Distribution",
            "Service_Code": "Fastest",
            "Payment_Type": "Manual",
            "Transfer_Mode": "Ground",
            "Total_Package_Count": 31,
            "Total_Package_Weight": 594.42,
            "Created_At": "2000-03-10T11:11:14Z",
            "Updated_At": "2000-03-11T13:11:14Z",
            "Items": [
                {"Item_Id": "P007435", "Amount": 23, "CrossDockingStatus": None},
                {"Item_Id": "P009557", "Amount": 1, "CrossDockingStatus": None}
            ]
        }

        # Post the new shipment
        response = requests.post(self.shipments_url, json=self.test_shipment, headers=self.headers)
        self.assertEqual(response.status_code, 201)
        self.shipment_id = response.json()["id"]

    def tearDown(self):
        # Clean up by deleting the created shipment
        response = requests.delete(f"{self.shipments_url}/{self.shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 204)

    def test_match_items(self):
        """Test matching items between shipments and orders (happy path)."""
        response = requests.get(f"{self.base_url}/match", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"GET /cross-docking/match - Status Code: {response.status_code}, Response: {response.text}")

    def test_receive_shipment(self):
        """Test receiving a shipment (happy path)."""
        response = requests.post(f"{self.base_url}/receive", json={"shipmentId": self.shipment_id}, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"POST /cross-docking/receive - Status Code: {response.status_code}, Response: {response.text}")

    def test_ship_shipment(self):
        """Test shipping a shipment (happy path)."""
        response = requests.post(f"{self.base_url}/ship", json={"shipmentId": self.shipment_id}, headers=self.headers)
        self.assertEqual(response.status_code, 200)
        print(f"POST /cross-docking/ship - Status Code: {response.status_code}, Response: {response.text}")

    def test_receive_shipment_invalid_api_key(self):
        """Test receiving a shipment with invalid API key (unhappy path)."""
        response = requests.post(f"{self.base_url}/receive", json={"shipmentId": self.shipment_id}, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"POST /cross-docking/receive with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_ship_shipment_invalid_api_key(self):
        """Test shipping a shipment with invalid API key (unhappy path)."""
        response = requests.post(f"{self.base_url}/ship", json={"shipmentId": self.shipment_id}, headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"POST /cross-docking/ship with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

    def test_match_items_invalid_api_key(self):
        """Test matching items with invalid API key (unhappy path)."""
        response = requests.get(f"{self.base_url}/match", headers=self.invalid_headers)
        self.assertEqual(response.status_code, 401)
        print(f"GET /cross-docking/match with invalid API key - Status Code: {response.status_code}, Response: {response.text}")

if __name__ == '__main__':
    unittest.main()