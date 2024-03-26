using UnityEngine;
using System.Collections.Generic;

public class NoveltyObjectEvaluator : MonoBehaviour
{

    // Set to store IDs of observed objects
    private HashSet<int> observedObjects = new HashSet<int>();

    // Angle for the camera's field of view
    public float fieldOfView = 110f;

    // Distance up to which the camera can see
    public float viewDistance = 20f;

    // Novelty score
    private int noveltyScore = 0;

    private void Update()
    {
        EvaluateNovelty();
    }

    // Evaluate the novelty of the objects in the agent's field of view
    private void EvaluateNovelty()
    {
        // Get objects in the agent's field of view
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);

        foreach (Collider hitCollider in hitColliders)
        {
            Vector3 directionToObject = (hitCollider.transform.position - transform.position).normalized;
                        // Check if the object is within the field of view
                if(Vector3.Angle(transform.forward, directionToObject) < fieldOfView) {


                // Perform a raycast to check if the object is actually visible (not occluded)
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToObject.normalized, out hit, viewDistance))
                {
                    if (hit.collider.gameObject == hitCollider.gameObject)
                    {
                        // Check for novelty
                        int objectID = hitCollider.gameObject.GetInstanceID();
                        if (!observedObjects.Contains(objectID))
                        {
                            observedObjects.Add(objectID);
                            noveltyScore++;  // Increase the novelty score
                            Debug.Log("Novel object detected: " + hitCollider.gameObject.name);
                            Debug.Log(noveltyScore);
                        }
                    }
                }
            }
        }
    }

    // Get the total number of unique objects observed by the agent
    public int GetTotalUniqueObjectsObserved()
    {
        return observedObjects.Count;
    }
}
