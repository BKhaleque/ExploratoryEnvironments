using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class GenerateRandomEnvironment : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3[] vertices;
    private float curMaxY = float.MaxValue;
    private float curMinY = float.MinValue;
    public Mesh meshToRender;
    private NavMeshSurface surface;
    GameObject [] allObjects;
    MeshFilter meshFilter;
    private TestPS forestSpawner;

    private int score;

    private float timePassed = 0;


    //public GameObject emptyHolder;
    public GameObject exploratoryAgent;
    private AssetPS smallAssetSpawner;
    public GameObject[] assetsToSpawn;

    void Awake()
    {
        smallAssetSpawner = GetComponent<AssetPS>();
        forestSpawner = FindObjectOfType<TestPS>();
        //Random.InitState(0);
        spawnMesh();
    }
    
    void spawnMesh()
    {
        meshToRender = new Mesh();
        meshToRender = new Mesh();
        surface = GetComponent<NavMeshSurface>();
        var mesh = new Environment_Genome();
        smallAssetSpawner.hasSpawned = false;
        forestSpawner.hasSpawned = false;
        mesh.totalNumOfHighPoints = Random.Range(0, 50);
        mesh.totalNumOfMidPoints = Random.Range(0, 50);
        mesh.totalXSize = 350;
        mesh.totalZSize = 350;
        mesh.midValueModifier = Random.Range(0f, 10f);
        mesh.highValueModifier = Random.Range(0f, 10f);
        mesh.hillsProp = Random.Range(0f, 1f);
        mesh.areaOfInfluence = Random.Range(0f, 40);
        DrawMesh(mesh);
        GetComponent<MeshFilter>().mesh = meshToRender;
        meshFilter = GetComponent<MeshFilter>();
        surface.BuildNavMesh();
        SpawnAssets(mesh);
        GameObject.Instantiate(exploratoryAgent, new Vector3(50, 1, 50), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        timePassed = timePassed + Time.deltaTime;
        //after 3 minutes, destroy the mesh, all spawned assets and the agent and regenerate the environment and agent
        if (!(timePassed > 180)) return;
        //get agent coverage of the environment
        CoverageRecorder cv = exploratoryAgent.GetComponent<CoverageRecorder>();
        cv.InitializeGrid();
        //reset meshfilter and meshcollider
        meshFilter.mesh = null;
        GetComponent<MeshCollider>().sharedMesh = null;
        //destroy all objects except the environment generator and directional light
        allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if(obj.name != "Directional Light" && obj.name != "Generator")
                Destroy(obj);
        }
        spawnMesh();
        timePassed = 0;
    }

    // public float GetCoverage()
    // {
    //     //get the agent coverage of the environment
    //     var coverage = 0f;
    //     
    //     var agentPos = exploratoryAgent.transform.position;
    //     var agentX = (int)agentPos.x;
    //     var agentZ = (int)agentPos.z;
    //     var agentY = (int)agentPos.y;
    //     var totalPoints = 0;
    //     var coveredPoints = 0;
    //     for (var z = 0; z < 350; z++)
    //     {
    //         for (var x = 0; x < 350; x++)
    //         {
    //             var point = new Vector3(x, 0, z);
    //             var dist = Vector3.Distance(agentPos, point);
    //             if (dist < 10)
    //             {
    //                 totalPoints++;
    //                 if (agentY - vertices[z * 350 + x].y < 1)
    //                 {
    //                     coveredPoints++;
    //                 }
    //             }
    //         }
    //     }
    // }
    private void DrawMesh(Environment_Genome mesh)
        {
            var flag = 0;


            vertices = new Vector3[(mesh.totalXSize + 1) * (mesh.totalZSize + 1)];

            curMaxY = float.MinValue;
            curMinY = float.MaxValue;

            var midPoints = new Vector3[mesh.totalNumOfMidPoints];
            var highPoints = new Vector3[mesh.totalNumOfHighPoints];
            var midElevation = new float[mesh.totalNumOfMidPoints];
            var highElevation = new float[mesh.totalNumOfHighPoints];
            for (var i = 0; i < mesh.totalNumOfMidPoints; i++)
            {
                midPoints[i] = new Vector3(Random.value * mesh.totalXSize, 0f,
                    Random.value * mesh.totalZSize);
                if (Random.value > mesh.hillsProp)
                {
                    midElevation[i] = -mesh.midValueModifier;
                }
                else
                {
                    midElevation[i] = mesh.midValueModifier;
                }
            }
            //Debug.Log($"Points generated: {midPoints[i]}");
            
                    
            for (var i = 0; i < mesh.totalNumOfHighPoints; i++)
            {
                highPoints[i] = new Vector3(Random.value * mesh.totalXSize, 0f , Random.value*mesh.totalZSize);
                if (Random.value > mesh.hillsProp)
                {
                    highElevation[i] = -mesh.highValueModifier;
                }
                else
                {
                    highElevation[i] = mesh.highValueModifier;
                }
            }
            for (var z = 0; z <= mesh.totalZSize; z++)
            {

                for (var x = 0; x <= mesh.totalXSize; x++)
                {
                    float y = 0;
                    //adjust y value according to the current sector
                    var midDist = float.MaxValue;
                    var highDist = float.MaxValue;
                    var targetMidElevation = 0f; 
                    var targetHeightElevation = 0f; 
                    var curPoint = new Vector3(x, 0f, z);
                        
                    for (var j = 0; j < mesh.totalNumOfMidPoints; j++)
                    {
                        var dist = Math.Abs(Vector3.Distance(curPoint, midPoints[j]));//(midPoints[j] - curPoint).magnitude;
                        if (!(dist < midDist)) continue;
                        midDist = dist;
                        targetMidElevation = midElevation[j];

                    }
                    for (var j = 0; j < mesh.totalNumOfHighPoints; j++)
                    { 
                        var dist = Math.Abs(Vector3.Distance(curPoint, highPoints[j]));//(highPoints[j] - curPoint).magnitude;
                        if (!(dist < highDist)) continue;
                        highDist = dist;
                        targetHeightElevation = highElevation[j];
                    }

                    if (midDist < highDist)
                    {
                        //use the mid stuff
                        y = targetMidElevation * (Math.Max(0f, mesh.areaOfInfluence - midDist)/mesh.areaOfInfluence);
                    }
                    else
                    {
                        // use the higher stuff
                        y = targetHeightElevation * (Math.Max(0f, mesh.areaOfInfluence - highDist)/mesh.areaOfInfluence);
                    }

                    if (y > curMaxY)
                    {
                        curMaxY = y;
                    }
                    if (y < curMinY)
                    {
                        curMinY = y;
                    }
                    vertices[flag] = new Vector3(x, y, z);
                    flag++;
                }
                
            }
            
            var triangles = CreateTriangles(mesh);
            UpdateMesh(vertices, triangles);
            //surface.BuildNavMesh();
            allObjects = FindObjectsOfType<GameObject>() ;

        }
    
    private int[] CreateTriangles(Environment_Genome mesh)
    {
        var triangles = new int[mesh.totalXSize * mesh.totalZSize * 6];
        var vert = 0;
        var tris = 0;
        for (var z = 0; z<mesh.totalZSize; z++)
        {
            for (var  x = 0; x < mesh.totalXSize; x++)
            {
        
                triangles[tris] = vert;
                triangles[1 + tris] = vert + mesh.totalXSize + 1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + mesh.totalXSize + 1;
                triangles[5 + tris] = vert + mesh.totalXSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

        return triangles;
        
    }

    private void UpdateMesh(Vector3[] vertices, int[] triangles)
    {
        meshToRender.Clear();
        meshToRender.indexFormat = IndexFormat.UInt32;
        //MeshRenderer.material.color = Color.white;
        meshToRender.vertices = vertices;
        meshToRender.triangles = triangles;
        meshToRender.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = meshToRender;
        
        //Debug.Log("Mesh Rendered");
    }
    private void SpawnAssets(Environment_Genome mesh)
    {
        mesh.poissonDiscParams = new PoissonDiscParams(Random.Range(7, 12),350,Random.Range(30, 350),Random.Range(0, 200),assetsToSpawn, Random.Range(0, 350), 350, Random.Range(7, 12));

        forestSpawner.radius = mesh.poissonDiscParams.forestRadius;
        forestSpawner.regionSize.y = mesh.poissonDiscParams.forestYSize;
        forestSpawner.regionSize.x = mesh.poissonDiscParams.forestXSize;
        smallAssetSpawner.radius = mesh.poissonDiscParams.assetXSpread;
        smallAssetSpawner.regionSize.y = mesh.poissonDiscParams.assetYSpread;
        smallAssetSpawner.regionSize.x = mesh.poissonDiscParams.assetZSpread;
        //smallAssetSpawner.numAssetsToSpawn = mesh.poissonDiscParams.numOfAssetsToSpawn;
        //smallAssetSpawner.Assets = assetsToSpawn;
        // landmarkSpawner.assetXSpread = mesh.poissonDiscParams.assetXSpread;
        // landmarkSpawner.assetZSpread = mesh.poissonDiscParams.assetZSpread;
        // landmarkSpawner.assetYSpread = mesh.poissonDiscParams.assetYSpread;
       
        // for (var i = 0; i < smallAssetSpawner.numAssetsToSpawn; i++)
        // {
        //     smallAssetSpawner.SpreadAssets();
        // }
        //
        // smallAssetSpawner.hasSpawned = true;
        //
        // for (var i = 0; i < landmarkSpawner.numAssetsToSpawn; i++)
        // {
        //     landmarkSpawner.SpreadAssets();
        // }
        // landmarkSpawner.hasSpawned = true;
        
        forestSpawner.spawnTrees();
        smallAssetSpawner.spawnTrees();
      //  Debug.Log(forestSpawner.radius);
       // Debug.Log(smallAssetSpawner.radius);

    }
    public float GetMaxY()
    {
        return curMaxY;
    }
    public float GetMinY()
    {
        return curMinY;
    }
}
