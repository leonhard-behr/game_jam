using UnityEngine;

public class CollectibleItem : InteractiveObject
{
    [Header("Collectible Settings")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool destroyOnCollect = true;
    [SerializeField] private GameObject collectEffect;
    
    protected override void Interact()
    {
        // Add score or other game mechanics
        Debug.Log("Player collected " + gameObject.name + " worth " + scoreValue + " points");
        
        // Spawn collection effect if available
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Destroy the object if configured to do so
        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
    }
}
