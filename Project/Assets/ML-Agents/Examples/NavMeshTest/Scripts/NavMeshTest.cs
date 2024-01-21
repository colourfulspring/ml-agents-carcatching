using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(this.name + "  Awake: ");
    }

    void Start()
    {
        Debug.Log(this.name + "  Start: ");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.name + "  Update: ");
    }

    private void FixedUpdate()
    {
        Debug.Log(this.name + "  Fixed Update: ");
    }
}
