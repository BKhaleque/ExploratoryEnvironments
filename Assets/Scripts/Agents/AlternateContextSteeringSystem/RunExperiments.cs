using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RunExperiments : MonoBehaviour
{
    public GameObject agent;


    //private PlanarSteeringBehaviour[] GetBehaviours() => SteeringBehaviours;
    //private int zeroTargetCounter = 0;
    //public Dictionary<(int, int), float> regionTime = new Dictionary<(int, int), float>();
    private int interval = 4;
    private float timer = 0;
    private string sceneName = "";
    int run = 0;
    private float timeSpent;
    string a = "A";
    public Dictionary<string, ObjectMetric[]> objectMetricsDict = new();
    public List<ObjectMetric> objectMetrics = new();
    public List<DirectionMetric> directionMetrics = new();
    public Dictionary<string, KeyValuePair<ObjectMetric[],DirectionMetric[]>> objectAndDirectionMetricsDict = new();

    public Dictionary<string, DirectionMetric[]> directionMetricsDict = new();

    //private Metric[] agentMetrics;
    //Dictionary<string, Metric[]> metricDict = new Dictionary<string, Metric[]>();

    private NoveltySearchEvaluator ng;
    //private CoverageAndDistribution cd;
    private Checklist ch;

    private int randomDirectionCounter = 0;
    private AnotherAgentController ag;
    private PositionTracker pt;

    private GameObject terrain;
    private Vector3 spawnPoint;
    private GameObject randomAgent;

    // Start is called before the first frame update
    void Awake()
    {
        ag = GetComponent<AnotherAgentController>();
        ng = GetComponent<NoveltySearchEvaluator>();
        //cd = GetComponent<CoverageAndDistribution>();
        var rg = GetComponent<RegionTimeMeasure>();
        rg.initialiseDict();
        pt = GetComponent<PositionTracker>();
        terrain = GameObject.Find("Terrain");
        var vertices = terrain.GetComponent<MeshFilter>().mesh.vertices;
        //gameObject.transform.position = vertices[Random.Range(0, vertices.Length)];
        a = "A";
        spawnPoint = vertices[Random.Range(0, vertices.Length)];
        gameObject.transform.position = spawnPoint;
        
        directionMetricsDict.Add("A", new DirectionMetric[] { directionMetrics[0] });
        directionMetricsDict.Add("B", new DirectionMetric[] { directionMetrics[1] });
        directionMetricsDict.Add("C", new DirectionMetric[] { directionMetrics[2] });
        directionMetricsDict.Add("D", new DirectionMetric[] { directionMetrics[3] });
        directionMetricsDict.Add("AB", new DirectionMetric[] { directionMetrics[0], directionMetrics[1] });
        directionMetricsDict.Add("AC", new DirectionMetric[] { directionMetrics[0], directionMetrics[2] });
        directionMetricsDict.Add("AD", new DirectionMetric[] { directionMetrics[0], directionMetrics[3] });
        directionMetricsDict.Add("BC", new DirectionMetric[] { directionMetrics[1], directionMetrics[2] });
        directionMetricsDict.Add("BD", new DirectionMetric[] { directionMetrics[1], directionMetrics[3] });
        directionMetricsDict.Add("CD", new DirectionMetric[] { directionMetrics[2], directionMetrics[3] });
        

        
        
        objectMetricsDict.Add("E", new ObjectMetric[] { objectMetrics[0] });
        objectMetricsDict.Add("F", new ObjectMetric[] { objectMetrics[1] });
        objectMetricsDict.Add("G", new ObjectMetric[] { objectMetrics[2] });
        objectMetricsDict.Add("H", new ObjectMetric[] { objectMetrics[3] });
        objectMetricsDict.Add("EF", new ObjectMetric[] { objectMetrics[0], objectMetrics[1] });
        objectMetricsDict.Add("EG", new ObjectMetric[] { objectMetrics[0], objectMetrics[2] });
        objectMetricsDict.Add("EH", new ObjectMetric[] { objectMetrics[0], objectMetrics[3] });
        objectMetricsDict.Add("FG", new ObjectMetric[] { objectMetrics[1], objectMetrics[2] });
        objectMetricsDict.Add("FH", new ObjectMetric[] { objectMetrics[1], objectMetrics[3] });
        objectMetricsDict.Add("GH", new ObjectMetric[] { objectMetrics[2], objectMetrics[3] });
        

       // objectAndDirectionMetricsDict.Add("AB3", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[0]}, new DirectionMetric[]{ directionMetrics[0]}));
        objectAndDirectionMetricsDict.Add("EB", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[0]}, new DirectionMetric[]{ directionMetrics[1]}));
        objectAndDirectionMetricsDict.Add("EC", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[0]}, new DirectionMetric[]{ directionMetrics[2]}));
        objectAndDirectionMetricsDict.Add("ED", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[0]}, new DirectionMetric[]{ directionMetrics[3]}));
        objectAndDirectionMetricsDict.Add("FB", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[1]}, new DirectionMetric[]{ directionMetrics[1]}));
        objectAndDirectionMetricsDict.Add("FC", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[1]}, new DirectionMetric[]{ directionMetrics[2]}));
        objectAndDirectionMetricsDict.Add("FD", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[1]}, new DirectionMetric[]{ directionMetrics[3]}));
        objectAndDirectionMetricsDict.Add("GB", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[2]}, new DirectionMetric[]{ directionMetrics[1]}));
        objectAndDirectionMetricsDict.Add("GC", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[2]}, new DirectionMetric[]{ directionMetrics[2]}));
        objectAndDirectionMetricsDict.Add("GD", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[2]}, new DirectionMetric[]{ directionMetrics[3]}));
        objectAndDirectionMetricsDict.Add("HB", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[3]}, new DirectionMetric[]{ directionMetrics[1]}));
        objectAndDirectionMetricsDict.Add("HC", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[3]}, new DirectionMetric[]{ directionMetrics[2]}));
        objectAndDirectionMetricsDict.Add("HD", new KeyValuePair<ObjectMetric[], DirectionMetric[]>(new ObjectMetric[] {objectMetrics[3]}, new DirectionMetric[]{ directionMetrics[3]}));
        

        
        ch = GetComponent<Checklist>();
        if (SceneManager.GetActiveScene().name.Contains("Journey"))
            sceneName = "Journey";
        else if (SceneManager.GetActiveScene().name.Contains("Plateu"))
            sceneName = "Zelda";
        else if (SceneManager.GetActiveScene().name.Contains("No"))
            sceneName = "NoMansSky";
        else if (SceneManager.GetActiveScene().name.Contains("Proteus"))
            sceneName = "Proteus";
        else if (SceneManager.GetActiveScene().name.Contains("Dense"))
            sceneName = "Dense";
        else if (SceneManager.GetActiveScene().name.Contains("Empty"))
            sceneName = "Empty";

        //rg.path = "Assets/Test-Data/HFSM/Heatmap Data/" + sceneName + "/A.txt";
        //InvokeRepeating("RunExps", 10f, 1.5f);
        rg.path = "Assets/Test-Data/ObjectAndDirectionBasedAgent/Heatmap Data/" + sceneName + "/" + a + ".txt";
        pt.filePath =
            "Assets/Test-Data/ObjectAndDirectionBasedAgent/Path Data/" + sceneName + "/" + a + ".txt";
        ag.DirectionMetrics = directionMetricsDict[a].ToList();
        ag.ObjectMetrics = new List<ObjectMetric>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RegionTimeMeasure rg = GetComponent<RegionTimeMeasure>();

        timer += Time.deltaTime;
        if (!(timer >= 180)) return;
        timer = 0f;
        RunExps();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void RunExps()
    {
        RegionTimeMeasure rg = GetComponent<RegionTimeMeasure>();

        writeEvaluation(a);
        switch (a)
        {
            case "A":
                a = "B";
                ResetAgent(a,rg);
                break;
            case "B":
                a = "C";
                ResetAgent(a,rg);
                break;
            case "C":
                a = "D";
                ResetAgent(a,rg);
                break;
            case "D": 
                a = "AB";
                ResetAgent(a,rg);
                break;
            case "AB":
                a = "AC";
                ResetAgent(a,rg);
                break;
            case "AC":
                a = "AD";
                ResetAgent(a,rg);
                break;
            case "AD":
                a = "BC";
                ResetAgent(a,rg);
                break;
            case "BC":
                a = "BD";
                ResetAgent(a,rg);
                break;
            case "BD":
                a = "CD";
                ResetAgent(a,rg);
                break;
            case "CD":
                a = "E";
                ResetAgent(a,rg);
                break;
            case "E":
                a = "F";
                ResetAgent(a,rg);
                break;
            case "F":
                a = "G";
                ResetAgent(a,rg);
                break;
            case "G":
                a = "H";
                ResetAgent(a,rg);
                break;
            case "H":
                a = "EF";
                ResetAgent(a,rg);
                break;
            case "EF":
                a = "EG";
                ResetAgent(a,rg);
                break;
            case "EG":
                a = "EH";
                ResetAgent(a,rg);
                break;
            case "EH":
                a = "FG";
                ResetAgent(a,rg);
                break;
            case  "FG":
                a = "FH";
                ResetAgent(a,rg);
                break;
            case "FH":
                a = "GH";
                ResetAgent(a,rg);
                break;
            case "GH":
                a = "EB";
                ResetAgent(a,rg);
                break;
           // case "AB3":
            //    a = "EB";
            //    ResetAgent(a,rg);
            //    break;
            case "EB":
                a = "EC";
                ResetAgent(a,rg);
                break;
            case "EC":
                a = "ED";
                ResetAgent(a,rg);
                break;
            case "ED":
                a = "FB";
                ResetAgent(a,rg);
                break;
            case "FB":
                a = "FC";
                ResetAgent(a,rg);
                break;
            case "FC":
                a = "FD";
                ResetAgent(a,rg);
                break;
            case "FD":
                a = "GB";
                ResetAgent(a,rg);
                break;
            case "GB":
                a = "GC";
                ResetAgent(a,rg);
                break;
            case "GC":
                a = "GD";
                ResetAgent(a,rg);
                break;
            case "GD":
                a = "HB";
                ResetAgent(a,rg);
                break;
            case "HB":
                a = "HC";
                ResetAgent(a,rg);
                break;
            case "HC":
                a = "HD";
                ResetAgent(a,rg);
                break;
            case "HD":
                 rg.writeToFile();
                 // randomAgent.GetComponent<RandomRunnerExperiments>().spawnPoint = spawnPoint;
                 // randomAgent.SetActive(true);
                 AppHelper.Quit();
                 break;
        }
        
        timer = 0f;

    }

    private void writeEvaluation(string metric)
    {
        ng = GetComponent<NoveltySearchEvaluator>();
        //cd = GetComponent<CoverageAndDistribution>();
        ch = GetComponent<Checklist>();
        //check if file exists to write to
        pt.WritePositionsToFile();
        var path = "Assets/Test-Data/ObjectAndDirectionBasedAgent/Evaluation/" + sceneName + "/" + metric + ".txt";
        System.IO.File.WriteAllText(path,string.Empty);

        //write to file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Inspection: " +
                         ch.CalculateInspectionScore());
        foreach (var entry in ng.PositionObjectViews)
        {
            writer.WriteLine(entry.Key + "," + entry.Value);
        }

        writer.Close();
        //reset ng,ch and cd
        ng.resetNoveltySearchEvaluator();
        //cd.resetCoverageAndDistribution();
        ch.resetCheckList();
    }
    private void ResetAgent(string metric, RegionTimeMeasure rg)
    {
        //Debug.Log("Resetting Agent");
        //if(timer)
        var vertices = terrain.GetComponent<MeshFilter>().mesh.vertices;
        gameObject.transform.position = spawnPoint;
        pt.filePath =
            "Assets/Test-Data/ObjectAndDirectionBasedAgent/Path Data/" + sceneName + "/" + metric + ".txt";
        ag.DirectionMetrics = new List<DirectionMetric>();
        ag.ObjectMetrics = new List<ObjectMetric>();
        ag.visitedObjects = new List<GameObject>();
        rg.writeToFile();
        rg.initialiseDict();
        rg.path = "Assets/Test-Data/ObjectAndDirectionBasedAgent/Heatmap Data/" + sceneName + "/" + metric + ".txt";
        if(objectMetricsDict.ContainsKey(a))
            ag.ObjectMetrics = objectMetricsDict[a].ToList();
        if(directionMetricsDict.ContainsKey(a))
            ag.DirectionMetrics = directionMetricsDict[a].ToList();
        if (objectAndDirectionMetricsDict.ContainsKey(a))
        {
            ag.DirectionMetrics = objectAndDirectionMetricsDict[a].Value.ToList();
            ag.ObjectMetrics = objectAndDirectionMetricsDict[a].Key.ToList();
        }
        
        //ag.visitedTargets = new List<Vector3>();
        //ag.targets = new List<Vector3>();
    }

    public static class AppHelper
    {
#if UNITY_WEBPLAYER
    public static string webplayerQuitURL = "http://google.com";
#endif
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
        }
    }

}
