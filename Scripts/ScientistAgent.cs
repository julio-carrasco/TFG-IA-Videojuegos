using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Events;

// Script that controls the scientist agent's behavior in the environment.
public class ScientistAgent : Agent
{
    public float agentRunSpeed;
    public bool useVectorObs;
    Rigidbody m_AgentRb;
    public bool buttonRewards = false;
    private bool button1 = false;
    private bool button2 = false;
    private bool button3 = false;
    private Vector3 startPos;
    public static UnityEvent reachedGoal = new UnityEvent();

    // Initialize is called when the agent is created
    // This is where we set up the agent's Rigidbody and subscribe to button events
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        startPos = transform.position;
        FrictionButton.buttonPressedEvent.AddListener(ButtonReward);
        WallButton.buttonPressedEvent.AddListener(ButtonReward2);
        ObstacleButton.buttonPressedEvent.AddListener(ButtonReward3);
    }

    // CollectObservations is called to gather the agent's observations, since the agent relies on raycast observations this is mostly empty
    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(StepCount / (float)MaxStep);
        }
    }

    // MoveAgent is called to move the agent based on the actions received from the neural network
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var moveAction = act[0];
        var rotateAction = act[1];
        switch (moveAction)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
        }
        switch (rotateAction)
        {
            case 1:
                rotateDir = transform.up * 1f;
                break;
            case 2:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 150f);
        m_AgentRb.AddForce(dirToGo * agentRunSpeed, ForceMode.VelocityChange);

    }

    // OnActionReceived is called when the agent receives actions from the neural network
    // It applies a small negative reward for each step taken to encourage efficiency
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);
        CheckAgentOutOfBounds();
    }

    // CheckAgentOutOfBounds checks if the agent has moved too far from its starting position
    private void CheckAgentOutOfBounds()
    {
        Debug.Log("Checking out of bounds");
        if (Mathf.Abs(transform.position.y) - Mathf.Abs(startPos.y) > 40)
        {
            Debug.Log("Agent out of bounds, resetting episode");
            SetReward(-1f);
            EndEpisode();
        }
    }
    // ButtonReward methods are called when the respective buttons are pressed
    private void ButtonReward()
    {
        if (!button1 && buttonRewards)
        {
            button1 = true;
            AddReward(0.5f);
        }

    }
    private void ButtonReward2()
    {
        if (!button2 && buttonRewards)
        {
            button2 = true;
            AddReward(0.5f);
        }
    }
    private void ButtonReward3()
    {
        if (!button3 && buttonRewards)
        {
            button3 = true;
            AddReward(0.5f);
        }
    }
    // ResetButtonRewards resets the button rewards to false this is called when a new episode starts
    private void ResetButtonRewards()
    {
        button1 = false;
        button2 = false;
        button3 = false;
    }
    // OnCollisionEnter is called when the agent collides with another object
    // If it collides with the goal, it invokes the reachedGoal event and gives a reward
    // If it collides with an enemy, it gives a negative reward and ends the episode
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("goal"))
        {
            reachedGoal?.Invoke();
            Debug.Log("Goal reached!");
            SetReward(3f);
        }
        if (col.gameObject.CompareTag("enemy"))
        {
            SetReward(-2f);
            EndEpisode();
        }
    }
    // Heuristic is called to allow manual control of the agent using keyboard input
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }
    // OnEpisodeBegin is called at the start of each episode
    // It resets the agent's position and assigns a random rotation
    // It also resets the button rewards if they are enabled
    public override void OnEpisodeBegin()
    {
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        transform.position = startPos;
        m_AgentRb.linearVelocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;
        if (buttonRewards)
        {
            ResetButtonRewards();
        }
    }
}
