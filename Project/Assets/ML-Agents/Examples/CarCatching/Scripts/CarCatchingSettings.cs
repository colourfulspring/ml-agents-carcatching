using System;
using UnityEngine;

public class CarCatchingSettings : MonoBehaviour
{
    /// <summary>
    /// The "walking speed" of the agents in the scene.
    /// </summary>
    public float agentRunSpeed;

    /// <summary>
    /// The agent rotation speed.
    /// Every agent will use this setting.
    /// </summary>
    public float agentRotationSpeed;

    /// <summary>
    /// The spawn area margin multiplier.
    /// ex: .9 means 90% of spawn area will be used.
    /// .1 margin will be left (so players don't spawn off of the edge).
    /// The higher this value, the longer training time required.
    /// </summary>
    public float spawnAreaMarginMultiplier;

    /// <summary>
    /// The precision of capture position map.
    /// </summary>
    public int pixelWidth;

    // public CapturePosMap CapturePosMap;
    public TrajList trajList;

    public void Awake()
    {
        // CapturePosMap = new CapturePosMap("data.json");
        trajList = new TrajList("rd_202408191817.json");
    }
}
