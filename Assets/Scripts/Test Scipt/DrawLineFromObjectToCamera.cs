using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineFromObjectToCamera : MonoBehaviour
{
    public LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 gameObjectPosition = transform.position;
        Vector3 cameraCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));

        RaycastHit hit;
        if (Physics.Raycast(gameObjectPosition, (cameraCenterPosition - gameObjectPosition).normalized, out hit))
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, gameObjectPosition);
            lineRenderer.SetPosition(1, hit.point);
        }
    }
}
