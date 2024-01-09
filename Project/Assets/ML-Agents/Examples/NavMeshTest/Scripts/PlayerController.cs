using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Update is called once per frame
    public Vector3 goal;
    void Update()
    {
        GetComponent<NavMeshAgent>().SetDestination(goal);
    }
}
