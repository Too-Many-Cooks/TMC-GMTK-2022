using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraShakeController : MonoBehaviour
{
    public void ShakeCamera(int typeOfShake)
    {
        switch (typeOfShake)
        {
            // Revolver shooting.
            case 0:
                CameraShaker.Instance.ShakeOnce(1f, 3, 0.01f, 0.7f);
                break;

            // Revolver reload.
            case 1:
                CameraShaker.Instance.ShakeOnce(0.4f, 1, 0.01f, 0.4f);
                break;


            // Shotgun shooting.
            case 2:
                CameraShaker.Instance.ShakeOnce(2, 2.5f, 0.01f, 1f);
                break;

            // Shotgun reload.
            case 3:
                CameraShaker.Instance.ShakeOnce(1.5f, 1.5f, 0.02f, 0.5f);
                break;

            // Errors
            default:
                Debug.LogError("TypeOfShake int not recognized, value " + typeOfShake + " not included in the controller switch.");
                break;

        }
    }
}
