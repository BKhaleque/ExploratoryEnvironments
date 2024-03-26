using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathComplexity : MonoBehaviour
{

    public Transform agentTransform; // Assign the agent's transform here
    private List<Vector3> pathPoints = new List<Vector3>();
    private float totalDistance = 0f;
    private int totalTurns = 0;
    private Vector3 lastDirection;

    void Start()
    {
        // Initialize the last direction with the agent's initial forward direction
        lastDirection = agentTransform.forward;
        // Add the initial position
        pathPoints.Add(agentTransform.position);
    }

    void Update()
    {
        // Check if the agent has moved a significant distance before logging the next point
        if (Vector3.Distance(agentTransform.position, pathPoints[pathPoints.Count - 1]) > 1.0f)
        {
            // Add the new position to the path
            pathPoints.Add(agentTransform.position);
            // Calculate the distance traveled since the last point
            totalDistance += Vector3.Distance(agentTransform.position, pathPoints[pathPoints.Count - 2]);

            // Check for a change in direction
            Vector3 currentDirection = (agentTransform.position - pathPoints[pathPoints.Count - 2]).normalized;
            if (Vector3.Angle(lastDirection, currentDirection) > 10f) // 10 degrees threshold for a turn
            {
                totalTurns++;
                lastDirection = currentDirection;
            }
        }
    }

    void OnDisable()
    {
        // Output the path complexity metrics
        Debug.Log("Total Distance Traveled: " + totalDistance);
        Debug.Log("Total Number of Turns: " + totalTurns);
    }

}
