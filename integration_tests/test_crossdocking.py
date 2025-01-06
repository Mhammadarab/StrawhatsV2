import unittest
import requests
import random
from datetime import datetime

class TestCrossdockingAPI(unittest.TestCase):
    """Test the Crossdocking API."""

    def setUp(self):
        self.base_url = "http://localhost:5000/cross-docking"
        self.headers = {"API_Key ": "a1b2c3d4e5"}
        self.invalid_headers = {"API_Key": "invalid_key"}

        # Get the current max timestamp
        response = requests.get(self.base_url, headers=self.headers)
        crossdocking = response.json()
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

        