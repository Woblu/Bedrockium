import requests
from bs4 import BeautifulSoup
import json

# The base directory of the archive
BASE_URL = "https://archive.org/download/mcbe-versions/"

def generate_versions_json():
    print(f"Connecting to {BASE_URL}...")
    response = requests.get(BASE_URL)
    
    if response.status_code != 200:
        print("Failed to reach the Internet Archive.")
        return

    soup = BeautifulSoup(response.text, 'html.parser')
    # Find all table links that look like version folders (e.g., 1.14.0.~/)
    links = soup.find_all('a')
    
    versions = []
    
    for link in links:
        folder_name = link.get('href', '')
        
        # Filter for folders that match the versioning pattern
        if folder_name.endswith('.~/'):
            clean_version = folder_name.replace('.~/', '')
            # The .7z file is usually named the same as the folder prefix
            file_name = f"{clean_version}.~.7z"
            download_url = f"{BASE_URL}{folder_name}{file_name}"
            
            versions.append({
                "Id": f"bedrock-{clean_version.replace('.', '-')}",
                "Name": f"Minecraft Bedrock {clean_version}",
                "DownloadUrl": download_url
            })
            print(f"Found: {clean_version}")

    # Save to versions.json
    with open('versions.json', 'w') as f:
        json.dump(versions, f, indent=2)
    
    print(f"\nSuccess! Generated versions.json with {len(versions)} entries.")

if __name__ == "__main__":
    generate_versions_json()