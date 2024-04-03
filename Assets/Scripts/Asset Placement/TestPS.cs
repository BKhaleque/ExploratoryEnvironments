using System.Collections.Generic;
using UnityEngine;

public class TestPS : MonoBehaviour
{
    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;
    public float displayRadius = 1;

    private float minY;
    private float maxY = 100f;
    public bool hasSpawned;

    [SerializeField]
   // private MeshGenerator meshGenerator;

    public GameObject prefTree;

    public List<Vector2> points;



    void Start()
    {
        //points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);

    }

    public void spawnTrees()
    {
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
    }
    private void LateUpdate()
    {
        if (hasSpawned)
        {

            return;

        }

        // if (!meshGenerator.IsReady())
        // {
        //     return;
        // }
        // if(!meshGenerator.story.Contains("forest"))
        //   return;

        DrawTrees();
        hasSpawned = true;
    }


    private void OnDrawGizmos()
    {

        var wire = new Vector3(regionSize.x, 0, regionSize.y);
        var center = new Vector3(regionSize.x / 2, 0, regionSize.y / 2);
        Gizmos.DrawWireCube(center, wire);


        if (points != null)
        {

            foreach (var point in points)
            {
                //Vector3 position = new Vector3(point.x, 0, point.y);
                //position.y = Terrain.activeTerrain.SampleHeight(position) + Terrain.activeTerrain.GetPosition().y;
                //Gizmos.DrawSphere(position, displayRadius);
            }
        }

    }
    public void DrawTrees()
    {

        if (points != null)
        {

            foreach (var point in points)
            {
                var ray = new Ray(new Vector3(point.x, maxY, point.y), Vector3.down);


                RaycastHit hit; //where our item was hit
                if (Physics.Raycast(ray, out hit))    //if it hits something, from moving down from transform.position, out is the returned value
                {
                    var spawnRotation = Quaternion.FromToRotation(Vector3.down, hit.normal);   //calculate the difference between the angle of the object hit and 'up'. this gives us a value to rotate a spawned item.
                                                                                                    //duplicate script ,or toggle on and off ,for some assets like trees?
                    var clone = Instantiate(prefTree, hit.point, spawnRotation); //spawn the item at the point where the raycast hit and the rotation of the ground.
                    //Destroy(gameObject);
                }
                //Vector3 position = new Vector3(point.x, 0, point.y);
                //position.y = Terrain.activeTerrain.SampleHeight(position) + Terrain.activeTerrain.GetPosition().y;
                //Instantiate(prefTree, position, Quaternion.identity);
            }
        }

    }
}
