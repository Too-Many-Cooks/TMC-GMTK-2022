using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformData
{
    Vector3 position;
    Quaternion rotation;
    Vector3 scale;

    public TransformData(Transform t)
    {
        position = t.position;
        rotation = t.rotation;
        scale = t.localScale;
    }

    public void ApplyDataTo(Transform targetTransform)
    {
        targetTransform.position = position;
        targetTransform.rotation = rotation;
        targetTransform.localScale = scale;
    }
}
