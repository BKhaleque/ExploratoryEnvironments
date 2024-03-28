using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class AnotherAgentController : MonoBehaviour
{
    public Camera agentCamera;
    public NavMeshAgent navMeshAgent;
    public List<GameObject> objectsInView = new();
    public List<GameObject> visitedObjects = new();
    public float lengthOfView = 80f;
    private float rotationSpeed = 15.0f;
    
    public bool randomInertia = false;

    public List<ObjectMetric> ObjectMetrics = new();
    public List<DirectionMetric> DirectionMetrics = new();
    Dictionary<Vector3, float> interestMap = new();
    private float timePassed = 0f;

    private Collider[] localObjs;
    
    public bool makeObjectsGoThrough = false;
    GameObject terrain;
    
    private GameObject[] objs;
    private float stuckTimer;

    private Dictionary<Vector3,int > visitedPositions = new Dictionary<Vector3, int>();
    private Vector3 movementDirection = Vector3.forward;
    private void Start()
    {
        objs = UnityEngine.Object.FindObjectsOfType<GameObject>();

        if (!makeObjectsGoThrough)
        {


            foreach (var obj in objs)
            {
                if (obj.name.Contains("Terrain"))
                {
                    terrain = obj;
                    continue;

                }

                if (obj.name.Contains("Agent"))
                {
                    continue;
                }

                if (obj.name.Contains("Wall"))
                {
                    continue;
                }

                var col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    if (obj.GetComponent<Collider>().GetType() == typeof(MeshCollider))
                    {
                        obj.GetComponent<MeshCollider>().convex = false;
                        continue;
                    }

                    col.isTrigger = false;

                }
            }
        }
        else
        {
            foreach (var obj in objs)
            {
                if (obj.name.Contains("Terrain"))
                {
                    terrain = obj;
                    continue;

                }

                if (obj.name.Contains("Agent"))
                {
                    continue;
                }

                if (obj.name.Contains("Wall"))
                {
                    continue;
                }

                var col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    if (obj.GetComponent<Collider>().GetType() == typeof(MeshCollider))
                    {
                        obj.GetComponent<MeshCollider>().convex = true;
                        continue;
                    }

                    col.isTrigger = true;

                }
            }
        }
    }

    private Dictionary<Vector3, float> NormaliseInterestMap()
    {
        //init new interest map
        var normalisedInterestMap = new Dictionary<Vector3, float>();
        //get interest map max value
        var max = interestMap.Values.Max();
        //get interest map min value
        var min = interestMap.Values.Min();
        foreach (var val in interestMap)
        {
            //normalise each value and put into new interest map
            normalisedInterestMap.Add(val.Key, Range(val.Value, min, max));
            //normalisedInterestMap = Range(val.Value, min, max);
        }

        return normalisedInterestMap;
    }
    
    private float Range(float val, float min, float max)
        =>  min + val * (max - min);

    private void FixedUpdate()
        {
            if (!visitedPositions.Keys.Contains(transform.position))
            {
                visitedPositions.Add(transform.position, 1);
            }
            else
            {
                visitedPositions[transform.position] += 1;
            }

                //visitedPositions.Add(transform.position);
            timePassed += Time.deltaTime;
            // Debug.Log(timePassed % 2f);
            UpdateFieldOfView();
            if (timePassed >= 2f)
            {
                movementDirection = DetermineMovementDirection();
                timePassed = 0f;
            }
            if (movementDirection != Vector3.zero)
            {
                //Debug.Log("We are moving");
                navMeshAgent.SetDestination(transform.position + movementDirection);
            }
            else if(movementDirection == Vector3.zero)
            {
                // Debug.Log("We are looking around");
                LookAround();
                stuckTimer += Time.deltaTime;
            }
            
            //check if we are not moving
            if (navMeshAgent.velocity.magnitude < 0.1f)
            {
                stuckTimer += Time.deltaTime;
            }
            
            if (stuckTimer > 3f)
            {
                stuckTimer = 0f;
                //pick a random direction and move 10 steps in that direction
                var randomDirection = UnityEngine.Random.Range(0, 360);
                var direction = Quaternion.AngleAxis(randomDirection, Vector3.up) * transform.forward;
                navMeshAgent.SetDestination(transform.position + direction * 10f);
            }
            //Check if we come within 10 units of any objects in view then add the, to the visited objects array, if they don't already exist
            foreach (var obj in objectsInView.Where(obj => Vector3.Distance(transform.position, obj.transform.position) <= 10f).Where(obj => !visitedObjects.Contains(obj)))
            {
                if (visitedObjects.Contains(obj)) 
                    continue;
                visitedObjects.Add(obj);
                if (!objectsInView.Contains(obj)) continue;
                objectsInView.Remove(obj);
                return;
            }
        
        }

        private bool ObjectSeen(GameObject obj)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(agentCamera);
            return obj.GetComponent<Collider>() != null && GeometryUtility.TestPlanesAABB(planes , obj.GetComponent<Collider>().bounds);
        }
        void UpdateFieldOfView()
        {
            // Update objectsInView based on the camera's FOV
            //localObjs = Physics.OverlapSphere(transform.position,100f);
            objs = UnityEngine.Object.FindObjectsOfType<GameObject>();

            // if (timePassed % 5f <= 0.2f)
            //{
            objectsInView = new List<GameObject>();
            //  Debug.Log("Updating objects in view");
            // }
            // RaycastHit hit;
            // var rayDirection = agentCamera.transform.forward;
            // var rayOrigin = agentCamera.transform.position;
            //float rayLength = 100f;

            // if (!Physics.Raycast(rayOrigin, rayDirection, out hit)) return;
            // if (hit.collider.gameObject.name.Contains("Terrain")) return;
            objs = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach (var obj in objs)
            {
                if (obj.gameObject.name.Contains("Terrain") || obj.gameObject.name.Contains("Wall") || obj.gameObject.name.Contains("Agent") || !obj.gameObject.activeSelf) continue;
                if(objectsInView.Contains(obj.gameObject)) continue;
                if(visitedObjects.Contains(obj.gameObject)) continue;
                //if object is too far it does not count as in view
                if(Vector3.Distance(obj.transform.position, transform.position) > lengthOfView) continue;
                if(ObjectSeen(obj.gameObject))
                    objectsInView.Add(obj.gameObject);
            }

        }

        Vector3 DetermineMovementDirection()
        {
            // Process what is in view and determine the best direction to move in
            var directionInterestMap = CalculateDirectionInterestMap(objectsInView);
            return FindHighestInterestDirection(directionInterestMap);
        }

        private void LookAround()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        Dictionary<Vector3, float> CalculateDirectionInterestMap(List<GameObject> objects)
        {
            var directions = 0;
            //  Sample directions around the agent that are in the camera frustrum, we can use the metrics to calculate the interest value for each direction
            interestMap = new Dictionary<Vector3, float>();
            // Initialize the interest map make sure the directions are in the camera frustrum (We want 36 directions)
            for (var i = -17; i < 36; i++)
            {
                var angle = i * 5f;
                // var direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
                if (Vector3.Angle(agentCamera.transform.forward, Quaternion.AngleAxis(angle, Vector3.up) * agentCamera.transform.forward) > agentCamera.fieldOfView) continue;
                directions++;
                var direction = Quaternion.AngleAxis(angle, Vector3.up) * agentCamera.transform.forward;
                //Debug.DrawRay(transform.position, direction);

                //Debug.DrawRay(transform.position, direction * 10f, Color.red);
                if(interestMap.ContainsKey(direction)) continue;
                if (visitedPositions.Keys.Contains(transform.position + direction))
                {
                    //apply a penalty to the direction
                    interestMap.Add(direction, -0.1f * visitedPositions[transform.position + direction]);
                }
                else
                    interestMap.Add(direction, 0f);
            }
            //Calculate the interest value for each direction using direction based metrics
            foreach (var dir in interestMap.Keys.ToList())
            {
                foreach (var metric in DirectionMetrics)
                {
                    metric.EvaluateDecision(dir);
                    interestMap[dir] += metric.GetScore() * metric.weight;
                }
            }
          //  Debug.Log(directions);
        
            //Object focused metric logic
            foreach (var obj in objects)
            {
                if (visitedObjects.Contains(obj)) continue;
                var directionToObj = (obj.transform.position - transform.position).normalized;
                //direction to obj should be rounded to the closest of the directions in the array
                var closestDir = interestMap.Keys.First();
                foreach (var dir in interestMap.Keys.Where(dir => Vector3.Angle(directionToObj, dir) < Vector3.Angle(closestDir, dir)))
                {
                    closestDir = dir;
                }
                directionToObj = closestDir;
                

                
                
                // Debug.Log(closestDir);
                var interestValue = CalculateInterestValueForObject(obj);
                // Debug.Log(interestValue);
                //Todo apply a normal curve to the interest value based on close directions to the direction to the object
                
                //find close directions to the objects direction
                
                var closeDirections = new List<Vector3>();
                foreach (var dir in interestMap.Keys.Where(dir => Vector3.Angle(directionToObj, dir) < 40f))
                {
                    closeDirections.Add(dir);
                }
                interestMap[directionToObj] += interestValue;
                var i = 0.9f;
                foreach (var dir in closeDirections)
                {
                    interestMap[dir] += interestValue * i;
                    i -=0.1f;
                }
            }
            return NormaliseInterestMap();
            //return interestMap;
        }

        private float CalculateInterestValueForObject(GameObject obj)
        {
            // Implement logic to calculate interest value for each object using the attached metrics and their weights TODO
            // Can use anticipation, group detection
            var score = 0f;
            foreach (var metric in ObjectMetrics)
            {
                metric.evaluateObj(obj);
                score += metric.getScore() * metric.weight;
            }
        
            return score; // Placeholder value
        }

        private Vector3 FindHighestInterestDirection(Dictionary<Vector3, float> interestMap)
        {
            var highestInterestDirections = new List<Vector3>();
            // Find the direction with the highest interest value
            var highestInterestDirection = Vector3.zero;
            var highestInterestValue = 0f;
            //NormaliseInterestMap();

            foreach (var entry in interestMap.Where(entry => entry.Value > highestInterestValue))
            {
                highestInterestValue = entry.Value;
                highestInterestDirection = entry.Key;
            }
            
            //loop through again and pick out directions with same interest value

            foreach (var entry in interestMap.Where(entry => entry.Value == highestInterestValue))
            {
                highestInterestDirections.Add(entry.Key);
            }

            // Debug.Log(highestInterestDirection);
            if (highestInterestDirections.Count <= 1) return highestInterestDirection;
            //return the direction which is closest to the current direction
            return randomInertia ? highestInterestDirections[UnityEngine.Random.Range(0, highestInterestDirections.Count)] : highestInterestDirections.OrderBy(dir => Vector3.Angle(dir, navMeshAgent.velocity)).First();

        }
}
