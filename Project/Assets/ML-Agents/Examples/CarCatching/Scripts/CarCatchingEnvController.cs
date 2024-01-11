using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
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
        [HideInInspector] public Vector3 GoalPosition;
    }

    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>
    [HideInInspector] public Bounds areaBounds;

    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    //List of Agents On Platform
    public List<CarInfo> AgentsList;

    public bool UseRandomAgentRotation = true;
    public bool UseRandomAgentPosition = true;

    private CarCatchingSettings m_CarCatchingSettings;

    // private int m_ResetTimer;

    void Start()
    {
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        m_CarCatchingSettings = FindObjectOfType<CarCatchingSettings>();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.StartingScale = item.Agent.transform.localScale;
            item.GoalPosition = item.Agent.transform.position;
        }

        ResetScene();
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
            var randomPosX = Random.Range(-areaBounds.extents.x * m_CarCatchingSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * m_CarCatchingSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * m_CarCatchingSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.z * m_CarCatchingSettings.spawnAreaMarginMultiplier);
            //Global Position
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, localY, randomPosZ);
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
        // m_ResetTimer = 0;

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
        Vector2[] ans = new Vector2[AgentsList.Count];
        // absolute position in the prefab of the current agent
        Vector2 agentAbsolutePos;
        Debug.Log(agent.transform.parent.gameObject.name +
                  ", " + agent.name + ", " + agent.transform.position);
        // Put normalized absolute position of the current agent at beginning
        {
            Vector3 pos = agent.transform.localPosition;
            agentAbsolutePos = new Vector2(pos.x, pos.z);
            ans[0] = NormalizePos2d(agentAbsolutePos);
        }
        // Put normalized absolute position of other agents to current agent in order.
        for (int i = 0, j = 1; i < AgentsList.Count; ++i)
        {
            Debug.Log(AgentsList[i].Agent.transform.parent.gameObject.name +
                      ", " + AgentsList[i].Agent.name + ", " + AgentsList[i].Agent.transform.position);
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
}
