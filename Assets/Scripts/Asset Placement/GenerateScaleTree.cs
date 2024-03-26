using UnityEngine;

public class GenerateScaleTree : MonoBehaviour
{
  public GameObject[] objects;

    public Transform parent;
    public float minSize = 1f;
    public float maxSize = 3f;

    // Start is called before the first frame update
    void Start()
    {
      
      	var rand = Random.Range(0, objects.Length);
        var clone = Instantiate(objects[rand], transform.position, Quaternion.identity);

        var scale = Vector3.one;
        var randomScale = Random.Range(minSize, maxSize);
        scale = new Vector3(randomScale, randomScale, randomScale);
       
        
       //Vector3 randomSize = new Vector3(Random.Range(minSize, maxSize), 1,1);
       clone.transform.localScale = scale;
       clone.transform.parent = GameObject.Find("emptyHolder").transform;


       Destroy(gameObject);
    }

	public void spawnTree(){
      	var rand = Random.Range(0, objects.Length);
        var clone = Instantiate(objects[rand], transform.position, Quaternion.identity);

        var scale = Vector3.one;
        var randomScale = Random.Range(minSize, maxSize);
        scale = new Vector3(randomScale, randomScale, randomScale);
       
        
       //Vector3 randomSize = new Vector3(Random.Range(minSize, maxSize), 1,1);
       clone.transform.localScale = scale;
       clone.transform.parent = GameObject.Find("emptyHolder(Clone)").transform;


       Destroy(gameObject);

	}
}
