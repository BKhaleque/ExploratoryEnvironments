using UnityEngine;
using UnityEngine.Rendering;

public class LightAndShadow : DirectionMetric
{
    private LightDetectionSystem lightDetectionSystem; // Custom light detection system attached to the agent
    private int lightChoices = 0;
    private int shadowChoices = 0;
    private float darkThreshold = 0.1f; // Threshold to distinguish between light and shadow
    private float lightIntensity;
    private void Awake()
    {
        // Initialize the light detection system
        lightDetectionSystem = gameObject.AddComponent<LightDetectionSystem>();
    }
    
    // Update is called once per frame
    private void Update()
    {
       // OnDisable();
    }

    public override void EvaluateDecision(Vector3 direction)
    {
        // Use the light detection system to determine if the agent's next move is towards a light or shadow area
         lightIntensity = lightDetectionSystem.GetCurrentLightIntensity(direction);
        lightChoices = 0;
        shadowChoices = 0;

        if (lightIntensity <= darkThreshold)
        {
            lightChoices++;
        }
        else
        {
            shadowChoices++;
        }
        
        //return lightChoices;
    }

    public override float GetScore()
    {
        // Calculate the metric,  as a ratio of light choices to total choices
        // var totalChoices = lightChoices + shadowChoices;
        // if(inverse)
        //     return totalChoices > 0 ? (float)shadowChoices / totalChoices : 0;
        //
        // return totalChoices > 0 ? (float)lightChoices / totalChoices : 0;
        return lightIntensity;
    }

    void OnDisable()
    {
        // Output the light vs shadow score
//        Debug.Log("Light vs Shadow Exploration Score: " + GetScore());
    }
}

public class LightDetectionSystem: MonoBehaviour
{
    public float rayDistance = 80f; // Distance of the raycast

    
    //constructor
    
    // This method returns the light intensity at the raycast hit point
    public float GetCurrentLightIntensity(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
        {
            // Sample the light at the hit point
            LightProbes.GetInterpolatedProbe(hit.point, null, out var probeData);
            var lightColor = new Color[2];
            probeData.Evaluate(new[] { Vector3.up, Vector3.down },  lightColor); // Evaluate the probe data for an upward direction

            // Convert the color to a grayscale value to represent intensity
            var lightIntensity = lightColor[0].grayscale;
            //Debug.Log(lightIntensity);
            return lightIntensity;
        }

        // Return a default value if nothing is hit
        return 0f;
    }

    void OnDrawGizmos()
    {
        // Draw the ray in the editor for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * rayDistance);
    }
}