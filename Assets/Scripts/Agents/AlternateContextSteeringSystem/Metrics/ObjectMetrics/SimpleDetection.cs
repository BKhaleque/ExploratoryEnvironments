using UnityEngine;

    public class SimpleDetection: ObjectMetric
    {
        private float score = 0;

        public override float getScore()
        {
            return score;
        }

        public override void evaluateObj(GameObject obj)
        {
            //The closer the object is the more it scores
            // if (obj == null)
            // {
            //     return;
            // }
            score = 0f;
           // score += 1f / Vector3.Distance(obj.transform.position, transform.position);
            score += 1f;
            return;
        }
    }
