using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using System.IO; // For file logging

public class llm_integration : MonoBehaviour
{
    // UI Components
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private TextMeshProUGUI responseText;
    
    [SerializeField] private string pythonApiUrl = "http://127.0.0.1:5001/query";
    
    private bool isProcessing = false;
    private string logFilePath;
    
    void Start()
    {
        // Setup logging
        string directory = Path.Combine(Application.persistentDataPath, "Logs");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        logFilePath = Path.Combine(directory, "llm_integration_log.txt");
        LogToFile("=== LLM Integration Started ===");
        
        // Log UI component status
        LogMessage($"InputField exists: {userInputField != null}");
        LogMessage($"SendButton exists: {sendButton != null}");
        LogMessage($"ResponseText exists: {responseText != null}");
        
        // Setup button click listener
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendMessage);
            LogMessage("Button click listener configured");
        }
        else
        {
            LogMessage("WARNING: Send button is null!", LogType.Warning);
        }
        
        // Setup input field to submit on Enter key
        if (userInputField != null)
        {
            userInputField.onEndEdit.AddListener(OnEndEdit);
            LogMessage("Input field end edit listener configured");
        }
        else
        {
            LogMessage("WARNING: User input field is null!", LogType.Warning);
        }
        
        // Check response text
        if (responseText == null)
        {
            LogMessage("WARNING: Response text field is null!", LogType.Warning);
        }
        
        LogMessage("LLM Integration initialized");
    }
    
    private void OnEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LogMessage("Enter key detected, sending message");
            SendMessage();
        }
    }
    
    public void SendMessage()
    {
        if (isProcessing)
        {
            LogMessage("Message processing already in progress, ignoring request");
            return;
        }
        
        if (string.IsNullOrEmpty(userInputField?.text))
        {
            LogMessage("Empty message, ignoring request");
            return;
        }
            
        string userMessage = userInputField.text;
        LogMessage($"Processing user message: {userMessage}");
        
        StartCoroutine(QueryLLM(userMessage));
        
        // Clear input field after sending
        userInputField.text = "";
    }
    
    private IEnumerator QueryLLM(string message)
    {
        isProcessing = true;
        LogMessage("API call started");
        
        // Create JSON payload
        string jsonPayload = JsonUtility.ToJson(new RequestData { message = message });
        LogMessage($"JSON payload created: {jsonPayload}");
        
        // Create web request
        using (UnityWebRequest request = new UnityWebRequest(pythonApiUrl, "POST"))
        {
            LogMessage($"Sending request to: {pythonApiUrl}");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            // Send the request
            LogMessage("Sending web request...");
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                LogMessage($"Received response: {responseJson}");
                
                try
                {
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);
                    LogMessage($"Parsed response: {responseData.response}");
                    
                    // Update UI text if available
                    if (responseText != null)
                    {
                        responseText.text = responseData.response;
                        LogMessage("Response text UI updated");
                        
                        // Force UI refresh
                        Canvas.ForceUpdateCanvases();
                    }
                    else
                    {
                        LogMessage("WARNING: ResponseText is null, can't update UI", LogType.Warning);
                    }
                }
                catch (System.Exception e)
                {
                    LogMessage($"ERROR parsing response JSON: {e.Message}", LogType.Error);
                }
            }
            else
            {
                LogMessage($"ERROR: Request failed with status {request.responseCode}", LogType.Error);
                LogMessage($"Error details: {request.error}", LogType.Error);
                LogMessage($"Response body: {request.downloadHandler.text}", LogType.Error);
            }
        }
        
        isProcessing = false;
        LogMessage("API call completed");
    }
    
    // Logging functions
    private void LogMessage(string message, LogType logType = LogType.Log)
    {
        // Log to console
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"[LLM] {message}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"[LLM] {message}");
                break;
            default:
                Debug.Log($"[LLM] {message}");
                break;
        }
        
        // Also log to file
        LogToFile($"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logType}] {message}");
    }
    
    private void LogToFile(string message)
    {
        try
        {
            File.AppendAllText(logFilePath, message + System.Environment.NewLine);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to write to log file: {e.Message}");
        }
    }
    
    // Classes for JSON serialization
    [System.Serializable]
    private class RequestData
    {
        public string message;
    }
    
    [System.Serializable]
    private class ResponseData
    {
        public string response;
    }
}