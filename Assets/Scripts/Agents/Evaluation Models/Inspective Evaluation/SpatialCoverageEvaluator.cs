using UnityEngine;
using System.Collections.Generic;

public class SpatialCoverageEvaluator : MonoBehaviour
{
    // The resolution of the grid used to discretize the environment
    public float gridResolution = 1.0f;

    // Dictionary to store visited grid cells
    private HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();

    // Dimensions of the environment (you can adjust these based on your specific environment)
    public Vector2 environmentDimensions = new Vector2(350, 350);

    private void Update()
    {
        RecordPosition(transform.position);
    }

    // Record the agent's current position
    private void RecordPosition(Vector3 position)
    {
        // Convert the 3D position to a 2D grid cell
        Vector2Int gridCell = new Vector2Int(
            Mathf.FloorToInt(position.x / gridResolution),
            Mathf.FloorToInt(position.z / gridResolution)
        );

        visitedCells.Add(gridCell);
    }

    // Calculate the spatial coverage percentage
    public float CalculateCoveragePercentage()
    {
        int totalCells = Mathf.FloorToInt(environmentDimensions.x / gridResolution) * 
                         Mathf.FloorToInt(environmentDimensions.y / gridResolution);

        return (float)visitedCells.Count / totalCells * 100.0f;
    }
}
