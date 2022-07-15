using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [Range(0, 100)] public float mouseXSensitivity = 12f;
    [Range(0, 100)] public float mouseYSensitivity = 12f;


    [Header("Clamps to camera movement")]
    // (if the camera is positioned in an almost vertical position, that makes it hard to control).
    [SerializeField] [Range(0.1f, 90f)] float topCameraClamp = 85;
    [SerializeField] [Range(-90f, -0.1f)] float bottomCameraClamp = -85;

    // Public clamped camera values referenced in another CameraFocusManager.
    [HideInInspector] public Vector2 cameraClamps = Vector2.zero;    // Value X determines the top clamp. Value Y determines the bottom clamp.


    // GameObject fills:
    [Header("Gameobject references")]
    public Transform cameraPivotTransform;


    // Stores the Y and X values of the camera rotation.
    // We make them public so that other scripts can access them
    // (this is important because yRotation and xRotation are the most accurate rotation value in degrees we have).
    [HideInInspector] public float horizontalRotation, verticalRotation;



    void Awake()
    {
        // We give our public variable cameraClamps its value.
        cameraClamps = new Vector2(topCameraClamp, bottomCameraClamp);


        // Locks the cursor in the middle of the screen.
        Cursor.lockState = CursorLockMode.Locked;


        // Giving its value to xRotation & yRotation. Otherwise, clamping doesn't work.
        // We also round the values to avoid nasty numbers.
        horizontalRotation = (float)System.Math.Round(transform.rotation.eulerAngles.y, 2);
        verticalRotation = (float)System.Math.Round(cameraPivotTransform.rotation.eulerAngles.x, 2);
    }


    void Update()
    {
        //New Input System for mouse values
        float mouseRotationX = Mouse.current.delta.x.ReadValue();
        float mouseRotationY = Mouse.current.delta.y.ReadValue();

        // We process the Input values by multiplicating them with the sensitivity and the deltaTime.
        float cameraXMovement = mouseRotationX * mouseXSensitivity * 10 * Time.deltaTime;
        float cameraYMovement = mouseRotationY * mouseYSensitivity * 10 * Time.deltaTime;

        // We apply the values of the Y Input to later clamp it.
        verticalRotation -= cameraYMovement;
        verticalRotation = Mathf.Clamp(verticalRotation, bottomCameraClamp, topCameraClamp);

        // We also apply the values of the X input and clamp them (between 0 and 360)
        horizontalRotation += cameraXMovement;
        if (horizontalRotation < 0)
            horizontalRotation += 360;
        if (horizontalRotation > 360)
            horizontalRotation -= 360;

        // Rotating the camera in the Y axis.
        cameraPivotTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Rotating the player in the X axis.
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }
}
