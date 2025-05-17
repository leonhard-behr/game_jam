using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    
    [Header("Camera Position Settings")]
    [SerializeField] private Vector3 positionOffset = new Vector3(-5, -5, -8); // Offset for 2.5D view
    [SerializeField] private Vector3 rotationAngles = new Vector3(-20, 0, 0); // Rotation for 2.5D view
    
    // Store the initial rotation to maintain consistency
    private Quaternion targetRotation;
    
    private void Start()
    {
        // Set the initial rotation based on the specified angles
        targetRotation = Quaternion.Euler(rotationAngles);
        transform.rotation = targetRotation;
    }
    
    private void LateUpdate()
    {
        if (target == null)
        {
            // Try to find the player if target is not set
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                return;
            }
        }
        
        // Calculate the desired position relative to the target
        // We only want to follow the X and Z position, ignoring Y to maintain our view
        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = new Vector3(
            targetPosition.x + positionOffset.x,
            targetPosition.y + positionOffset.y,
            targetPosition.z + positionOffset.z
        );
        
        // Smoothly move the camera to that position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Ensure we maintain our target rotation
        transform.rotation = targetRotation;
    }
}
