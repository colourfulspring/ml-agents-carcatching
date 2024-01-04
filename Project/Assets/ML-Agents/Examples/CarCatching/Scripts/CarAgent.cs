using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;


public class CarAgent : Agent
{
    private CarCatchingSettings m_CarCatchingSettings;
    private Rigidbody m_CatchingCarRb; //cached on initialization

    protected override void Awake()
    {
        m_CarCatchingSettings = FindObjectOfType<CarCatchingSettings>();
    }

    public override void Initialize()
    {
        // Cache the agent rb
        m_CatchingCarRb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        // m_CatchingCarRb.AddForce(Vector3.right, ForceMode.VelocityChange);
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + ": " + this.transform.position + " " +
                  this.transform.localPosition);
    }
}
