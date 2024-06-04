using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random; //using UnityEngine.UI;

namespace Evolutionary_Process
{
    [RequireComponent(typeof(MeshFilter))]
    public class Evaluate : MonoBehaviour
    {
        // private MeshFilter[] meshes;
        private Mesh meshToRender;
        private NavMeshSurface surface;
        public NavMeshAgent navMeshAgent;
        public GameObject validatorAgent;

        //public bool generate;

        public Camera cam;

        private float curMaxY = float.MinValue;
        private float curMinY = float.MaxValue;

        private AssetAreaSpawner smallAssetSpawner;

        private GameObject[] allObjects;

        private TestPS forestSpawner;
        private AssetAreaSpawner landmarkSpawner;

        private int score;

        public GameObject landmarkSpawnerObj;
        public GameObject treeSpawnerObj;
        public GameObject smallAssetSpawnerObj;

        public GameObject emptyHolder;

        private Vector3[] vertices;

        private Dictionary<Vector3,float> interestMeasureTable;

        //private List<Vector3> pathForExplorationAgentToFollow;
        //private float interestMeasure;

        private void Awake()
        {
            Random.InitState(0);
            meshToRender = new Mesh();
            GetComponent<MeshFilter>().mesh = meshToRender;
            
            smallAssetSpawner = GameObject.Find("AssetSpawner").GetComponent<AssetAreaSpawner>();
            landmarkSpawner = GameObject.Find("LandmarkSpawner").GetComponent<AssetAreaSpawner>();
            forestSpawner = FindObjectOfType<TestPS>();
            //interestMeasure = 0;
            surface = GetComponent<NavMeshSurface>();
            interestMeasureTable = new Dictionary<Vector3, float>();
            //agent = GetComponent<NavMeshAgent>();
        }

        public void EvaluateMesh(Environment_Genome mesh)
        {

            //vertices = new Vector3[(mesh.totalXSize + 1) * (mesh.totalZSize + 1)];
            // foreach (var go in allObjects)
            // {
            //     if (!go.CompareTag("DND"))
            //         Destroy(go);
            // }

            // Instantiate(landmarkSpawnerObj, Vector3.zero, Quaternion.identity);
            // Instantiate(smallAssetSpawnerObj, Vector3.zero, Quaternion.identity);
            // Instantiate(treeSpawnerObj, Vector3.zero, Quaternion.identity);
            // Instantiate(emptyHolder, Vector3.zero, Quaternion.identity);
            // forestSpawner = FindObjectOfType<TestPS>();
            // smallAssetSpawner = GameObject.Find("AssetSpawner(Clone)").GetComponent<AssetAreaSpawner>();
            // landmarkSpawner = GameObject.Find("LandmarkSpawner(Clone)").GetComponent<AssetAreaSpawner>();

            DrawMesh(mesh);
            mesh.score+= CalculateNavMesh(mesh.totalXSize, mesh.totalZSize);
            mesh.score += StandardDeviation(mesh.totalXSize,mesh.totalZSize);
            
            var filePath = GETPath();

            var writer = File.CreateText(filePath);
            writer.WriteLine("X;Z;Interestingness;");
            foreach (var kv in interestMeasureTable)
            {
                writer.WriteLine("{0};{1};{2};", kv.Key.x, kv.Key.z, kv.Value);
            }
            
        }
        private static string GETPath(){
#if UNITY_EDITOR
            return Application.dataPath +"/CSV/"+"heatmaps.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
        }

        private float StandardDeviation(int xSize,int zSize)
        {
            var localF = GetLocalDeviationFitness(vertices, xSize, zSize);
            var globalF = GetGlobalDeviationFitness(vertices);

            return (localF + globalF) / 2;
        }
        private float GetGlobalDeviationFitness(Vector3[] mesh, float targetDeviation = 10f)
        {
            var std = CalculateGlobalStd(mesh);

            return Mathf.Clamp(0, 1, (std / targetDeviation));
        }

        private static float GetLocalDeviationFitness(Vector3[] mesh, int xSize, int zSize, float maxStd = 5f, int neighboursDistance = 1)
        {
            //var sumStd = 0f;
            // var sumNormal = 0f;
            var sumNormalClamp = 0f;

            for (var z = 0; z < zSize; z++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    var std = CalculateLocalStd(mesh, xSize, zSize, x, z, neighboursDistance);
                    // sumStd += std;
                    //sumNormal += (std / maxStd);
                    sumNormalClamp += Mathf.Clamp((std / maxStd), 0f, 1f);
                }
            }

