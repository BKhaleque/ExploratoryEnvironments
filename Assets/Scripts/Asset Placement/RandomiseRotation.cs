using UnityEngine;

public class RandomiseRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RandomiseAssetRotation();
    }

    void RandomiseAssetRotation()
    {
        var randYRotation = Quaternion.Euler(0, Random.Range(0, 360), 0); //rotate the generated assets around the y axis so assets aren't flipped 
        transform.rotation = randYRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
