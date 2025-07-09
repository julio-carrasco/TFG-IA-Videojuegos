using UnityEngine;
using UnityEngine.Events;

// Script to control a button that can be pressed by an agent that spawns obstacles
public class ObstacleButton : MonoBehaviour
{
    public Transform buttonVisual; // The part that moves when pressed
    public float pressDepth = 0.05f;
    public float pressSpeed = 5f;
    private Vector3 initialPosition;
    private bool isPressed = false;
    private bool spawned = false;
    public GameObject obstaclePrefab;
    public int numberToSpawn = 5;
    public Transform spawnAreaCenter;
    public Vector3 spawnAreaSize;
    public static UnityEvent buttonPressedEvent = new UnityEvent();
    void Start()
    {
        if (buttonVisual == null) buttonVisual = transform;
        initialPosition = buttonVisual.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        // Animate button press
        if (isPressed)
        {
            buttonVisual.localPosition = Vector3.Lerp(
                buttonVisual.localPosition,
                initialPosition - new Vector3(0, pressDepth, 0),
                Time.deltaTime * pressSpeed
            );
        }
        else
        {
            buttonVisual.localPosition = Vector3.Lerp(
                buttonVisual.localPosition,
                initialPosition,
                Time.deltaTime * pressSpeed
            );
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            SpawnObstacles();
        }
    }
    private void SpawnObstacles()
    {
        if (spawned) return;
        buttonPressedEvent?.Invoke();
        spawned = true;
        isPressed = true;
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(0, spawnAreaSize.y) + 10,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );
            Vector3 spawnPosition = spawnAreaCenter.position + randomOffset;
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            isPressed = false;
        }
    }
}
