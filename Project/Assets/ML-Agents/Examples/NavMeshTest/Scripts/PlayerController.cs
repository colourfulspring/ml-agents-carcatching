using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(new Vector3(0, 0, 12.66f));
    }
}
