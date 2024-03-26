using System;
using System.Collections.Generic;
using UnityEngine;

public class LargeObjectDetection : ObjectMetric
//TODO: Implement this class
    {
        private List<GameObject> objectsInView;
        private AnotherAgentController ag;
        private float score;
        private GameObject largestObjectSeen = null;

        private void Start()
        {
            ag = GetComponent<AnotherAgentController>();
        }

        public override float getScore()
        {
            return score;
        }

        public override void evaluateObj(GameObject obj)
        {
            //Check size of object against all objects in view, if it is 2 standard deviations larger than the average, add to score
            score = 0f;
            //Get all objects in view
            objectsInView = ag.objectsInView;
            
            //get largest object in view
            foreach (var objInView in objectsInView)
            {
                if (largestObjectSeen == null)
                {
                    largestObjectSeen = objInView;
                }
                else if (objInView.transform.localScale.x > largestObjectSeen.transform.localScale.x)
                {
                    largestObjectSeen = objInView;
                }
            }
            
            //compare object to largest object, the score is the percentage of the largest object's size that the object is
            score = obj.transform.localScale.x / largestObjectSeen.transform.localScale.x;
            
            
            // //Get average size of objects in view
            // float averageSize = 0f;
            // foreach (var objInView in objectsInView)
            // {
            //     averageSize += objInView.transform.localScale.x;
            // }
            // averageSize /= objectsInView.Count;
            // //Get standard deviation of objects in view
            // float standardDeviation = 0f;
            // foreach (var objInView in objectsInView)
            // {
            //     standardDeviation += Mathf.Pow(objInView.transform.localScale.x - averageSize, 2);
            // }
            // standardDeviation /= objectsInView.Count;
            // standardDeviation = Mathf.Sqrt(standardDeviation);
            // //Check if object is 2 standard deviations larger than average
            // if (obj.transform.localScale.x > averageSize + 2 * standardDeviation)
            // {
            //     score += 1f;
            // }
            
        }
    }
