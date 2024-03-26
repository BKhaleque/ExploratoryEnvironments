using UnityEngine;
using System.Collections.Generic;

public class EntropyEvaluator : MonoBehaviour
{
    // The resolution of the grid used to discretize the environment
    public float gridResolution = 1.0f;

    // Dictionary to store visited grid cells and their visit counts
    private Dictionary<Vector2Int, int> visitedCells = new Dictionary<Vector2Int, int>();

    // Total number of moves made by the agent
    private int totalMoves = 0;

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

        // Update the visit count for the grid cell
        if (visitedCells.ContainsKey(gridCell))
        {
            visitedCells[gridCell]++;
        }
        else
        {
            visitedCells[gridCell] = 1;
        }

        totalMoves++;
    }

    // Calculate the entropy of visited locations
    public float CalculateEntropy()
    {
        float entropy = 0.0f;

        foreach (var cell in visitedCells)
        {
            float probability = (float)cell.Value / totalMoves;
            entropy -= probability * Mathf.Log(probability, 2);
        }

        return entropy;
    }
}