import json

# Load all_results.json
with open('all_results.json', 'r') as f:
    all_results = json.load(f)

# Load items_copy.json
with open('items_copy.json', 'r') as f:
    items_copy = json.load(f)

# Create a dictionary from all_results for quick lookup
results_dict = {item['id']: item['classifications'] for item in all_results}

# Update items_copy with classifications
for item in items_copy:
    item_id = item.get('uid')
    if item_id and item_id in results_dict:
        item['classifications'] = results_dict[item_id]

# Save the updated items_copy.json
with open('items_copy_updated.json', 'w') as f:
    json.dump(items_copy, f, indent=4)

print("Updated items_copy.json with classifications.")