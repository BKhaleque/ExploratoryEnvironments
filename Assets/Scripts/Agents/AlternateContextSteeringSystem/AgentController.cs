using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

namespace Agents.AlternateContextSteeringSystem
{
    public class AgentController : MonoBehaviour
    {
        public float distance;
        public float horizontalFov;
        public float verticalFov;

        //public float contextMapResolution = 10f;
        public NavMeshAgent navMeshAgent;
        //private Rigidbody rb;
        public float anticipationDetectionWeight;
        public float groupDetectionWeight;
        public float largeObjectDetectionWeight;
        public float simpleDetectionWeight;
        public float highestTerrainPointDetectionWeight;
        public float stepSize = 10.0f; // Angle step size
        public GameObject terrain;
        private Vector3[] vertices;
        public List<Vector3> visitedTargets;
        private float timePassed = 0f;
        Vector3 bestDirection = Vector3.zero;
        float bestValue = float.MinValue;
        // void Awa()
        // {
        //     rb = GetComponent<Rigidbody>();
        // }

        private void Update()
        {
            timePassed += Time.deltaTime;
        }

        void FixedUpdate()
        {
            Vector3 chosenDirection = EvaluateContextMap();
            MoveInDirection(chosenDirection);
            vertices = terrain.GetComponent<MeshFilter>().sharedMesh.vertices;

        }

        // ReSharper disable Unity.PerformanceAnalysis
        Vector3 EvaluateContextMap()
        {
            if (timePassed > 2)
            {
                timePassed = 0;
                bestDirection = Vector3.zero;
                bestValue = float.MinValue;


               // float halfFOV = fov * 0.5f;
                for (float azimuthal = 0; azimuthal < horizontalFov; azimuthal += stepSize)
                {
                    for (float polar = 0; polar <= verticalFov; polar += stepSize)
                    {
                        //Direction should be where it is facing
                        Vector3 direction = transform.rotation * SphericalToCartesian(azimuthal, polar) ;
                        var value = EvaluateDirection(direction);
                        Debug.DrawRay(transform.position, direction * distance);

                        if (value > bestValue)
                        {
                            bestValue = value;
                            bestDirection = direction;
                        }
                    }
                }

            }

            //Debug.Log(bestDirection);

            // if(Physics.Raycast(this.gameObject.transform.position, bestDirection, out var hit, distance)){
            //     Debug.Log(hit.collider.gameObject.name);
            // }
                return bestDirection;
            

        }

        Vector3 SphericalToCartesian(float azimuthal, float polar)
        {
            float azimuthalRad = azimuthal * Mathf.Deg2Rad;
            float polarRad = polar * Mathf.Deg2Rad;

            float x = Mathf.Sin(polarRad) * Mathf.Cos(azimuthalRad);
            float y = Mathf.Cos(polarRad);
            float z = Mathf.Sin(polarRad) * Mathf.Sin(azimuthalRad);

            return new Vector3(x, y, z);
        }


        float EvaluateDirection(Vector3 direction)
        {
            float value = 0;
            // Anticipation Detection (Detecting space around and behind an object)
            value += EvaluateAnticipation(direction) * anticipationDetectionWeight;
            // Group Detection
            value += EvaluateGroup(direction) * groupDetectionWeight;
            // Large Object Detection
            value += EvaluateLargeObject(direction) * largeObjectDetectionWeight;
            // Simple Detection
            value += EvaluateSimple(direction) * simpleDetectionWeight;

            // Highest Terrain Point Detection 
            value += EvaluateTerrainPoints(direction) * highestTerrainPointDetectionWeight;
            return value;
        }
        
