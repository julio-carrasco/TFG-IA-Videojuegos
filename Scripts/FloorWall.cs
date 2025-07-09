using UnityEngine;
using UnityEngine.Events;

// Script to control the movement of a wall

public class FloorWall : MonoBehaviour
{
    public Transform wallVisual;
    public float speed = 5f;
    public float moveDistance = 3f;
    
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool shouldMove = false;

    void Start()
    {
        if(wallVisual == null) wallVisual = transform;
        initialPosition = wallVisual.position;
        targetPosition = initialPosition; // Start at initial position
        WallButton.buttonPressedEvent.AddListener(MoveWall);
    }

    public void MoveWall()
    {
        targetPosition = initialPosition + new Vector3(0, moveDistance, 0);
        shouldMove = true;
    }

    void Update()
    {
        if (shouldMove)
        {
            wallVisual.position = Vector3.MoveTowards(wallVisual.position, targetPosition, speed * Time.deltaTime);
            
            // Stop moving when we reach the target
            if (Vector3.Distance(wallVisual.position, targetPosition) < 0.01f)
            {
                shouldMove = false;
            }
        }
    }
}