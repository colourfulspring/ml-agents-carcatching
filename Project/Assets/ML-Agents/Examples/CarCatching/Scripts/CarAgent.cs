using Unity.Mathematics;
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

    // The radius of the decision range. We inversely normalize the output of neural network
    // to this range
    public int decisionRangeRadius;

    private NavMeshAgent m_AgentNavMeshAgent;  //cached on initialization

    public bool isRunning;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Awake: ");
        m_CarCatchingSettings = FindObjectOfType<CarCatchingSettings>();
    }

    void Start()
    {
        // var localY = transform.localPosition.y; // Attention: local Y, car in prefab

        // var navigationGoal = useRandomNavigationGoal
        //     ? carCatchingEnvController.GetRandomPos() + new Vector3(0f, localY, 0f)
        //     : customNavigationGoal;
        // GetComponent<NavMeshAgent>().SetDestination(navigationGoal);
    }

    public override void Initialize()
    {
        base.Initialize();
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Initialize: ");
        m_AgentNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Collects vector observations for each agent.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  CollectObservations: " + carCatchingEnvController.ResetTimer);
        Vector2[] obs = carCatchingEnvController.GetAgentPosObs(this);
        foreach (var myobs in obs)
        {
            Debug.Log(this.transform.parent.gameObject.name +
                      ", " + this.name + "  Observations: " + myobs);
            sensor.AddObservation(myobs);
        }
        // sensor.AddOneHotObservation(2, 8);
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
        var rawAction = new Vector2(actionBuffers.ContinuousActions[0], actionBuffers.ContinuousActions[1]);
        var clipAction = new Vector2(Mathf.Clamp(rawAction[0], -1f, 1f), Mathf.Clamp(rawAction[1], 0f, 1f));

        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  onActionReceived: " + rawAction + "   " + transform.position);

        // var reverselyNormalizedPos2d = ReverselyNormalizePos2dCartesian(rawAction);
        var reverselyNormalizedPos2d = ReverselyNormalizePos2dPolar(clipAction);

        // Add the reversely normalized action and the car's global position (because setDestination accept a global target position)
        var navigationGoal = transform.position +
                             new Vector3(reverselyNormalizedPos2d.x, 0f, reverselyNormalizedPos2d.y);

        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  onActionReceived: navigationGoal " + navigationGoal);
        m_AgentNavMeshAgent.SetDestination(navigationGoal);

        // Move the agent using the action.
        // MoveAgent(actionBuffers.DiscreteActions);
    }

    // This method reversely normalized pos output by neural network using Cartesian coordinate system. The car itself acts as origin.
    // So the legal decision range is a square whose side length is decisionRangeRadius.
    public Vector2 ReverselyNormalizePos2dCartesian(Vector2 pos)
    {
        Vector2 ans = new Vector2(pos.x * decisionRangeRadius, pos.y * decisionRangeRadius);
        return ans;
    }

    // This method reversely normalized pos output by neural network using Polar coordinate system. The car itself acts as origin.
    // The legal decision range is a circle never hits any obstacles.
    public Vector2 ReverselyNormalizePos2dPolar(Vector2 pos)
    {
        //pos.x is normalized angle, pos.y is normalized radius
        var normalizedAngle = pos.x;
        var normalizedRadius = pos.y;

        // non-normalized angle
        var angle = normalizedAngle * math.PI;
        float radius = decisionRangeRadius;
        Vector3 dir = new Vector3(math.cos(angle), 0, math.sin(angle));

        // Physics.DefaultRaycastLayers means all layers will be considered during casting except Ignore Raycast layer.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, decisionRangeRadius, Physics.DefaultRaycastLayers))
        {
            radius = hit.distance;
        }
        // non-normalized radius
        radius = normalizedRadius * radius;

        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  onActionReceived: polar angle " + angle + "polar radius  " + radius);
        Vector2 ans = new Vector2(radius * math.cos(angle), radius * math.sin(angle));
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  onActionReceived: polar target " + ans);
        return ans;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // var discreteActionsOut = actionsOut.DiscreteActions;
        // if (Input.GetKey(KeyCode.D))
        // {
        //     discreteActionsOut[0] = 3;
        // }
        // else if (Input.GetKey(KeyCode.W))
        // {
        //     discreteActionsOut[0] = 1;
        // }
        // else if (Input.GetKey(KeyCode.A))
        // {
        //     discreteActionsOut[0] = 4;
        // }
        // else if (Input.GetKey(KeyCode.S))
        // {
        //     discreteActionsOut[0] = 2;
        // }
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = 0f;
        continuousActionsOut[1] = 0f;
    }

    public void FixedUpdate()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  FixedUpdate: " + carCatchingEnvController.ResetTimer + "   " + transform.position);

        // Debug.Log( this.transform.parent.gameObject.name +
        //            ", " + this.name + "  StepCount: " + StepCount);
        // Debug.Log( this.transform.parent.gameObject.name +
        //            ", " + this.name + "  CompleteEpisodes: " + CompletedEpisodes);
    }
}
