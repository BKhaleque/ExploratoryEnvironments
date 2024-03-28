using Unity.VisualScripting;
using UnityEngine;

public class RaycastAssetAligner : MonoBehaviour
{
    //public float raycastDistance = 100f;
    private GameObject assetToSpawn;
    
    private float minY;
    private float maxY;

    private string name = "";

    void Start()
    {
        assetToSpawn = this.gameObject;
        PositionRaycast();
    }

    void PositionRaycast()
    {
        
        var ray = new Ray(new Vector3(transform.position.x, maxY, transform.position.z), Vector3.down);

        RaycastHit hit; //where our item was hit
        if (Physics.Raycast(ray, out hit))    //if it hits something, from moving down from transform.position, out is the returned value
        {
            //Debug.Log("raycast hit");
            var spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);   //calculate the difference between the angle of the object hit and 'up'. this gives us a value to rotate a spawned item. 
                                                                                            //duplicate script ,or toggle on and off ,for some assets like trees?
           var clone = Instantiate(assetToSpawn, hit.point, spawnRotation); //spawn the item at the point where the raycast hit and the rotation of the ground. 
            Destroy(gameObject);     
        }
    }


    public void SetMinAndMaxY(float minY, float maxY)
    {
        this.minY = minY;
        this.maxY = maxY;
    }

    void OnDrawGizmosSelected()
    {
        //Ray r = new Ray(new Vector3(transform.position.x, transform.position.y + 100f, transform.position.z), new Vector3(transform.position.x, transform.position.y - 100f, transform.position.z));
        var r = new Ray(new Vector3(transform.position.x, maxY + 1f, transform.position.z), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            name = hit.transform.name;
            Gizmos.color = Color.green;
        }
        else
        {
            name = "nothing";
            Gizmos.color = Color.red;
        }
        Gizmos.DrawRay(r);
        //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, -10, transform.position.z));
    }


}
