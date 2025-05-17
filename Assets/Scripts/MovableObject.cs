using UnityEngine;

public class MovableObject : InteractiveObject
{
    [Header("Movable Object Settings")]
    [SerializeField] private float pickupDistance = 1.5f;
    [SerializeField] private float carryHeight = 0f; // Reduced to 0 for top-down games
    [SerializeField] private float carryDistance = 1.0f;
    [SerializeField] private float dropCooldown = 0.5f;
    [SerializeField] private LayerMask obstacleLayers; // Layers that block placement

    private bool isBeingCarried = false;
    private Rigidbody2D objectRigidbody;
    private Collider2D objectCollider;
    private Vector3 originalPosition;
    private Vector3 originalScale; // Store the original scale
    private bool canDropObject = true;
    private float lastDropTime;

    // Override the parent Start method to initialize additional components
    protected override void Start()
    {
        base.Start(); // Call the parent Start method first

        // Get or add a Rigidbody2D
        objectRigidbody = GetComponent<Rigidbody2D>();
        if (objectRigidbody == null)
        {
            objectRigidbody = gameObject.AddComponent<Rigidbody2D>();
            objectRigidbody.gravityScale = 0f;
            objectRigidbody.freezeRotation = true;
            objectRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
        
        // Store reference to collider
        objectCollider = GetComponent<Collider2D>();
        
        // Store original position and scale
        originalPosition = transform.position;
        originalScale = transform.localScale;
        
        // Set interaction message
        interactionPrompt = "Press E to pick up";
        
        // Set proper physics settings
        objectRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        objectRigidbody.linearDamping = 10f; // High drag to prevent sliding
        objectRigidbody.angularDamping = 10f;
    }

    protected override void Update()
    {
        // Skip the parent Update if we're carrying the object
        if (!isBeingCarried)
        {
            base.Update();
        }
        else
        {
            // Implement carrying behavior
            UpdateCarryPosition();
            
            // Check for drop input
            if (Input.GetKeyDown(KeyCode.E) && canDropObject && Time.time - lastDropTime > dropCooldown)
            {
                DropObject();
            }
        }
    }

    // Override the Interact method to implement pickup behavior
    protected override void Interact()
    {
        if (!isBeingCarried)
        {
            PickupObject();
        }
    }

    private void PickupObject()
    {
        // Only pickup if the player is close enough
        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= pickupDistance)
        {
            isBeingCarried = true;
            
            // Update physics
            objectRigidbody.isKinematic = true;
            objectRigidbody.linearVelocity = Vector2.zero; // Reset any velocity
            
            // Ensure scale is preserved
            transform.localScale = originalScale;
            
            if (objectCollider != null)
            {
                objectCollider.isTrigger = true;
            }
            
            // Hide prompt
            if (UIPromptController.Instance != null)
            {
                UIPromptController.Instance.HidePrompt();
                UIPromptController.Instance.ShowPrompt("Press E to place");
            }
            
            Debug.Log($"Player picked up {gameObject.name}");
        }
    }

    private void DropObject()
    {
        // Check if there's room to place the object
        Vector2 dropPosition = CalculateDropPosition();
        
        // Raycast to check if there's an obstacle at the drop position
        Collider2D obstacle = Physics2D.OverlapCircle(dropPosition, 0.5f, obstacleLayers);
        
        if (obstacle == null)
        {
            // Place the object
            transform.position = new Vector3(dropPosition.x, dropPosition.y, transform.position.z);
            isBeingCarried = false;
            
            // Update physics
            objectRigidbody.isKinematic = false;
            objectRigidbody.linearVelocity = Vector2.zero; // Important: Reset velocity to prevent sliding
            
            // Ensure scale is preserved
            transform.localScale = originalScale;
            
            if (objectCollider != null)
            {
                objectCollider.isTrigger = false;
            }
            
            // Update prompt
            if (UIPromptController.Instance != null)
            {
                UIPromptController.Instance.HidePrompt();
            }
            
            Debug.Log($"Player placed {gameObject.name}");
        }
        else
        {
            // Show message that there's no room
            if (UIPromptController.Instance != null)
            {
                UIPromptController.Instance.ShowPrompt("Can't place here - blocked by obstacle");
            }
            Debug.Log($"Can't place {gameObject.name} - blocked by {obstacle.name}");
        }
        
        lastDropTime = Time.time;
    }

    private void UpdateCarryPosition()
    {
        if (playerTransform != null)
        {
            // Calculate direction player is facing
            Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            Vector2 carryDirection = playerInput.magnitude > 0.1f ? playerInput : Vector2.down;
            
            // Set position in front of player
            Vector2 targetPosition = (Vector2)playerTransform.position + carryDirection * carryDistance;
            
            // Add height if needed (reduced/removed for top-down games)
            if (carryHeight > 0)
            {
                targetPosition += Vector2.up * carryHeight;
            }
            
            // Keep the original z position to avoid depth issues
            float zPos = transform.position.z;
            
            // Smoothly move object
            transform.position = Vector3.Lerp(
                transform.position, 
                new Vector3(targetPosition.x, targetPosition.y, zPos), 
                Time.deltaTime * 10f
            );
            
            // Ensure scale is preserved
            transform.localScale = originalScale;
        }
    }

    private Vector2 CalculateDropPosition()
    {
        if (playerTransform != null)
        {
            // Calculate direction player is facing
            Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            Vector2 dropDirection = playerInput.magnitude > 0.1f ? playerInput : Vector2.down;
            
            // Calculate position in front of player
            return (Vector2)playerTransform.position + dropDirection * carryDistance;
        }
        
        return transform.position;
    }
    
    // Reset the object to its original position (useful for debugging)
    public void ResetPosition()
    {
        transform.position = originalPosition;
        transform.localScale = originalScale;
        isBeingCarried = false;
        objectRigidbody.isKinematic = false;
        objectRigidbody.linearVelocity = Vector2.zero;
        
        if (objectCollider != null)
        {
            objectCollider.isTrigger = false;
        }
    }
}