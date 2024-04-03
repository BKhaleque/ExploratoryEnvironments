using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationChange : DirectionMetric
{

    private float lastElevation = 0f;
    private int uphillChoices = 0;
    private int downhillChoices = 0;
    private float elevationChangeThreshold = 0.5f; // Threshold to distinguish between level, uphill, and downhill
    private float score = 0f;

    private GameObject terrain;

    private Vector3[] vertices;
    // Start is called before the first frame update
    void Start()
    {
        //lastElevation = agentTransform.position.y;
        terrain = GameObject.Find("Generator");
        vertices = terrain.GetComponent<GenerateRandomEnvironment>().meshToRender.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        // Assuming the agent makes a decision at regular intervals or certain conditions
        // Call EvaluateDecision() when a decision is made
        //EvaluateDecision();
    }

    public override void EvaluateDecision(Vector3 direction)
    {
        score = 0f;
        var currentElevation = transform.position.y;
        uphillChoices = 0;
        downhillChoices = 0;
        float elevationChange = 0;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 80f))
        {
            if (hit.collider.gameObject.name.Contains("Terrain"))
            {
                //get the closest vertex to the hit point
                Vector3 closestVertex = Vector3.zero;
                float minDistance = Mathf.Infinity;
                foreach (Vector3 vertex in vertices)
                {
                    float distance = Vector3.Distance(hit.point, vertex);
                    if (distance < minDistance)
                    {
                        closestVertex = vertex;
                        minDistance = distance;
                    }
                }
                //get the elevation of the closest vertex
                elevationChange =  closestVertex.y - currentElevation;
            }
        }
        
        // for each 1 unit of elevation change, add 0.1 to the score, cap at 1f
        if(Mathf.Abs(elevationChange) < 10f)
            score = Mathf.Abs(elevationChange)/10f;
        else
            score = 1f;
        
        
        //

        // if (elevationChange > elevationChangeThreshold)
        // {
        //     uphillChoices++;
        // }
        // else if (elevationChange < -elevationChangeThreshold)
        // {
        //     downhillChoices++;
        // }
        // else
        // {
        //     levelChoices++;
        // }

        //lastElevation = currentElevation;
    }

    public override float GetScore()
    {
        // Calculate the metric, for example, as a ratio of uphill choices to total choices
        // var totalChoices = uphillChoices + downhillChoices;
        // if(inverse)
        //     return totalChoices > 0 ? (float)downhillChoices / totalChoices : 0;
        //
        // return totalChoices > 0 ? (float)uphillChoices / totalChoices : 0;
        return score;   
    }

    void OnDisable()
    {
        // Output the elevation change preference score
    //    Debug.Log("Elevation Change Score: " + GetScore());
    }
}
