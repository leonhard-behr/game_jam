using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for LINQ methods like Any

// Top-down 2D character controller using Unity's built-in Input system
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRadius = 2f;
    
    [Header("Collider Settings")]
    [SerializeField] private float feetColliderHeight = 0.2f; // Height of feet collider
    [SerializeField] private float feetColliderWidth = 0.5f;  // Width of feet collider
    [SerializeField] private float feetColliderYOffset = -0.4f; // Y-offset from center
    
    private Rigidbody2D rb;
    private BoxCollider2D feetCollider;
    
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
        
        // Get or create feet collider
        feetCollider = GetComponent<BoxCollider2D>();
        if (feetCollider == null)
        {
            feetCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Adjust collider to only cover feet area
        AdjustFeetCollider();
        
        // Make sure the player has the correct tag
        if (gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
            Debug.Log("Set player tag to 'Player'");
        }
    }
    
    // Adjust the collider to only include the feet
    private void AdjustFeetCollider()
    {
        if (feetCollider != null)
        {
            // Set size for feet-only collision
            feetCollider.size = new Vector2(feetColliderWidth, feetColliderHeight);
            
            // Offset collider to be at the feet position
            feetCollider.offset = new Vector2(0, feetColliderYOffset);
            
            Debug.Log($"Adjusted player collider to feet-only: Size={feetCollider.size}, Offset={feetCollider.offset}");
        }
    }
    
    // You can use this to visualize the collider in the editor
    private void OnDrawGizmosSelected()
    {
        // Draw a wire cube representing the feet collider
        Gizmos.color = Color.green;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.offset, col.size);
            Gizmos.matrix = oldMatrix;
        }
        else
        {
            // If no collider exists yet, use the configured values
            Vector2 size = new Vector2(feetColliderWidth, feetColliderHeight);
            Vector3 center = new Vector3(0, feetColliderYOffset, 0);
            Gizmos.DrawWireCube(transform.position + center, size);
        }
    }
    
    private void Update()
    {
        // Movement is handled in FixedUpdate
        
        // The InteractiveObject class handles the interaction already
        // We don't need to implement additional logic here since
        // each InteractiveObject checks for key press in its own Update method
        
        // This is just for debugging purposes
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E to interact");
        }
    }
    
    private void FixedUpdate()
    {
        // Get all input fields in the scene
        bool isAnyInputFieldFocused = FindObjectsByType<TMPro.TMP_InputField>(FindObjectsSortMode.None)
            .Any(field => field.isFocused);
        
        // Skip movement if input field is focused
        if (isAnyInputFieldFocused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        // Handle top-down movement (both horizontal and vertical)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;
        rb.linearVelocity = movement;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions if needed
    }
}
