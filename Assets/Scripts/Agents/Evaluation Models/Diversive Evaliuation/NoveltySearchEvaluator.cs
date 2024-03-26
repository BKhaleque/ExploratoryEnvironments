using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NoveltySearchEvaluator : MonoBehaviour
{
    public readonly Dictionary<Vector3, float> PositionObjectViews = new Dictionary<Vector3, float>();
    //private Dictionary<GameObject, float> objectsSeen = new Dictionary<GameObject, float>();
    private readonly Dictionary<String, float> TypesSeen = new();

    public Camera cam;
    public float lengthOfView = 80f;
    private float decayRate = 0.01f; // Decay rate for novelty scores
    private float initialNoveltyScore = 0.1f; // Initial novelty score for a new object
    private float repeatViewScore = 0.08f; // Score for seeing an object again immediately
   // private float overallNoveltyScore = 0f; // Overall novelty score for the agent
   private List<GameObject> objectsInView = new List<GameObject>();
   private List<GameObject> objs = new List<GameObject>();
   private Dictionary<GameObject,String> objectTypeDict = new Dictionary<GameObject, string>();
   public float timePassed = 0f;

    // Call this method when the agent sees objects
    private void Start()
    {
        objs = FindObjectsOfType<GameObject>().ToList();
        //initialise positionObjectViews by splitting the map into a grid
        //get the size of the map
        var terrain = GameObject.Find("Terrain");
        var terrainSize = 350f;
        var gridSize = 7;
        var gridX = 350 / gridSize;
        var gridZ = 350 / gridSize;
        //initialise the grid
        for (float x = 0; x < terrainSize; x += gridX)
        {
            for (float z = 0; z < terrainSize; z += gridZ)
            {
                PositionObjectViews.Add(new Vector3(x, 0, z), 0f);
            }
        }
        //classify objects into types based on tag and add to dictionary
        foreach (var obj in objs)
        {
            objectTypeDict.Add(obj,obj.tag);
        }
        

    }
    public void FixedUpdate()
    {
        // timePassed += Time.deltaTime;
        // if (!(timePassed >= 2f)) return;
        UpdateFieldOfView();
        recordPositionObjectView();
    }
    private bool ObjectSeen(GameObject obj)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return obj.GetComponent<Collider>() != null && GeometryUtility.TestPlanesAABB(planes , obj.GetComponent<Collider>().bounds);
    }
    void UpdateFieldOfView()
    {
        // Update objectsInView based on the camera's FOV
        //localObjs = Physics.OverlapSphere(transform.position,100f);

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
        foreach (var obj in objs.Where(obj => !obj.gameObject.name.Contains("Terrain") && !obj.gameObject.name.Contains("Wall") && !obj.gameObject.name.Contains("Agent") && obj.gameObject.activeSelf).Where(obj => !objectsInView.Contains(obj.gameObject)))
        {
            if(ObjectSeen(obj.gameObject))
                objectsInView.Add(obj.gameObject);
            if(Vector3.Distance(obj.transform.position, transform.position) > lengthOfView) continue;
        }
    }
    private void recordPositionObjectView()
    {
        //get all objects in camera view
        var noveltyScore = 0f;
        //check if object is in dictionary
        foreach (var obj in objectsInView)
        {
            if (!TypesSeen.ContainsKey(objectTypeDict[obj]))
            {

                    TypesSeen.Add(obj.tag, Time.time);
                    // if(TypesSeen.ContainsKey(objectTypeDict[obj]))
                    //     TypesSeen[objectTypeDict[obj]] += initialNoveltyScore;
                    // else
                    //     TypesSeen.Add(objectTypeDict[obj],initialNoveltyScore);
                    noveltyScore = initialNoveltyScore;
            }
        }
        
        foreach (var obj in objectsInView)
        {
            if (TypesSeen.ContainsKey(objectTypeDict[obj]))
            {
                var timeSinceLastSeen = Time.time - TypesSeen[obj.tag];
                TypesSeen[obj.tag] = timeSinceLastSeen;
                noveltyScore += 0 + (timeSinceLastSeen * decayRate);
                if(noveltyScore > initialNoveltyScore)
                    noveltyScore = initialNoveltyScore;
            }
        }
        

        var closestPosition = Vector3.zero;
        var closestDistance = Mathf.Infinity;
        foreach (var position in PositionObjectViews.Keys)
        {
            var distance = Vector3.Distance(position, transform.position);
            if (!(distance < closestDistance)) continue;
            closestDistance = distance;
            closestPosition = position;
        }
        //add novelty score to closest position
        PositionObjectViews[closestPosition] += noveltyScore;
        
    }





    public void resetNoveltySearchEvaluator()
    {
        PositionObjectViews.Clear();
        TypesSeen.Clear();
        var terrain = GameObject.Find("Terrain");
        var terrainSize = 350f;
        var gridSize = 7;
        var gridX = terrainSize / gridSize;
        var gridZ = terrainSize / gridSize;
        for (float x = 0; x < terrainSize; x += gridX)
        {
            for (float z = 0; z < terrainSize; z += gridZ)
            {
                PositionObjectViews.Add(new Vector3(x, 0, z), 0f);
            }
        }
    }

}
