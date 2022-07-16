using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(WeaponSlotMovement))]
[CanEditMultipleObjects]
public class SecondOrder_TestGraph : Editor
{
    WeaponSlotMovement myScript;          // Holds our target script.
    float leftX, rightX, lowY, highY;   // Variables that hold the size of the graph being drawn.

    SecondOrderDynamics_Editor myDynamics;

    private void OnEnable()
    {
        myScript = target as WeaponSlotMovement;
        CalculateBounds();
    }



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CalculateBounds();

        GUILayout.Space(10);

        // We initialize our graph.
        SecondOrderDynamics_Editor myDynamics = new SecondOrderDynamics_Editor(myScript.f, myScript.z, myScript.r, 0);


        TLP.Editor.EditorGraph graph = new TLP.Editor.EditorGraph(leftX, lowY, rightX, highY, "My function", 100);

        graph.AddFunction(x => myDynamics.Update(0.02f, 1, 0, true), Color.cyan);
        graph.AddLineX(0);
        graph.AddLineY(0, Color.white);
        graph.AddLineY(1, new Color(.5f, .8f, .3f, 1));

        for (int i = 1; i < 11; i++)
        {
            if (i <= rightX)
                graph.AddLineX(i, new Color(.3f + .05f * i, .3f - .05f * i, .3f, 1));
        }
        graph.Draw();
    }

    // Calculating the bounds of the graph in the inspector.
    void CalculateBounds()
    {
        // We re-write our Dynamics class.
        myDynamics = new SecondOrderDynamics_Editor(myScript.f, myScript.z, myScript.r, 0);

        // Size of the graph in the X Axis.
        leftX = Mathf.Clamp(-10 / Mathf.Sqrt(myScript.f), -.3f, -.8f);
        rightX = Mathf.Min(7 / myScript.f + 1f, 9.5f);

        // We calculate a sample of values for our function. We store them in a List<float>.
        List<float> myResults = new List<float>();
        for (int i = 0; i < 100; i++)
            myResults.Add(myDynamics.Update(0.02f, 1, 0, true));

        lowY = Mathf.Min(Mathf.Min(myResults.ToArray()) - 0.3f, -.4f);
        highY = Mathf.Max(Mathf.Max(myResults.ToArray()) + 0.3f, 1.4f);
    }
}
