using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Anticipation : ObjectMetric
{
    // Start is called before the first frame update
    private float score = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void evaluateObj(GameObject obj)
    {

        score = 0f;
       // var angle = Vector3.Angle(this.transform.position, gameObject.transform.position);
       if(obj.transform.gameObject.name.Contains("Terrain") || obj.transform.gameObject.name.Contains("Agent") || obj.transform.gameObject.activeSelf == false || obj.transform.gameObject == null)
           return;
        //calculate the penumbra and umbra sizes of the object
        // var penumbra = obj.transform.localScale.x * Mathf.Tan(angle);
        // var umbra = obj.transform.localScale.x * Mathf.Tan(angle);
        
        //normalise penumbra and umbra sizes against the agent's field of view
        float distanceToAgent = Vector3.Distance(obj.transform.position, gameObject.transform.position);
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
        float objectArea = Mathf.PI * Mathf.Pow(obj.transform.localScale.x, 2);
        float penumbraNormalised = penumbraArea / objectArea;
        float umbraNormalised = umbraArea / objectArea;
        
        //calculate the score
        score = penumbraNormalised - umbraNormalised;
        
        



        //throw new System.NotImplementedException();
    }

    public override float getScore()
    {
        //return normalised value
        return score;
    }
}
