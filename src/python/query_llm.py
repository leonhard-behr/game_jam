"""runs on local device"""
import requests
import time
import json
import os
import hmac
import hashlib
import base64

TAILSCALE_IP = "100.124.135.38"  # home pc IP
CHAT_HISTORY_FILE = os.path.join(os.path.dirname(__file__), "chat_history.json")

# Load the API key that must match the server
API_KEY_PATH = os.path.join(os.path.dirname(__file__), "client_key.key")
if not os.path.exists(API_KEY_PATH):
    print("API key file not found. Please get the key from the server.")
    API_KEY = input("Enter API key: ")
    with open(API_KEY_PATH, 'w') as key_file:
        key_file.write(API_KEY)
else:
    with open(API_KEY_PATH, 'r') as key_file:
        API_KEY = key_file.read().strip()

SYS_PROMPT = """
You are in a roleplay scenario with the user.
You and the user are in a conversation. You both are in seperate cells. You do not see each other, you can only communicate through text.
The user is human, you will act as a human too. The user will ask you questions and will find things in his cell.

If the user asks you to do something, you will do it. You will give the user tips since you are longer in the cell than the user. 
"""

def init_chat_history():
    """Initialize chat history file if it doesn't exist"""
    if not os.path.exists(CHAT_HISTORY_FILE):
        chat_history = {
            "system_prompt": SYS_PROMPT,
            "history": []
        }
        save_chat_history(chat_history)
    return load_chat_history()

def load_chat_history():
    """Load chat history from JSON file"""
    if os.path.exists(CHAT_HISTORY_FILE):
        with open(CHAT_HISTORY_FILE, 'r') as f:
            return json.load(f)
    return init_chat_history()

def save_chat_history(chat_history):
    """Save chat history to JSON file"""
    with open(CHAT_HISTORY_FILE, 'w') as f:
        json.dump(chat_history, f, indent=2)

def generate_request_signature(request_data):
    """Generate HMAC signature for request"""
    # Create timestamp
    timestamp = str(int(time.time()))
    
    # Convert request data to bytes for hashing
    data = str(request_data).encode('utf-8')
    message = data + timestamp.encode('utf-8')
    
    # Create signature
    signature = hmac.new(
        API_KEY.encode('utf-8'), 
        message, 
        hashlib.sha256
    ).hexdigest()
    
    return timestamp, signature

def api_call(user_message):
    """Make API call with current chat history and update history with response"""
    # Load current chat history
    chat_history = load_chat_history()
    
    # Add user message to history
    chat_history["history"].append({"role": "user", "content": user_message})
    save_chat_history(chat_history)
    
    # Prepare prompt with context
    prompt = chat_history["system_prompt"]
    for msg in chat_history["history"]:
        if msg["role"] == "user":
            prompt += f"\nUser: {msg['content']}"
        else:
            prompt += f"\nAssistant: {msg['content']}"
    
    # Prepare request data
    request_data = {
        "system_prompt": chat_history["system_prompt"],
        "prompt": prompt,
        "model": "llama3:8b",
    }
    
    # Generate authentication signature
    timestamp, signature = generate_request_signature(request_data)
    
    # Make API call
    start = time.time()
    try:
        response = requests.post(
            f"http://{TAILSCALE_IP}:5000/ask", 
            json=request_data,
            headers={
                'X-Auth-Timestamp': timestamp,
                'X-Auth-Signature': signature
            }
        )
        
        if response.status_code != 200:
            print(f"Error: Server returned status code {response.status_code}")
            print(f"Response: {response.text}")
            return None
            
        response_text = response.json().get("response")
        if not response_text:
            print("Error: Received empty response from server")
            return None
        
        # Add assistant response to history
        chat_history["history"].append({"role": "assistant", "content": response_text})
        save_chat_history(chat_history)
        
        end = time.time()
        print("Time taken to get response:", end - start)
        
        return response_text
    except Exception as e:
        print(f"Error making API call: {e}")
        return None

# Initialize chat history
init_chat_history()

# Interactive chat mode
def chat_loop():
    print("Chat mode started. Type 'exit' to quit.")
    while True:
        user_input = input("\nYou: ")
        if user_input.lower() == "exit":
            break
        
        response_text = api_call(user_input)
        if response_text:
            print("\nAssistant:", response_text)

# Run interactive chat mode
chat_loop()