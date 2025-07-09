using UnityEngine;

// Script to control a button that can be pressed by an agent that fires event to reduce the friction of the floor
public class friction : MonoBehaviour
{
    public Collider floorCollider;
    public PhysicsMaterial normalFriction;
    public PhysicsMaterial lowFriction;
    public float slipperyDuration = 3f;

    void Start()
    {
        FrictionButton.buttonPressedEvent.AddListener(MakeSlippery);
    }

    private void MakeSlippery()
    {
        StartCoroutine(TemporarilyReduceFriction());
    }

    private System.Collections.IEnumerator TemporarilyReduceFriction()
    {
        Debug.Log("Making floor slippery for " + slipperyDuration + " seconds.");
        floorCollider.material = lowFriction;
        yield return new WaitForSeconds(slipperyDuration);
        floorCollider.material = normalFriction;
    }
}
