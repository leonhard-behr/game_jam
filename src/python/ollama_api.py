"""runs on server"""
from flask import Flask, request, jsonify
import requests

app = Flask(__name__)
OLLAMA_URL = "http://localhost:11434/api/generate"

@app.route('/ask', methods=['POST'])
def ask_model():
    data = request.json
    prompt = data.get('prompt')
    model = data.get('model', 'deepseek-coder:6.7b')

    payload = {
        "model": model,
        "prompt": prompt,
        "stream": False
    }

    response = requests.post(OLLAMA_URL, json=payload)
    return jsonify(response.json())

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
