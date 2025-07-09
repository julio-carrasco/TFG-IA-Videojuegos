using UnityEngine;
using UnityEngine.Events;

// Script to control a button that can be pressed by an agent that fires event to reduce the firction of the floor
public class FrictionButton : MonoBehaviour
{
    public Transform buttonVisual; // The part that moves when pressed
    public float pressDepth = 0.05f;
    public float pressSpeed = 5f;
    public static UnityEvent buttonPressedEvent = new UnityEvent();
    private Vector3 initialPosition;
    private bool isPressed = false;

    private void Start()
    {
        if (buttonVisual == null) buttonVisual = transform;
        initialPosition = buttonVisual.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        if (isPressed) return;

        isPressed = true;
        buttonPressedEvent?.Invoke();
    }

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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            isPressed = false;
        }
    }
}
