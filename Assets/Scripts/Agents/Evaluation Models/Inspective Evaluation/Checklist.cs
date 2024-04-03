using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Checklist : MonoBehaviour
{
    public Camera agentCamera; // Assign the agent's camera
    public List<GameObject> allObjectsInScene; // List of all objects in the scene
    public float inspectionDistanceThreshold; // Threshold distance for inspection

    private HashSet<GameObject> seenObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, float> closestDistances = new Dictionary<GameObject, float>();
    
    private void Start()
    {
        updateObjects();
    }

    public void updateObjects()
    {
        allObjectsInScene = GameObject.FindObjectsOfType<GameObject>().ToList();
        //remove inactive objects
        //remove agent
        // allObjectsInScene.RemoveAll(obj => obj.activeInHierarchy == false);

        allObjectsInScene.RemoveAll(obj => obj.name.Contains("Agent"));
        allObjectsInScene.RemoveAll(obj => obj.name.Contains("Wall"));
        allObjectsInScene.RemoveAll(obj => obj.name.Contains("Recorders"));
        allObjectsInScene.RemoveAll(obj => obj.name.Contains("Terrain"));
        allObjectsInScene.RemoveAll(obj => obj.name.Contains("Directional Light"));
        allObjectsInScene.RemoveAll(obj => obj.name.Contains("Generator"));
        //remove all inactive objects

    }

    void FixedUpdate()
    {
        updateObjects();
        allObjectsInScene.RemoveAll(obj => obj == null);

        UpdateSeenObjects();
        //every 2 seconds print the inspection score
        // if (Time.frameCount % 120 == 0)
        // {
        //     Debug.Log(CalculateInspectionScore());
        // }
    }

    void UpdateSeenObjects()
    {
        foreach (var obj in allObjectsInScene)
        {
            if (IsObjectInCameraFrustum(obj))
            {
                seenObjects.Add(obj);
                UpdateClosestDistance(obj);
            }
        }
    }
    
    public void resetCheckList()
    {
        seenObjects.Clear();
        closestDistances.Clear();
        allObjectsInScene.Clear();
    }

    bool IsObjectInCameraFrustum(GameObject obj)
    {
        Vector3 viewportPoint = agentCamera.WorldToViewportPoint(obj.transform.position);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    void UpdateClosestDistance(GameObject obj)
    {
        float currentDistance = Vector3.Distance(transform.position, obj.transform.position);
        if (!closestDistances.ContainsKey(obj) || currentDistance < closestDistances[obj])
        {
            closestDistances[obj] = currentDistance;
        }
    }

    public float CalculateInspectionScore()
    {
        int inspectedObjectCount = 0;
        foreach (var obj in seenObjects)
        {
            if (closestDistances.ContainsKey(obj) && closestDistances[obj] <= inspectionDistanceThreshold)
            {
                inspectedObjectCount++;
            }
        }

        return allObjectsInScene.Count > 0 ? ((float)inspectedObjectCount / allObjectsInScene.Count)*100f : 0f;
    }
}
