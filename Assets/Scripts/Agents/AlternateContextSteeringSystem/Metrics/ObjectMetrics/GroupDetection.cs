using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupDetection : ObjectMetric
{
    private float score = 0f;

    private Collider[] otherObjs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float getScore()
    {

        return score;
    }

    public override void evaluateObj(GameObject obj)
    {
        //Check for other objects within 5 units of the object and determine if it is in a group/pair
        //If it is in a group/pair, add to the score
        //
        score = 0f;
        otherObjs = Physics.OverlapSphere(obj.transform.position, 8f);
        //remove inactive objects
        foreach (var otherObj in otherObjs)
        {
            if (otherObj.gameObject == null)
                continue;
            if(otherObj.gameObject.transform.parent != null)
                continue;
            if(otherObj.gameObject.Equals(this.gameObject))
                continue;
            if (otherObj.gameObject != obj && !otherObj.gameObject.name.Contains("Terrain") && score < 1f)
            {
                score += 0.1f;
            }
        }
    }
}
