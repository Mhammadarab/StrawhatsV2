import json
from openai import OpenAI
import math
from pydantic import BaseModel
from typing import List,Literal
from tqdm import tqdm
import concurrent.futures

class Item(BaseModel):
    id: str
    classifications: List[Literal["hazardous", "non-hazardous"]]

class Items(BaseModel):
    items: List[Item]

client = OpenAI(organization="",
                api_key="")
#,item_lines.json,item_groups.json,item_types.json
# Load the JSON data from the file

def process_batch(batch, items_info, batch_size, client):
    items = items_info[batch * batch_size:(batch + 1) * batch_size]

    completion = client.beta.chat.completions.parse(
        model="gpt-4o-mini", 
        messages=[
            {
                'role': "system",
                "content": """You are a smart classification intelligence software.
                I will provide you with an array of items containing id, item_line, item_group, and item_type
                based on those details you will classify each item into 'hazardous' and 'non-hazardous'
                You will return a JSON object of the following format:
                {
                    "items": {
                        "id": <item_id>,
                        "classifications": [<classification>]
                    }
                }

                YOU MUST ALWAYS RETURN VALID JSON AS A RESPONSE AND ALWAYS MAKE A CLASSIFICATION """
            },
            {
                "role": "user",
                "content": json.dumps(items)
            }
        ],
        response_format=Items,
        temperature=0
    )

    # Extract the response
    response: Items = completion.choices[0].message.parsed

    # Return the result for further use (avoid modifying shared variables like 'results' directly)
    return [i.model_dump() for i in response.items]


def save_results(results, batch):
    with open(f"results_{batch}.json", "w") as out:
        json.dump(results, out)


def main(number_of_batches, batch_size, items_info, client, max_workers= 5):
    
    # Use ThreadPoolExecutor to process batches concurrently
    with concurrent.futures.ThreadPoolExecutor(max_workers=max_workers) as executor:
        # Create a list of future objects that will execute process_batch concurrently
        futures = [
            executor.submit(process_batch, batch, items_info, batch_size, client)
            for batch in range(number_of_batches)
        ]

        # Process the results as they are returned
        for batch, future in enumerate(concurrent.futures.as_completed(futures)):
            try:
                batch_results = future.result()  # This gets the results from process_batch

                # Save the results to a file after each batch
                save_results(batch_results, batch)

                print(f"processed batch {batch} with {len(batch_results)} items")
                
            except Exception as e:
                print(f"An error occurred in batch {batch}: {e}")

if __name__ == "__main__":
    results_batches = range(59)
    all_results = []
    for batch in results_batches:
        with open(f"results_{batch}.json", "r") as data_in:
            batch_results = json.load(data_in)
            all_results+=batch_results

    with open("all_results.json", "w") as out:
        json.dump(sorted(all_results, key=lambda r : r["id"]), out)
        

    # with open('items.json', 'r') as file:
    #     data = json.load(file)

    # with open('item_lines.json', 'r') as file:
    #     item_lines = json.load(file)

    # with open('item_groups.json', 'r') as file:
    #     item_groups = json.load(file)

    # with open('item_types.json', 'r') as file:
    #     item_types = json.load(file)

    # items_info = []


    # for item in data:
    #     uid = item['uid']
    #     item_line = item['item_line']
    #     item_group = item['item_group']
    #     item_type = item['item_type']

    #     item_info = {
    #         "id": uid
    #     }

    #     for il in item_lines:
    #         if il['id'] == item_line:
    #             name = il['name']
    #             item_info["item_line"] = name

    #     for ig in item_groups:
    #         if ig['id'] == item_group:
    #             name = ig['name']
    #             item_info["item_group"] = name

    #     for it in item_types:
    #         if it['id'] == item_type:
    #             name = it['name']
    #             item_info["item_type"] = name
        

    #     items_info.append(item_info)
        

    # batch_size = 200
    # number_of_batches = math.ceil(len(items_info)/batch_size)

    # main(number_of_batches, batch_size, items_info, client)


    
