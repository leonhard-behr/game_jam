using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string targetSceneName; // This should be "env2", not "SpawnPoint_env2"
    [SerializeField] private string spawnPointName = "SpawnPoint"; // This should be "env2" or "SpawnPoint_env2"
    [SerializeField] private bool requiresInteraction = true;
    [SerializeField] private string transitionPrompt = "Press E to enter";
    
    private bool playerInRange = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Auto-transition if interaction is not required
            if (!requiresInteraction)
            {
                TransitionToScene();
            }
            else if (UIPromptController.Instance != null)
            {
                UIPromptController.Instance.ShowPrompt(transitionPrompt);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            if (UIPromptController.Instance != null)
            {
                UIPromptController.Instance.HidePrompt();
            }
        }
    }
    
    private void Update()
    {
        if (playerInRange && requiresInteraction && Input.GetKeyDown(KeyCode.E))
        {
            TransitionToScene();
        }
    }
    
    private void TransitionToScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target scene name is not set!");
            return;
        }
        
        // Prepare spawnPointName
        if (!string.IsNullOrEmpty(spawnPointName) && 
            !spawnPointName.StartsWith("SpawnPoint_") && 
            spawnPointName != "SpawnPoint")
        {
            spawnPointName = "SpawnPoint_" + spawnPointName;
        }
        
        // Use FadeManager to handle transition with fade effect
        if (FadeManager.Instance != null)
        {
            Debug.Log($"Using FadeManager to transition to {targetSceneName} with spawn point {spawnPointName}");
            FadeManager.Instance.FadeAndLoadScene(targetSceneName, spawnPointName);
        }
        else
        {
            // Fallback to direct loading if FadeManager isn't available
            Debug.LogWarning("FadeManager not found! Falling back to direct scene loading");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SavePlayerState();
                GameManager.Instance.targetSpawnPoint = spawnPointName;
            }
            
            SceneManager.LoadScene(targetSceneName);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}