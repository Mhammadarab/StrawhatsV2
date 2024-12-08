import json

# Filepath to the JSON file
filepath = r'C:\Users\moham\StrawhatsV2\data\item_types.json'

# Load the JSON data
with open(filepath, 'r') as file:
    data = json.load(file)

# Increment the id of each item by 1
for item in data:
    item['id'] += 1

# Save the updated JSON data back to the file
with open(filepath, 'w') as file:
    json.dump(data, file, indent=2)

print("IDs have been incremented by 1.")