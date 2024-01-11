using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class CarAgent : Agent
{
    private CarCatchingSettings m_CarCatchingSettings;
    public CarCatchingEnvController carCatchingEnvController;

    public bool useRandomNavigationGoal;

    // The user specified goal of NavMeshAgent
    public Vector3 customNavigationGoal;

    protected override void Awake()
    {
        base.Awake();
        m_CarCatchingSettings = FindObjectOfType<CarCatchingSettings>();
    }

    void Start()
    {
        var goal = useRandomNavigationGoal ? customNavigationGoal : customNavigationGoal;
        GetComponent<NavMeshAgent>().SetDestination(customNavigationGoal);
    }

    /// <summary>
    /// Collects vector observations for each agent.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Debug.Log(this.transform.parent.gameObject.name +
        //           ", " + this.name + "CollectObservations: ");
        // Vector2[] obs = carCatchingEnvController.GetAgentPosObs(this);
        // foreach (var myobs in obs)
        // {
        //     sensor.AddObservation(myobs);
        //     // Debug.Log(myobs + ",");
        // }
        sensor.AddOneHotObservation(2, 8);
    }

    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        // Debug.Log(this.transform.parent.gameObject.name +
        //           ", " + this.name + " OnActionReceived: " + action);
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }

        transform.Rotate(rotateDir, m_CarCatchingSettings.agentRotationSpeed);
    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move the agent using the action.
        // MoveAgent(actionBuffers.DiscreteActions);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    public void FixedUpdate()
    {
    }
}
