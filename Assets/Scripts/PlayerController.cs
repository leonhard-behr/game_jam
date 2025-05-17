using UnityEngine;
using System.Collections.Generic;
using TMPro; // Add this for TMP_InputField reference

// Top-down 2D character controller using Unity's built-in Input system
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRadius = 2f;
    
    private Rigidbody2D rb;
    private TMP_InputField[] inputFields; // Reference to all input fields
    
    private void Start()
    {
        // Get or add required components
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {            
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // No gravity for top-down view
            rb.linearDamping = 0.5f; // Add some damping for smoother movement
            rb.freezeRotation = true; // Keep the character upright
        }
        
        // Ensure we have a collider
        if (GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Make sure the player has the correct tag
        if (gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
            Debug.Log("Set player tag to 'Player'");
        }
        
        // Find all input fields in the scene
        inputFields = FindObjectsOfType<TMP_InputField>();
        Debug.Log($"Found {inputFields.Length} input fields in the scene");
    }
    
    private void Update()
    {
        // This is just for debugging purposes
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E to interact");
        }
    }
    
    private bool IsAnyInputFieldFocused()
    {
        if (inputFields == null || inputFields.Length == 0)
            return false;
            
        foreach (var inputField in inputFields)
        {
            if (inputField != null && inputField.isFocused)
            {
                return true;
            }
        }
        
        // Also check if EventSystem's current selected gameobject is an input field
        if (UnityEngine.EventSystems.EventSystem.current != null && 
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
        {
            var selectedInputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if (selectedInputField != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void FixedUpdate()
    {
        // Check if any input field is focused - if so, don't process movement
        if (IsAnyInputFieldFocused())
        {
            // Zero out velocity when input field is focused
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        // Handle top-down movement (both horizontal and vertical)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;
        rb.linearVelocity = movement;  // Changed from linearVelocity to velocity for compatibility
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions if needed
    }
}
