using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WeaponSlotMovement : MonoBehaviour
{
    [SerializeField] bool playInEditor = false; // Activates/deactivates constant refresh rate of the scene in edit mode.

    [Header("Second Order Variables")]
    [Range(0.0001f, 8)] public float f = 0;
    [Range(0, 5)] public float z = 0;
    [Range(-5, 5f)] public float r = 0;

    public Transform followTransform;   // The transform that we are following.

    SecondOrderDynamics myDynamics;
    Vector3 oldFollowPosition;          // Stores the old position of the object that we are following. It is used to calculate its speed.


    private void Update()
    {
        Vector3 speed;
        if (Time.deltaTime != 0)
            speed = (followTransform.position - oldFollowPosition) / Time.deltaTime;
        else
            speed = Vector3.zero;

        if (speed.magnitude < 100)
        {

            Vector3 newPosition = GetDynamics().Update(Time.deltaTime, followTransform.position, speed);

            // We prevent a Vector3(NaN, NaN, NaN) from being applied to our position. I don't know why, but sometimes the Update() function returns that.
            if (newPosition != new Vector3(float.NaN, float.NaN, float.NaN))
                transform.position = newPosition;
        }

        oldFollowPosition = followTransform.position;
    }


    SecondOrderDynamics GetDynamics()
    {
        if (myDynamics == null)
        {
            myDynamics = new SecondOrderDynamics(f, z, r, followTransform.position);
        }

        // If our f, z and/or r has changed, we restart our script.
        if (myDynamics.playerInputs != new Vector3(f, z, r))
        {
            myDynamics = new SecondOrderDynamics(f, z, r, followTransform.position);
        }

        return myDynamics;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying & playInEditor)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }
}
