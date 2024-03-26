using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Openness : DirectionMetric
{
    public float checkRadius = 80f; // Radius to check for openness

    private int openChoices = 0;
    private int enclosedChoices = 0;
    private float score;

    public void Update()
    {
        //OnDisable();
    }


    public override void EvaluateDecision(Vector3 direction)
    {
        openChoices = 0;
        enclosedChoices = 0;
        RaycastHit hit;
        // determine if the agent's next move is towards an open or enclosed area
        if (Physics.Raycast(this.transform.position, direction, out hit, checkRadius))
        {
            //first check we don't hit terrain
            if (!hit.collider.gameObject.name.Contains("Terrain"))
            {
                //get distance of hit
                float distance = Vector3.Distance(this.transform.position, hit.point);
                score = distance/checkRadius;
            }
            //     enclosedChoices++;
            // else
            //     openChoices++;
        }
        else
        {
            score = 1f;
        }
    }

    public override float GetScore()
    {
        // Calculate the metric, as a ratio of open choices to total choices
        // var totalChoices = openChoices + enclosedChoices;
        // if(inverse)
        //     return totalChoices > 0 ? (float)enclosedChoices / totalChoices : 0;
        // return totalChoices > 0 ? (float)openChoices / totalChoices : 0;
        return score;
    }

    void OnDisable()
    {
        // Output the openness preference score
  //      Debug.Log(enclosedChoices);
  //      Debug.Log(openChoices);
     //   Debug.Log("Openness Preference Score: " + GetScore());
    }
}
