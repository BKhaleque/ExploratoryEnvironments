using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectMetric : MonoBehaviour
{
    public float weight;

    public abstract float getScore();
    public abstract void evaluateObj(GameObject obj);
}