        float EvaluateAnticipation(Vector3 direction)
        {
            float value = 0;
            //var dirToRayCastFrom = transform.rotation;
            if (Physics.Raycast(this.gameObject.transform.position, direction, out var hit, distance))
            {
                if (ObjectTooHigh(hit.collider.gameObject))
                {
                    return value;
                }
                if(Vector3.Distance(this.gameObject.transform.position, hit.collider.gameObject.transform.position) <10f){
                    if(!visitedTargets.Contains(hit.collider.gameObject.transform.position) && NotTooClose())
                        visitedTargets.Add(hit.collider.gameObject.transform.position);                    
                    return 0;
                }
                if(visitedTargets.Contains(hit.collider.gameObject.transform.position)){
                    return 0;
                }

                if (hit.collider.gameObject.name == "Terrain" && hit.collider.gameObject.name.Contains("Agent") &&
                    hit.collider.gameObject.name.Contains("Wall")) return value;
                //return 1;
                for (var x = -10; x<10; x++){
                    //cast a ray from agent to object (+i in x value)
                    //check if ray does not hit an object
                    Vector3 posToCheck;
                
                    if(x< 0){
                        posToCheck = hit.collider.gameObject.transform.position + new Vector3(-hit.collider.gameObject.GetComponent<Collider>().bounds.size.x + x, 0.0f, 0.0f); //check left side of obj
                    }
                    else{
                        posToCheck = hit.collider.gameObject.transform.position + new Vector3(hit.collider.gameObject.GetComponent<Collider>().bounds.size.x + x, 0.0f, 0.0f); //check right side of obj
                    }
                    //posToCheck *= -1;
                    //Debug.DrawRay(transform.position, Quaternion.Inverse(transform.rotation) * posToCheck);
                    //Debug.DrawRay(Vector3.zero, Quaternion.Inverse(transform.rotation) * posToCheck);
                    // Debug.DrawRay(this.transform.position, posToCheck);

                    if(!Physics.Raycast(this.transform.position, posToCheck, distance +3f)){
                        value+=0.05f;
                    }
                }
            }
            return value;

        }

        private float EvaluateGroup(Vector3 direction)
        {
            float value = 0;
            if (Physics.Raycast(this.gameObject.transform.position, direction, out var hit, distance))
            {
                if (ObjectTooHigh(hit.collider.gameObject))
                {
                    return value;
                }
                if(Vector3.Distance(this.gameObject.transform.position, hit.collider.gameObject.transform.position) <10f){
                    if(!visitedTargets.Contains(hit.collider.gameObject.transform.position) && NotTooClose())
                        visitedTargets.Add(hit.collider.gameObject.transform.position);
                    return 0;
                }
                if(visitedTargets.Contains(hit.collider.gameObject.transform.position)){
                    return 0;
                }
                if(hit.collider.gameObject.name != "Terrain" || !hit.collider.gameObject.name.Contains("Agent")|| !hit.collider.gameObject.name.Contains("Wall")){
                    var objsAroundObject = Physics.OverlapSphere(hit.collider.gameObject.transform.position, 5f);
                    for(int i = 0; i<objsAroundObject.Length; i++){
                        value +=0.1f;
                    }
                }

            }
            return value;
        }

        float EvaluateSimple(Vector3 direction)
        {
            if (Physics.Raycast(this.gameObject.transform.position, direction, out var hit, distance))
            {
                if (ObjectTooHigh(hit.collider.gameObject))
                {
                    return 0;
                }
                if(Vector3.Distance(this.gameObject.transform.position, hit.collider.gameObject.transform.position) <10f){
                    if(!visitedTargets.Contains(hit.collider.gameObject.transform.position) && NotTooClose())
                        visitedTargets.Add(hit.collider.gameObject.transform.position);
                    return 0;
                }
                if(visitedTargets.Contains(hit.collider.gameObject.transform.position)){
                    return 0;
                }
//                Debug.Log(hit.collider.gameObject.name);
                if(!hit.collider.gameObject.name.Contains("Terrain") || !hit.collider.gameObject.name.Contains("Agent")|| !hit.collider.gameObject.name.Contains("Wall"))
                    return 1;
            }
            return 0;
        }

