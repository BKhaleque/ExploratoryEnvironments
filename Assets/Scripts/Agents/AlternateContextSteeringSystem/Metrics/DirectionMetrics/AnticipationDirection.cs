using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnticipationDirection : DirectionMetric
{
    // Start is called before the first frame update
    float score = 0f;
    public float checkDistance = 80f;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void EvaluateDecision(Vector3 direction)
    {
        RaycastHit hit;
        score = 0f;
        if (Physics.Raycast(transform.position, direction, out hit, checkDistance))
        {
            
            var angle = Vector3.Angle(this.transform.position, hit.transform.position);

            if(hit.transform.gameObject.name.Contains("Terrain") || hit.transform.gameObject.name.Contains("Agent") || hit.transform.gameObject.activeSelf == false|| hit.transform.gameObject.name.Contains("Wall"))
                return;
            float distanceToAgent = Vector3.Distance(hit.transform.position, gameObject.transform.position);
            float agentHeight = gameObject.transform.localScale.y;
            float agentRadius = gameObject.transform.localScale.x;
            float umbraLength = (distanceToAgent * agentHeight) / agentHeight;

            float penumbraOuterLength = (distanceToAgent * (agentHeight + agentRadius)) / agentHeight;
            float penumbraInnerLength = (distanceToAgent * (agentHeight - agentRadius)) / agentHeight;
            float penumbraLength =  penumbraOuterLength - penumbraInnerLength;
        
            //get object radius
            float penumbraArea = Mathf.PI * Mathf.Pow(penumbraLength, 2);
            //calculate umbra area
            float umbraArea = Mathf.PI * Mathf.Pow(umbraLength, 2);

            //normalise the area of the penumbra and umbra by the area of the object
            float objectArea = Mathf.PI * Mathf.Pow(hit.transform.localScale.x, 2);
            float penumbraNormalised = penumbraArea / objectArea;
            float umbraNormalised = umbraArea / objectArea;
        
            //calculate the score
            score = penumbraNormalised - umbraNormalised;

            score += EvaluateDirectionAlignment(direction);
            score += EvaluateProximity(hit.distance);

        }
    }
    
    
    float EvaluateProximity(float distance)
    {
        if(inverse)
            return 1 / (checkDistance - distance);
        
        return 1 / distance;
    }
    
    float EvaluateDirectionAlignment(Vector3 direction)
    {
        var areaOfInterestDirection = Vector3.forward;
        var alignmentScore = Vector3.Dot(direction.normalized, areaOfInterestDirection.normalized);
        return alignmentScore;
    }

    public override float GetScore()
    {
        return score;
    }
}