            return 1 - (sumNormalClamp / mesh.Length);
        }
    

        private static float CalculateLocalStd(Vector3[] mesh, int xSize, int zSize, int x, int z, int neighboursDistance = 1)
        {
            var sumSqr = 0f;
            var numChecked = 0f;

            for (var zOffset = -neighboursDistance; zOffset <= neighboursDistance; zOffset++)
            {
                for (var xOffset = -neighboursDistance; xOffset <= neighboursDistance; xOffset++)
                {
                    if ((z + zOffset < 0 || z + zOffset >= zSize) || (x + xOffset < 0 || x + xOffset >= xSize))
                        continue;
                    //point to add
                    var i = ((z + zOffset) * zSize) + x + xOffset;
                    sumSqr += mesh[i].y * mesh[i].y;
                    numChecked++;
                }
            }

            return (float)Math.Sqrt(sumSqr / numChecked);
        }
        private static float CalculateGlobalStd(Vector3[] mesh)
        {
            var sumSqr = mesh.Sum(p => p.y * p.y);

            return (float)Math.Sqrt(sumSqr / mesh.Length);

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
            surface.BuildNavMesh();
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

            SpawnAssets(mesh);
            return triangles;
        
        }
        public float GetMaxY()
        {
            return curMaxY;
        }
        public float GetMinY()
        {
            return curMinY;
        }
        private void SpawnAssets(Environment_Genome mesh)
        {
            forestSpawner.radius = mesh.poissonDiscParams.forestRadius;
            forestSpawner.regionSize.y = mesh.poissonDiscParams.forestYSize;
            forestSpawner.regionSize.x = mesh.poissonDiscParams.forestXSize;
            smallAssetSpawner.assetXSpread = mesh.poissonDiscParams.assetXSpread;
            smallAssetSpawner.assetZSpread = mesh.poissonDiscParams.assetZSpread;
            smallAssetSpawner.assetYSpread = mesh.poissonDiscParams.assetYSpread;
            smallAssetSpawner.assetsToSpread = mesh.poissonDiscParams.assetsToSpawn;
            smallAssetSpawner.numAssetsToSpawn = mesh.poissonDiscParams.numOfAssetsToSpawn;
            landmarkSpawner.assetXSpread = mesh.poissonDiscParams.assetXSpread;
            landmarkSpawner.assetZSpread = mesh.poissonDiscParams.assetZSpread;
            landmarkSpawner.assetYSpread = mesh.poissonDiscParams.assetYSpread;
       
            for (var i = 0; i < smallAssetSpawner.numAssetsToSpawn; i++)
            {
                smallAssetSpawner.SpreadAssets();
            }
       
            smallAssetSpawner.hasSpawned = true;
       
            for (var i = 0; i < landmarkSpawner.numAssetsToSpawn; i++)
            {
                landmarkSpawner.SpreadAssets();
            }
            landmarkSpawner.hasSpawned = true;
            
            forestSpawner.spawnTrees();


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
    
        private float CalculateNavMesh( int xSize, int zSize, float validationNavProportion = 0.1f)
        {
            
            var zStep = zSize * validationNavProportion;
            var xStep = xSize * validationNavProportion;
            var xMax = xSize - xStep;
            var zMax = zSize - zStep;

            var totalReached = 0f;
            var totalPoints = 0f;

            //look in a grid frin (bottom left + validationNavProportion) to top (right - validationNavProportion)
            for (var z = zStep; z <= zMax; z += zStep)
            {
                for (var x = xStep; x <= xMax; x += xStep)
                {
                    totalReached += GetNumberOfReachables(x, z, xStep, zStep, xMax, zMax);
                    if (x+xStep <= xMax)
                    {
                        totalPoints++;
                    }
                    if (z+zStep <= zMax)
                    {
                        totalPoints++;
                    }
                }
            }
            return totalReached/totalPoints;
        }
    
        private float GetNumberOfReachables(float x, float z, float xStep, float zStep, float xMax, float zMax)
        {
            //used for calculating paths, can be ignored
            var p = new NavMeshPath();

            float canReach = 0;
            //calculate current y
            RaycastHit hit;
            var origin = new Vector3(x, curMaxY + 1, z);
            var ray = new Ray(origin, Vector3.down);
            //shoot a raycast down to get the mesh location at this coordinate
            if (!Physics.Raycast(ray, out hit)) return canReach;
            var y = hit.point.y;
            if (z + zStep <= zMax)
            {

                if (CanAgentReach(x, y, z, x, z + zStep, p))
                {
                    canReach++;
                }
            }

            if (!(x + xStep <= xMax)) return canReach;
            if (CanAgentReach(x, y, z, x + xStep, z, p))
            {
                canReach++;
            }

            return canReach;
        }
        private bool CanAgentReach(float fromX, float fromY, float fromZ, float toX, float toZ, NavMeshPath p)
        {
            var origin = new Vector3(toX, curMaxY + 1, toZ);
            var ray = new Ray(origin, Vector3.down);
            if (!Physics.Raycast(ray, out var hit)) return false;
            navMeshAgent.transform.position = new Vector3(fromX, fromY, fromZ);
            foreach (var t in allObjects)
            {
                if (t.name == "Generator" && t.name == "ValidatorAgent") continue;
                for (var i = 0; i < 4; i++)
                {
                    IsInView(validatorAgent, t);
                    validatorAgent.transform.Rotate(0.0f,90.0f,0.0f);
                }
            }
            return navMeshAgent.CalculatePath(hit.point, p);
        }
        private bool IsInView(GameObject origin, GameObject toCheck)
        {
            var pointOnScreen = cam.WorldToScreenPoint(toCheck.transform.position);
            var position = origin.transform.position;
            var rotation = origin.transform.rotation;
            //Is in front
            if (pointOnScreen.z < 0)
            {
                if (interestMeasureTable.ContainsKey(position))
                {

                    interestMeasureTable[position] += 0f;
                    //interestMeasureTable[position].Add(rotation,0f);
                    
                    
                }
                else
                {
                    interestMeasureTable.Add(position,0f);
                   // interestMeasureTable[position].Add(rotation,0f);

                }



                Debug.Log(origin.name +" is behind: " + toCheck.name + " at point " + position );
                return false;
            }
 
            //Is in FOV
            if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
                (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
            {
                Debug.Log("OutOfBounds: " + toCheck.name + " at point " + position);
                if (interestMeasureTable.ContainsKey(position))
                {

                        interestMeasureTable[position]+= 0f;

                       // interestMeasureTable[position].Add(rotation,0f);
                    
                    
                }
                else
                {
                    interestMeasureTable.Add(position,0f);
                   // interestMeasureTable[position].Add(rotation,0f);
                }            
                return false;
            }

            var heading = toCheck.transform.position - position;
           // var direction = heading.normalized;// / heading.magnitude;

            if (!Physics.Linecast(position, toCheck.transform.position, out var hit))
            {
                score += 1 / allObjects.Length;
                if (interestMeasureTable.ContainsKey(position))
                {
                    interestMeasureTable[position] += calculateInterestingness(toCheck);
                        //interestMeasureTable[position].Add(rotation,calculateInterestingness(toCheck));
                }
                else
                {
                    interestMeasureTable.Add(position,calculateInterestingness(toCheck));
                    //interestMeasureTable[position].Add(rotation,calculateInterestingness(toCheck));
                }               
                return true;
            }
            if (hit.transform.name == toCheck.name) return true; 
            //Debug.DrawLine(cam.transform.position, toCheck.transform.position, Color.red);
            if (interestMeasureTable.ContainsKey(position))
            {
                interestMeasureTable[position] += 0f;
            }
            else
            {
                interestMeasureTable.Add(position,0f);
            }     
            Debug.Log(toCheck.name + " occluded by " + hit.transform.name + " at point " + position);
            return false;
        }

        private float calculateInterestingness(GameObject gameObject)
        {
            if (gameObject.name.Contains("House"))
                return 10f * ((float)1 / allObjects.Length);
            if (gameObject.name.Contains("Tree"))
                return gameObject.transform.localScale.x * ((float) 1 / allObjects.Length);
            return ((float)1 / allObjects.Length);

        }

    }
}
