using Evolutionary_Process;
using UnityEngine;

public class AssetAreaSpawner : MonoBehaviour
{
    
    
    public GameObject[] assetsToSpread;
    public int numAssetsToSpawn = 10; //number of assets to spawn

    public float assetXSpread = 10; //Find a way to spread over the generated terrain, do i call the mesh generate script to get the x,y, and z details?
    public float assetYSpread;
    public float assetZSpread = 10;
    // Start is called before the first frame update

    public bool hasSpawned;

    //private Evaluate _evaluate;
    private GenerateRandomEnvironment _evaluate;

    [SerializeField]
    //private MeshGenerator meshGenerator;

    void Start()
    {
        _evaluate = GetComponent<GenerateRandomEnvironment>();
    }
    
    void LateUpdate()
    {
        if (hasSpawned)
        {
            return;
        }
        //
        // if (!meshGenerator.IsReady())
        // {
        //     return;
        // }
        

        //Debug.Log($"About to look for size with minY={meshGenerator.GetMinY()} and maxY={meshGenerator.GetMaxY()}");

        var size = FindMeshSize();
        assetXSpread = size.x;
        assetZSpread = size.z;

        // for (int i = 0; i < numAssetsToSpawn; i++)
        // {
        //     SpreadAssets();
        // }
        // hasSpawned = true;
    }

    /// <summary>
    /// Looks for the maximum size of the terrain that has been geberated
    /// </summary>
    /// <returns>A vector 3 where the X and Z coordinates are the maximum values for the terrain (Y is always zero and can be ignored)</returns>
    private Vector3 FindMeshSize()
    {
        var terrainSize = Vector3.zero;
        var curCheckPos = 10f;
        const float checkStep = 10f;

        while (terrainSize.x == 0f || terrainSize.z == 0f)
        {
            if (terrainSize.x == 0f)
            {
                //check for x
                if (!Physics.Raycast(new Ray(new Vector3(curCheckPos, _evaluate.GetMaxY() + 1f, 0f), Vector3.down)))
                {
                    //Debug.Log($"Didn't hit the ground at x {curCheckPos}");
                    terrainSize = new Vector3(curCheckPos - checkStep, 0, terrainSize.z);
                }
            }

            if (terrainSize.z == 0f)
            {
                //check for z
                if (!Physics.Raycast(new Ray(new Vector3(0f, _evaluate.GetMaxY() + 1f, curCheckPos), Vector3.down)))
                {
                    terrainSize = new Vector3(terrainSize.x, 0, curCheckPos - checkStep);
                }
            }
            curCheckPos += checkStep;
        }

        return terrainSize;
    }
    
    public void SpreadAssets()
    {
        _evaluate = GetComponent<GenerateRandomEnvironment>();
        var randposition = new Vector3(Random.Range(0f, assetXSpread), 1f, Random.Range(0f, assetZSpread));  //define the positions in the space

        var ray = new Ray(new Vector3(randposition.x, _evaluate.GetMaxY()-1, randposition.z), Vector3.down);

        RaycastHit hit; //where our item was hit
        if (Physics.Raycast(ray, out hit))    //if it hits something, from moving down from transform.position, out is the returned value
        {
            //Debug.Log("I have hit the ground!");
        }
        //TODO change for type
        var clone = Instantiate(assetsToSpread[Random.Range(0,assetsToSpread.Length)], randposition, Quaternion.identity);

        clone.GetComponent<RaycastAssetAligner>().SetMinAndMaxY(_evaluate.GetMinY(), _evaluate.GetMaxY());
    }





}
