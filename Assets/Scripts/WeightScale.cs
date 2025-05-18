using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A script that detects objects placed on a platform and displays their weight value.
/// The weight is determined by the last character of the object's name (e.g., "Weight3" has weight 3).
/// </summary>
public class WeightScale : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private Text weightDisplay; // UI Text component to display the weight
    [SerializeField] private string defaultText = "0"; // Text to show when nothing is on the scale
    
    // Reference to the object platform this script is attached to
    private ObjectPlatform platform;
    
    private void Awake()
    {
        // Get the ObjectPlatform component
        platform = GetComponent<ObjectPlatform>();
        if (platform == null)
        {
            Debug.LogError("WeightScale script requires an ObjectPlatform component on the same GameObject.");
            enabled = false;
            return;
        }
        
        // Initialize display
        UpdateWeightDisplay();
    }
    
    private void Update()
    {
        UpdateWeightDisplay();
    }
    
    /// <summary>
    /// Updates the weight display based on the current object on the platform
    /// </summary>
    private void UpdateWeightDisplay()
    {
        if (weightDisplay == null)
            return;
            
        // Get the current object on the platform
        CarriableObject obj = platform.GetPlacedObject();
        
        if (obj == null)
        {
            // No object on the scale
            weightDisplay.text = defaultText;
            return;
        }
        
        // Get the name of the object
        string objName = obj.name;
        
        // Extract the last character
        if (objName.Length > 0)
        {
            string lastChar = objName[objName.Length - 1].ToString();
            
            // Check if the last character is a number
            int weight;
            if (int.TryParse(lastChar, out weight))
            {
                // Show the weight value
                weightDisplay.text = weight.ToString();
            }
            else
            {
                // Last character isn't a number, show 0 or some default
                weightDisplay.text = defaultText;
            }
        }
        else
        {
            // Empty name, show default
            weightDisplay.text = defaultText;
        }
    }
    
    /// <summary>
    /// Get the current weight value (useful for other scripts)
    /// </summary>
    /// <returns>The current weight value or 0 if nothing on the scale</returns>
    public int GetCurrentWeight()
    {
        CarriableObject obj = platform.GetPlacedObject();
        
        if (obj == null)
            return 0;
            
        string objName = obj.name;
        
        if (objName.Length > 0)
        {
            string lastChar = objName[objName.Length - 1].ToString();
            
            int weight;
            if (int.TryParse(lastChar, out weight))
            {
                return weight;
            }
        }
        
        return 0;
    }
}
