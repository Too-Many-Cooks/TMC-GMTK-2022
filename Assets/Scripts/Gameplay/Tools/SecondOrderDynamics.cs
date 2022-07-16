using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SecondOrderDynamics
// Taken from the following video by t3ssel8r: https://www.youtube.com/watch?v=KPoeNZZ6H4s
{
    private Vector3 xp;                     // Previous input.
    private Vector3 y, yd;                  // State Variables.
    private float _w, _z, _d, k1, k2, k3;   // Constants.
    public Vector3 playerInputs;            // We store the player f, z and r inputs to compare mid-runtime if they have changed.


    public SecondOrderDynamics(float f, float z, float r, Vector3 originalPosition)
    {
        // Compute constants.
        _w = 2 * Mathf.PI * f;
        _z = z;
        _d = _w * Mathf.Sqrt(Mathf.Abs(z * z - 1));
        k1 = z / (Mathf.PI * f);
        k2 = 1 / (_w * _w);
        k3 = r * z / _w;

        // Initialize varibles.
        xp = originalPosition;
        y = originalPosition;
        yd = Vector3.zero;

        // Store player input variables.
        playerInputs = new Vector3(f, z, r);
    }

    public Vector3 Update(float time, Vector3 position, Vector3 inputVelocity, bool inputVelocityUnknown = false)
    {
        if (inputVelocityUnknown == true) // Estimate velocity.
        {
            inputVelocity = (position - xp) / time;
            xp = position;
        }

        float k1_stable, k2_stable;
        if (_w * time < _z) // Clamp k2 to guarantee stability without jitter.
        {
            k1_stable = k1;
            k2_stable = Mathf.Max(k2, time * time / 2 + time * k1 / 2, time * k1);
        }
        else    // Use pole matching when the system is very fast.
        {
            float t1 = Mathf.Exp(-_z * _w * time);
            float alpha = 2 * t1 * (_z <= 1 ? Mathf.Cos(time * _d) : (float)Math.Cosh(time * _d));
            float beta = t1 * t1;
            float t2 = time / (1 + beta - alpha);
            k1_stable = (1 - beta) * t2;
            k2_stable = time * t2;
        }

        y = y + time * yd; // Integrate position by velocity.
        yd = yd + time * (position + k3 * inputVelocity - y - k1 * yd) / k2_stable; // Integrate velocity by acceleration.
        return y;
    }
}
