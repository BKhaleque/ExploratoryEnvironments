using UnityEngine;

public class Generate : MonoBehaviour
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
      //Debug.Log("thing should have spawned");
      var scale = Vector3.one;
      var randomScale = Random.Range(minSize, maxSize);
      scale = new Vector3(randomScale, randomScale, randomScale);
      clone.transform.localScale = scale;
      clone.transform.parent = GameObject.Find("emptyHolder").transform;
      Destroy(gameObject);
    }
    
}
