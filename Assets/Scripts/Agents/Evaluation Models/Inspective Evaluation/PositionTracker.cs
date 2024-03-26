using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PositionTracker : MonoBehaviour
{
    public float recordInterval = 1.0f; // Time interval in seconds to record position
    private Dictionary<float, Vector3> positions = new ();
    private float timer = 0.0f;
    public string filePath;

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        
        positions.Add(timer, transform.position);
        
    }



    public void WritePositionsToFile()
    {
        StringBuilder sb = new StringBuilder();
        System.IO.File.WriteAllText(filePath,string.Empty);

        foreach (var pos in positions)
        {
            //writing the position to the file
            sb.AppendLine(pos.Key + "," + pos.Value);
        }

        File.WriteAllText(filePath, sb.ToString());
        Debug.Log("Positions saved to " + filePath);
        positions = new ();
        timer = 0.0f;
    }
}
