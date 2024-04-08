using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
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

    private Environment_Genome[] InitialMeshes;
    private List<Environment_Genome> goodMeshes = new List<Environment_Genome>();
    private List<Environment_Genome> badMeshes = new List<Environment_Genome>();
    private int score;

    private float timePassed = 0;
    private int counter = 0;

    //public GameObject emptyHolder;
    public GameObject exploratoryAgent;
    private AssetPS smallAssetSpawner;
    public GameObject[] assetsToSpawn;

    void Awake()
    {
        //initialise 10 meshes
        InitialMeshes = new Environment_Genome[10];
        for (var i = 0; i < 10; i++)
        {
            InitialMeshes[i] = new Environment_Genome();
            //random parameters
            InitialMeshes[i].totalNumOfHighPoints = Random.Range(0, 50);
            InitialMeshes[i].totalNumOfMidPoints = Random.Range(0, 50);
            InitialMeshes[i].totalXSize = 350;
            InitialMeshes[i].totalZSize = 350;
            InitialMeshes[i].midValueModifier = Random.Range(0f, 10f);
            InitialMeshes[i].highValueModifier = Random.Range(0f, 10f);
            InitialMeshes[i].hillsProp = Random.Range(0f, 1f);
            InitialMeshes[i].areaOfInfluence = Random.Range(0f, 40f);
            InitialMeshes[i].poissonDiscParams = new PoissonDiscParams(Random.Range(7, 12),350,Random.Range(30, 350),Random.Range(0, 200),assetsToSpawn, Random.Range(0, 350), 350, Random.Range(7, 12));
        }
        smallAssetSpawner = GetComponent<AssetPS>();
        forestSpawner = FindObjectOfType<TestPS>();
        //Random.InitState(0);
        spawnMesh(InitialMeshes[0]);
    }
    
    void spawnMesh(Environment_Genome mesh)
    {
         meshToRender = new Mesh();
        surface = GetComponent<NavMeshSurface>();
        // var mesh = new Environment_Genome();
         smallAssetSpawner.hasSpawned = false;
         forestSpawner.hasSpawned = false;
        // mesh.totalNumOfHighPoints = Random.Range(0, 50);
        // mesh.totalNumOfMidPoints = Random.Range(0, 50);
        // mesh.totalXSize = 350;
        // mesh.totalZSize = 350;
        // mesh.midValueModifier = Random.Range(0f, 10f);
        // mesh.highValueModifier = Random.Range(0f, 10f);
        // mesh.hillsProp = Random.Range(0f, 1f);
        // mesh.areaOfInfluence = Random.Range(0f, 40);
        DrawMesh(mesh);
        GetComponent<MeshFilter>().mesh = meshToRender;
        meshFilter = GetComponent<MeshFilter>();
        surface.BuildNavMesh();
        SpawnAssets(mesh);
        GameObject.Instantiate(exploratoryAgent, new Vector3(50, 1, 50), Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed = timePassed + Time.deltaTime;
        //after 3 minutes, destroy the mesh, all spawned assets and the agent and regenerate the environment and agent
        if (!(timePassed > 20)) return;
        var cv = exploratoryAgent.GetComponent<CoverageRecorder>();
        var inspection = exploratoryAgent.GetComponent<Checklist>();
        //check if inspection is > 20%
        if (cv.GetExplorationPercentage() >= 5 && cv.GetExplorationPercentage()<= 80 && inspection.CalculateInspectionScore() >= 0.01f && inspection.CalculateInspectionScore() <= 0.9f)
        {
            goodMeshes.Add(InitialMeshes[counter]);
        }
        else
        {
            badMeshes.Add(InitialMeshes[counter]);
        }
        
        if (counter ==9)
        {
            Debug.Log("Generating new Pop");
            counter = 0;
            var newPop = new Environment_Genome[10];
    
            //Do crossover and mutation to create a new population of meshes until the population is full
            for (var i = 0; i < 10; i++)
            {
                //take 2 random meshes from the goodMeshes or badmeshes list and perform crossover
                Environment_Genome mesh1;
                Environment_Genome mesh2;
                //check if the lists are empty
                if (goodMeshes.Count < 2)
                {
                    //take 2 random meshes from the badMeshes list and perform crossover
                    mesh1 = badMeshes[Random.Range(0, badMeshes.Count)];
                    mesh2 = badMeshes[Random.Range(0, badMeshes.Count)];
                }
                else
                {
                    mesh1 = goodMeshes[Random.Range(0, goodMeshes.Count)];
                    mesh2 = goodMeshes[Random.Range(0, goodMeshes.Count)];
                }

                //make sure they are not the same
                while (mesh1.Equals(mesh2))
                {
                    mesh2 = goodMeshes[Random.Range(0, goodMeshes.Count)];
                }
    
                newPop[i] = crossOver(mesh1, mesh2);
            }

            InitialMeshes = newPop;
            badMeshes = new List<Environment_Genome>();
            goodMeshes = new List<Environment_Genome>();
        }
        Debug.Log(counter);
        counter++;
        //get agent coverage of the environment



        cv.InitializeGrid();
        inspection.resetCheckList();
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
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        spawnMesh(InitialMeshes[counter]);
        timePassed = 0;
    }

    private Environment_Genome crossOver(Environment_Genome mesh1, Environment_Genome mesh2)
    {
        
        var child = new Environment_Genome();
        //using a random chance pick a parameter from either mesh1 or mesh2
        var rand = Random.Range(0, 2);
        child.totalNumOfHighPoints = rand == 0 ? mesh1.totalNumOfHighPoints : mesh2.totalNumOfHighPoints;
        rand = Random.Range(0, 2);
        child.totalNumOfMidPoints = rand == 0 ? mesh1.totalNumOfMidPoints : mesh2.totalNumOfMidPoints;
        rand = Random.Range(0, 2);
        child.totalXSize = rand == 0 ? mesh1.totalXSize : mesh2.totalXSize;
        rand = Random.Range(0, 2);
        child.totalZSize = rand == 0 ? mesh1.totalZSize : mesh2.totalZSize;
        rand = Random.Range(0, 2);
        child.midValueModifier = rand == 0 ? mesh1.midValueModifier : mesh2.midValueModifier;
        rand = Random.Range(0, 2);
        child.highValueModifier = rand == 0 ? mesh1.highValueModifier : mesh2.highValueModifier;
        rand = Random.Range(0, 2);
        child.hillsProp = rand == 0 ? mesh1.hillsProp : mesh2.hillsProp;
        rand = Random.Range(0, 2);
        child.areaOfInfluence = rand == 0 ? mesh1.areaOfInfluence : mesh2.areaOfInfluence;
        rand = Random.Range(0, 2);
        child.poissonDiscParams = rand == 0 ? mesh1.poissonDiscParams : mesh2.poissonDiscParams;
        return child;
        
    }
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
