using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Update is called once per frame
    public Vector3 goal;

    private void Awake()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Awake: ");
    }

    void Start()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Start: ");
    }

    private void Update()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Start: ");
    }

    void FixedUpdate()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  FixedUpdate: ");
        GetComponent<NavMeshAgent>().SetDestination(goal + this.transform.parent.position);
    }
}
