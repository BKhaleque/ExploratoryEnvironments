using UnityEngine;

public class CoverageRecorder : MonoBehaviour
{
    public int gridWidth = 350;
    public int gridHeight = 350;
    public float cellSize = 1f; // Size of each cell in the grid
    private bool[,] exploredGrid;
    private double totalCells;
    private int exploredCells = 0;

    void Start()
    {
        InitializeGrid();
    }

    void FixedUpdate()
    {
        RecordExploration();
    }

    public void InitializeGrid()
    {
        exploredGrid = new bool[(int)(gridWidth/cellSize), (int)(gridHeight/cellSize)];
        totalCells = (gridWidth/cellSize * gridHeight/cellSize);
        exploredCells = 0;
    }

    void RecordExploration()
    {
        Vector2 gridPosition = WorldToGrid(transform.position);
        int x = Mathf.FloorToInt(gridPosition.x);
        int y = Mathf.FloorToInt(gridPosition.y);

        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            if (!exploredGrid[x, y])
            {
                exploredGrid[x, y] = true;
                exploredCells++;
               // Debug.Log($"Explored {exploredCells} out of {totalCells} cells. Coverage: {GetExplorationPercentage()}%");
            }
        }
    }

    float GetExplorationPercentage()
    {
        return (exploredCells / (float)totalCells) * 100;
    }

    Vector2 WorldToGrid(Vector3 worldPosition)
    {
        // Assuming the bottom-left of the grid is at (0,0,0) in world space
        float x = worldPosition.x / cellSize;
        float y = worldPosition.z / cellSize; // Use Z for a flat XZ plane representation
        return new Vector2(x, y);
    }
}