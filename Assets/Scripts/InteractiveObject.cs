using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] protected float interactionRadius = 2f;
    [SerializeField] protected string interactionPrompt = "Press E to interact";
    [SerializeField] protected Color highlightColor = new Color(1f, 0.92f, 0.016f, 1f); // Yellow highlight
    
    protected Color originalColor;
    protected Renderer objectRenderer;
    protected bool playerInRange = false;
    
    protected virtual void Start()
    {
        // Get the renderer component
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("No Renderer found on " + gameObject.name);
        }
        
        // Make sure there's a collider for detection
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("Added BoxCollider2D to " + gameObject.name);
        }
    }
    
    protected virtual void Update()
    {
        // Check if player is in range
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            bool wasInRange = playerInRange;
            playerInRange = distanceToPlayer <= interactionRadius;
            
            // Highlight object if player is in range
            if (objectRenderer != null)
            {
                objectRenderer.material.color = playerInRange ? highlightColor : originalColor;
            }
            
            // Show or hide interaction prompt
            if (playerInRange && !wasInRange)
            {
                if (UIPromptController.Instance != null)
                {
                    UIPromptController.Instance.ShowPrompt(interactionPrompt);
                }
            }
            else if (!playerInRange && wasInRange)
            {
                if (UIPromptController.Instance != null)
                {
                    UIPromptController.Instance.HidePrompt();
                }
            }
            
            // Check for interaction input
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }
    
    // Method called when player interacts with this object
    protected virtual void Interact()
    {
        Debug.Log("Player interacted with " + gameObject.name);
        // Override this method in derived classes to implement specific interactions
    }
    
    // Show interaction radius in editor for debugging
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}