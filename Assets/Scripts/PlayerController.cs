using UnityEngine;
using System.Collections.Generic;

// Top-down 2D character controller using Unity's built-in Input system
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRadius = 2f;
    
    private Rigidbody2D rb;
    
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
