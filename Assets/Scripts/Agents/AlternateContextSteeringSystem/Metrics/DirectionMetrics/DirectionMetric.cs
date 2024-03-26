using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DirectionMetric : MonoBehaviour
{

    public float weight;
    // Start is called before the first frame update
    public bool inverse;

    public abstract void EvaluateDecision(Vector3 direction);
    public abstract float GetScore();
    // Update is called once per frame

}
