using UnityEngine;

// Script to respawn a goal object at a random position within a specified range
// The goal will respawn if it collides with an agent or if it is not in a valid position
// It checks for obstacles within a specified radius to ensure the goal is not placed
// in a position that would cause it to be stuck or inaccessible
public class GoalRespawner : MonoBehaviour
{
    public float rangeX = 60f;
    public float rangeZ = 60f; // Range for random respawn position
    public float checkRadius = 2f; // Radius to check for collisions
    public float yOffset = 3.5f; // Y offset for the goal position
    public LayerMask obstacleMask; // Set this in the inspector to include only obstacles

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        Respawn();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("agent"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        Vector3 newPos;
        int attempts = 0;
        int maxAttempts = 50; // Avoid infinite loops

        do
        {
            float x = Random.Range(startPos.x - rangeX, startPos.x + rangeX);
            float z = Random.Range(startPos.z - rangeZ, startPos.z + rangeZ);
            newPos = new Vector3(x, yOffset, z);
            attempts++;
        }
        while (Physics.CheckSphere(newPos, checkRadius, obstacleMask) && attempts < maxAttempts);

        if (attempts < maxAttempts)
        {
            transform.position = newPos;
        }
        else
        {
            Debug.LogWarning("Failed to find empty space for goal after many attempts, default position.");
            transform.position = new Vector3(startPos.x, yOffset, startPos.z); // Fallback to start position if no valid position found
        }
    }
}