        float EvaluateLargeObject(Vector3 direction){
            float value = 0f;
            var averageSize = 0f;
            if (Physics.Raycast(this.gameObject.transform.position, direction, out var hit, distance))
            {
                if (ObjectTooHigh(hit.collider.gameObject))
                {
                    return value;
                }
                if(Vector3.Distance(this.gameObject.transform.position, hit.collider.gameObject.transform.position) <10f){
                    if(!visitedTargets.Contains(hit.collider.gameObject.transform.position) && NotTooClose())
                        visitedTargets.Add(hit.collider.gameObject.transform.position);
                    return 0;
                }
                if(visitedTargets.Contains(hit.collider.gameObject.transform.position)){
                    return 0;
                }
//                Debug.Log(hit.collider.gameObject.name);
                if(hit.collider.gameObject.name != "Terrain" || !hit.collider.gameObject.name.Contains("Agent")|| !hit.collider.gameObject.name.Contains("Wall")){
                    //check around local area of object
                    var colliders = Physics.OverlapSphere(hit.collider.transform.position, 40f);
                    //calculate if object is greater than 2SDs above object sizes in local area (TODO)

                    foreach (var collider in colliders)
                    {
                        averageSize += collider.gameObject.GetComponent<Collider>().bounds.size.magnitude;
                    }
                    averageSize /= colliders.Length;
                    //Get standard deviation of all objects
                    var standardDeviation = 0f;
                    foreach (var collider in colliders)
                    {
                        standardDeviation += Mathf.Pow(collider.gameObject.GetComponent<Collider>().bounds.size.magnitude - averageSize, 2);
                    }
                    standardDeviation /= colliders.Length;
                    standardDeviation = Mathf.Sqrt(standardDeviation);
                    if (hit.collider.gameObject.GetComponent<Collider>().bounds.size.magnitude > averageSize + 2 * standardDeviation)
                    {
                        value +=1f;
                    }
                }
                   
            }
            return value;
        }


        float EvaluateTerrainPoints(Vector3 direction){
            var value = 0f;
            if (Physics.Raycast(this.gameObject.transform.position, direction, out var hit, distance))
            {
                // if(Vector3.Distance(this.gameObject.transform.position, hit.collider.gameObject.transform.position) <10f){
                //     visitedTargets.Add(hit.collider.gameObject.transform.position);
                //     return 0;
                // }
                // if(visitedTargets.Contains(hit.collider.gameObject.transform.position)){
                //     return 0;
                // }
                if (hit.collider.gameObject.name != "Terrain") return value;
                var bestVertex = Vector3.zero;
                //find the closest vertex 
                var closestDist = 100000000f;
                foreach(var vertex in vertices){
                    if (Vector3.Distance(hit.point, vertex) < 10f)
                    {
                        if(!visitedTargets.Contains(vertex) && NotTooClose())
                            visitedTargets.Add(vertex);
                    
                    }

                    if(Vector3.Distance(hit.point,vertex) < closestDist && Vector3.Distance(hit.point,vertex) > 10f ){
                        bestVertex = vertex;
                        closestDist = Vector3.Distance(hit.point,vertex);
                    }
                }

                //After we find the closest vertex measure it's height and give it a value
                value = Math.Abs(bestVertex.y - transform.position.y);
            }
            return value;
        }

        void MoveInDirection(Vector3 direction)
        {
             var targetPosition = transform.position + direction*5; // Move 5 units in the chosen direction
            navMeshAgent.SetDestination(targetPosition);
            //Debug.Log(direction);
            //rb.velocity = Vector3.zero;
            //rb.AddForce(direction);
        }

        bool NotTooClose()
        {
            //loop through all points in visitedTargets and check if the distance between the current pos and point is less than 10 if so return false
            foreach(var point in visitedTargets){
                if(Vector3.Distance(transform.position, point) < 10f)
                    return false;
            }

            return true;
        }

        bool ObjectTooHigh(GameObject obj)
        {
            //check object relative y to agent, if it is greater than 10 return true
            return Math.Abs(obj.transform.position.y - transform.position.y) > 10f;
        }
    }
}