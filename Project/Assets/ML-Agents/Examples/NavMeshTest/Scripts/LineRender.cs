using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LineRender : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPosition;
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField, Range(0f, 20f)] private float width = 2f;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        previousPosition = transform.position;
        line.SetPosition(0, previousPosition);
        line.startWidth = line.endWidth = width;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, currentPosition);
            previousPosition = currentPosition;
        }
    }
}
