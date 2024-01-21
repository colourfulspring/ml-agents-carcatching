using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Awake: ");
    }

    void Start()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Start: ");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Update: ");
    }

    private void FixedUpdate()
    {
        Debug.Log(this.transform.parent.gameObject.name +
                  ", " + this.name + "  Fixed Update: ");
    }
}
