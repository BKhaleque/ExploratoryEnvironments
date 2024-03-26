using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverageAndDistribution : MonoBehaviour
{
    public Transform agentTransform; // Assign the agent's transform here
    public int gridSize = 10; // Size of the grid
    private HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();
    private int totalCells;

    void Start()
    {
        totalCells = gridSize * gridSize;
        MarkVisitedCell(agentTransform.position);
    }

    void FixedUpdate()
    {
        MarkVisitedCell(agentTransform.position);
    }

    private void MarkVisitedCell(Vector3 position)
    {
        var gridPosition = new Vector2Int(
            Mathf.FloorToInt(position.x / gridSize),
            Mathf.FloorToInt(position.z / gridSize));
        //check if the cell has been visited before
        if (visitedCells.Contains(gridPosition))
        {
            return;
        }

        visitedCells.Add(gridPosition);

    }

    public void resetCoverageAndDistribution()
    {
        visitedCells.Clear();
    }

    public KeyValuePair<float,float> getCoverageAndDistribution()
    {
        // Calculate coverage
        // ReSharper disable once PossibleLossOfFraction
        var coverage = (float) (visitedCells.Count / totalCells);
        Debug.Log("Coverage: " + coverage);

        // For distribution, analyze the distribution of visited cells
        // For a simple approach, calculate the standard deviation of the distances between visited cells
        // This will give you a measure of how evenly distributed the visited cells are
        float sumDistances = 0f;
        foreach (Vector2Int cell1 in visitedCells)
        {
            foreach (Vector2Int cell2 in visitedCells)
            {
                sumDistances += Vector2Int.Distance(cell1, cell2);
            }
        }
        float meanDistance = sumDistances / (visitedCells.Count * visitedCells.Count);
        float sumSquaredDistances = 0f;
        foreach (Vector2Int cell1 in visitedCells)
        {
            foreach (Vector2Int cell2 in visitedCells)
            {
                sumSquaredDistances += Mathf.Pow(Vector2Int.Distance(cell1, cell2) - meanDistance, 2);
            }
        }
        float standardDeviation = Mathf.Sqrt(sumSquaredDistances / (visitedCells.Count * visitedCells.Count));
        Debug.Log("Distribution: " + standardDeviation);
        return new KeyValuePair<float, float>(coverage, standardDeviation);
    }
}
