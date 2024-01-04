using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class CarCatchingEnvController : MonoBehaviour
{
    [System.Serializable]
    public class CarInfo
    {
        public CarAgent Agent;
        [HideInInspector] public Vector3 StartingPos;
        [HideInInspector] public Quaternion StartingRot;
        [HideInInspector] public Vector3 StartingScale;
        [HideInInspector] public Rigidbody Rb;
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

    public GameObject area;

    //List of Catching Agents On Platform
    public List<CarInfo> CatchingAgentsList = new List<CarInfo>();

    //List of Running Agents On Platform
    public List<CarInfo> RunningAgentsList = new List<CarInfo>();

    public bool UseRandomAgentRotation = true;
    public bool UseRandomAgentPosition = true;

    private CarCatchingSettings m_CarCatchingSettings;

    // private int m_ResetTimer;

    void Start()
    {
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        m_CarCatchingSettings = FindObjectOfType<CarCatchingSettings>();
        foreach (var item in CatchingAgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.StartingScale = item.Agent.transform.localScale;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
        }

        foreach (var item in RunningAgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.StartingScale = item.Agent.transform.localScale;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
        }

        ResetScene();
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos(Quaternion rot)
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        var fixedY = RunningAgentsList[0].Agent.transform.position.y;
        var agentHalfExtents = RunningAgentsList[0].StartingScale * 0.5f;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * m_CarCatchingSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * m_CarCatchingSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * m_CarCatchingSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.z * m_CarCatchingSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, fixedY, randomPosZ);
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

        //Reset Catching Agents
        foreach (var item in CatchingAgentsList)
        {
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos(rot) : item.StartingPos;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        //Reset Running Agents
        foreach (var item in RunningAgentsList)
        {
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos(rot) : item.StartingPos;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
    }
}
