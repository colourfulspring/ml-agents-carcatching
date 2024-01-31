using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.Sentis.Layers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CarCatchingEnvController : MonoBehaviour
{
    [System.Serializable]
    public class CarInfo
    {
        public CarAgent Agent;

        // StartingPos and StartingRot is used when UseRandomAgentPosition is true and UseRandomAgentRotation is true
        [HideInInspector] public Vector3 StartingPos;

        [HideInInspector] public Quaternion StartingRot;

        // StartingScale is used for collision detection between cars and walls during initialization
        [HideInInspector] public Vector3 StartingScale;
        [HideInInspector] public DecisionRequester DecisionRequester;
        [HideInInspector] public NavMeshAgent NavMeshAgent;
    }

    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps;

    /// <summary>
    /// The area bounds.
    /// </summary>
    [HideInInspector] public Bounds areaBounds;

    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    /// <summary>
    /// List of Agents On Platform.
    /// The running agents must stay at a index that less than the catching agents.
    /// </summary>
    public List<CarInfo> AgentsList;

    /// <summary>
    /// Agents at [0, RunningNum-1] are Running Agents, Agents at [RunningNum, AgentList.Count-1] are Catching Agents
    /// </summary>
    private int RunningNum;

    public bool UseRandomAgentRotation = true;
    public bool UseRandomAgentPosition = true;

    private CarCatchingSettings m_CarCatchingSettings;

    public int ResetTimer;

    protected void Awake()
    {
        // Get the ground's bounds
        Debug.Log( this.name + "  Awake: ");
        areaBounds = ground.GetComponent<Collider>().bounds;
        m_CarCatchingSettings = FindObjectOfType<CarCatchingSettings>();
    }

    void Start()
    {
        Debug.Log( this.name + "  Start: ");
        foreach (var item in AgentsList)
        {
            var itemTrans = item.Agent.transform;
            item.StartingPos = itemTrans.position;
            item.StartingRot = itemTrans.rotation;
            item.StartingScale = itemTrans.localScale;
            item.DecisionRequester = item.Agent.GetComponent<DecisionRequester>();
            item.NavMeshAgent = item.Agent.GetComponent<NavMeshAgent>();
        }

        for (; RunningNum < AgentsList.Count && AgentsList[RunningNum].Agent.isRunning; ++RunningNum) ;
        // Academy.Instance.OnEnvironmentReset += ResetScene;
        ResetScene();
    }

    /// <summary>
    /// Use the ground's bounds to pick a random local position in the area bounds. Make sure the Y value
    /// is 0, X and Z value lies in [-areaBounds.extents.x(z), areaBounds.extents.x(z)], respectively.
    /// </summary>
    public Vector3 GetRandomPos()
    {
        var randomPosX = Random.Range(-areaBounds.extents.x * m_CarCatchingSettings.spawnAreaMarginMultiplier,
            areaBounds.extents.x * m_CarCatchingSettings.spawnAreaMarginMultiplier);

        var randomPosZ = Random.Range(-areaBounds.extents.z * m_CarCatchingSettings.spawnAreaMarginMultiplier,
            areaBounds.extents.z * m_CarCatchingSettings.spawnAreaMarginMultiplier);
        return new Vector3(randomPosX, 0f, randomPosZ);
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position. The requirements is that the car's collider
    /// doesn't collide with any wall's collider
    /// </summary>
    public Vector3 GetRandomSpawnPos(Quaternion rot)
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        var localY = AgentsList[0].Agent.transform.localPosition.y; // Attention: local Y
        var agentHalfExtents = AgentsList[0].StartingScale * 0.5f;
        while (foundNewSpawnLocation == false)
        {
            //Global Position = ground position + x,z   local position + y
            randomSpawnPos = ground.transform.position + GetRandomPos() + new Vector3(0f, localY, 0f);
            Debug.Log( this.name + "  GetRandomSpawnPos: " + randomSpawnPos);
            if (Physics.CheckBox(randomSpawnPos, agentHalfExtents, rot) == false)
            {
                foundNewSpawnLocation = true;
            }
        }

        return randomSpawnPos;
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }

    public void ResetScene()
    {
        Debug.Log( this.name + "  ResetScene: ");

        ResetTimer = 0;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos(rot) : item.StartingPos;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
        }
    }

    //Provide Position Observations for each Agent. Always put the current agent at beginning.
    //All other agents except the current one are arranged in a pre-defined order.
    public Vector2[] GetAgentPosObs(CarAgent agent)
    {
        Debug.Log(agent.transform.parent.gameObject.name + ", " + agent.name + ", " + "GetAgentPosObs: ");

        Vector2[] ans = new Vector2[AgentsList.Count];
        // absolute position in the prefab of the current agent
        Vector2 agentAbsolutePos;
        // Debug.Log(agent.transform.parent.gameObject.name +
        //           ", " + agent.name + ", " + agent.transform.position);
        // Put normalized absolute position of the current agent at beginning
        {
            Vector3 pos = agent.transform.localPosition;
            agentAbsolutePos = new Vector2(pos.x, pos.z);
            ans[0] = NormalizePos2d(agentAbsolutePos);
        }
        // Put normalized absolute position of other agents to current agent in order.
        for (int i = 0, j = 1; i < AgentsList.Count; ++i)
        {
            // Debug.Log(AgentsList[i].Agent.transform.parent.gameObject.name +
            //           ", " + AgentsList[i].Agent.name + ", " + AgentsList[i].Agent.transform.position);
            if (AgentsList[i].Agent == agent)
                continue;

            Vector3 pos = AgentsList[i].Agent.transform.localPosition;
            Vector2 pos_2d = new Vector2(pos.x, pos.z);
            ans[j] = NormalizePos2d(pos_2d);

            ++j;
        }
        return ans;
    }

    // The primitive position lies in range [-areaBounds.extents.x,areaBounds.extents.x]*[-areaBounds.extents.z,areaBounds.extents.z].
    // The normalized position lies in range [-1,1]*[-1,1]
    public Vector2 NormalizePos2d(Vector2 pos)
    {
        Vector2 ans = new Vector2(pos.x / areaBounds.extents.x,
            pos.y / areaBounds.extents.z);
        // Debug.Log(areaBounds.extents + " " + pos + " " + ans);
        return ans;
    }

    public void FixedUpdate()
    {
        Debug.Log( this.name + "  FixedUpdate: " + ResetTimer + "   " + transform.position);

        if (ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            for (int i = 0; i < AgentsList.Count; ++i)
            {
                AgentsList[i].Agent.EndEpisode();
            }
            ResetScene();
        }

        // CollectObservation and onActionReceived of all Agents will be called if this condition is satisfied
        if (ResetTimer % AgentsList[0].DecisionRequester.DecisionPeriod == 0)
        {
            // calculate NavMeshPath distance of all catching cars to the running car.
            // Attention! This reward calculation process only support one running car!!!!!!!!!!!
            Vector3 TargetPosition = AgentsList[0].Agent.transform.position;
            NavMeshPath path = new NavMeshPath();
            for (int i = RunningNum; i < AgentsList.Count; ++i)
            {
                Vector3 SourcePosition = AgentsList[i].Agent.transform.position;

                // Find a path from each catching car towards the running car
                float distance = -(2 * areaBounds.extents.x + 2 * areaBounds.extents.z); // sentinel, so no path reward is -1
                if (NavMesh.CalculatePath(SourcePosition, TargetPosition, AgentsList[i].NavMeshAgent.areaMask, path))
                {
                    distance = Vector3.Distance(SourcePosition, path.corners[0]);
                    for (int j = 1; j < path.corners.Length; ++j)
                    {
                        distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                    }
                }
                else
                {
                    Debug.Log("NO PATH FOUND!!!!");
                }
                Debug.Log(AgentsList[i].Agent.transform.parent.gameObject.name +
                          ", " + AgentsList[i].Agent.name + "  Distance: " + distance + "   " + ResetTimer + "   "
                          + AgentsList[i].Agent.transform.position + "   " + AgentsList[0].Agent.transform.position);
                float reward = -Mathf.Abs(distance) / (2 * areaBounds.extents.x + 2 * areaBounds.extents.z);
                AgentsList[i].Agent.AddReward(reward);
                Debug.Log(AgentsList[i].Agent.transform.parent.gameObject.name +
                          ", " + AgentsList[i].Agent.name + "  Addrewards: " + reward + "   " + ResetTimer + "   "
                          + AgentsList[i].Agent.transform.position + "   " + AgentsList[0].Agent.transform.position);
            }
        }

        ResetTimer += 1;
    }
}
