using UnityEngine;

public class RaycastAssetAlignerNoOverlap : MonoBehaviour
{
    public GameObject[] assetsToPickFrom;
    public float raycastDistance = 100f;
    public float overlapTestBoxSize = 1f;
    public LayerMask spawnedAssetLayer;
    // Start is called before the first frame update
    void Start()
    {
        PositionRaycast();
    }

    void PositionRaycast()  
    {
        RaycastHit hit; //where our item was hit
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))    //if it hits something, from moving down from transform.position, out is the returned value
        {
            var spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);   //calculate the difference between the angle of the object hit and 'up'. this gives us a value to rotate a spawned item. 
                                                                                            //duplicate script ,or toggle on and off ,for some assets like trees?

            var overlapTestBoxScale = new Vector3(overlapTestBoxSize, overlapTestBoxSize, overlapTestBoxSize);      //create a box the size of the float stated
            var collidersInsideOverlapBox = new Collider[1]; // we only need to check for one collider 
            var numberOfCollidersFound = Physics.OverlapBoxNonAlloc(hit.point, overlapTestBoxScale, collidersInsideOverlapBox, spawnRotation, spawnedAssetLayer);   //'overlapboxnonalloc' doesn't allocate memory for garbage allocation. check at hit.point, then pass in the array and the rotation. 
            
            if (numberOfCollidersFound == 0)
            {
                Pick(hit.point, spawnRotation);
            }
        }
    }

    void Pick(Vector3 positionToSpawn, Quaternion rotationToSpawn)
    {
        var randomIndex = Random.Range(0, assetsToPickFrom.Length);     //generate a random number to grab an asset
        var clone = Instantiate(assetsToPickFrom[randomIndex], positionToSpawn, rotationToSpawn); //spawn the item at the point where the raycast hit and the rotation of the ground. 
    }
}
