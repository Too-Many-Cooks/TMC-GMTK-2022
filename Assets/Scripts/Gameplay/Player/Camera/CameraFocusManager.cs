using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusManager : MonoBehaviour
{
    [Header("Camera focussing speeds")]
    // The maximum and minimum time the camera takes to move towards a fixed position.
    [SerializeField] [Range(0.01f, 5f)] float minTurningTime = 0.3f;
    [SerializeField] [Range(0.01f, 5f)] float maxTurningTime = 0.8f;


    [Header("Maximum focussed movement")]
    // This value checks the average degrees of movement in a second. If it is too high, it stops the focus.
    [SerializeField] [Range(50f, 200000f)] float maxFocussedVariation = 130000; // This value is º^2/s.


    // Camera rotation and clamps.
    Vector2 maximumYRotation;               // At Awake(), we store this 2 values from CameraMovement.
                                            // The format is Vector2(topClamp, -bottomClamp).
                                            // We obtain its value from our static reference to CameraMovement script.


    Transform cameraPivotTransform;         // Reference to the camera's pivot, the center of its rotation.
                                            // We obtain its value from our static reference to CameraMovement script.


    // Vars for focussing objects:
    Transform currentFocussedObject;        // Stores the Transform that should be followed by the camera at all times.
                                            // If its value is null, the camera is not following anything.

    float turningDuration = 0;              // It stores the ammount of time that it should take to move the camera towards smth.
    float turningTimer = 0;                 // It stores how much time has passed since the beggining of movement.


    // Vars for storing old values: (used in lerps)
    TransformData origCameraPivotTransform;     // Stores the cameraPivotTransform from when the focussing started.
    float origVerticalRotation;                 // Stores the yRotation value from when the focussing started.
    float origHorizontalRotation;               // Stores the xRotation value from when the focussing started.
    Transform freeTransform1;                   // We use this GameObjects to apply TransformData to them,
                                                // because I ain't calculating a Vector.forward on my own.
    bool clockwiseTurn = false;                 // Used to keep track of the direction that a movement takes.
                                                // It prevents bugs where the angle is close to 180º.


    // Reference to our camera script (stored at Awake()).
    CameraMovement myCameraMovement;



    private void Awake()
    {
        // We set our reference to our CameraMovement script.
        myCameraMovement = GetComponent<CameraMovement>();

        // We obtain and store the value of the maximum Y rotation with the format: Vector2(topClamp, -bottomClamp).
        if (myCameraMovement != null)
            maximumYRotation = myCameraMovement.cameraClamps;
        else
            Debug.LogError("Couldn't find CameraMovement script.");


        // We also obtain the value of the cameraPivotTransform.
        cameraPivotTransform = myCameraMovement.cameraPivotTransform;


        // We create and allocate our freeTransform.
        freeTransform1 = new GameObject().transform;
        freeTransform1.SetParent(transform);
        freeTransform1.name = "freeTransform1";
    }


    private void LateUpdate()
    {
        // If we are following an object
        if (currentFocussedObject != null)
        {
            // First we update our timer.
            UpdateTimer();


            // Now we apply our TransformData to our 2 freeTransforms to be able to use them in our function.
            origCameraPivotTransform.ApplyDataTo(freeTransform1);


            // We check the current angle between the original playerTransform and the last focussedObjectTransform.
            Vector2 rotation = CalculateDegreesToRotate(currentFocussedObject,
                                                        freeTransform1,
                                                        origHorizontalRotation,
                                                        origVerticalRotation);


            // We obtain our percentage of completion of the movement:
            float percentage = turningTimer / turningDuration;

            // < Space for applying a processing equation to our percentage of completion >

            percentage = EasingFunctions.ApplyEase(percentage, EasingFunctions.Functions.InOutSine);


            // We now do a check for sudden moves (which should cancel the focus)
            // We first store a Vector2 with what is going to be our changes in the character's rotation.
            Vector2 changesInDegrees = new Vector2
                (Mathf.Lerp(origHorizontalRotation, origHorizontalRotation + rotation.x, percentage)
                            - myCameraMovement.horizontalRotation,
                 Mathf.Lerp(origVerticalRotation, origVerticalRotation - rotation.y, percentage)
                            - myCameraMovement.verticalRotation);

            // And then compare the magnitude of that vector to our movement limit. We take into account Time.deltaTime
            if (changesInDegrees.magnitude / Time.deltaTime > maxFocussedVariation)
            {
                StopFocussing();
                print(changesInDegrees.magnitude / Time.deltaTime);
            }

            else
            {
                // And we apply the lerp to both rotations, using the old yRotation and xRotation values as guidance:

                // horizontalRotation:

                // We first update our xRotation value in our other script to avoid them from de-synching.
                myCameraMovement.horizontalRotation =
                    Mathf.Lerp(origHorizontalRotation, origHorizontalRotation + rotation.x, percentage);

                // We then update our transform.
                transform.localRotation = Quaternion.Euler(0, myCameraMovement.horizontalRotation, 0);


                // verticalRotation:

                // We first update our yRotation value in our other script to avoid them from de-synching.
                myCameraMovement.verticalRotation =
                    Mathf.Lerp(origVerticalRotation, origVerticalRotation - rotation.y, percentage);

                // We then update the transform.
                cameraPivotTransform.localRotation = Quaternion.Euler(myCameraMovement.verticalRotation, 0, 0);
            }
        }
    }



    // This function is used when it is required to fix the camera on a new object.
    public void StartFocussing(Transform trans)
    {
        // We save the Pivot of a new focussed object. We store it separetely to check that the pivot exists.
        Transform pivot = trans.Find("CameraPivot");


        if (pivot != null)  // If we DO find a pivot.
        {
            // We allocate its value to our currentFocussedObject variable.
            currentFocussedObject = pivot;

            // We reset the timer.
            turningTimer = 0;

            // We store the current rotation values of our player (to use them to lerp between rotation values).
            origCameraPivotTransform = new TransformData(cameraPivotTransform);
            origHorizontalRotation = myCameraMovement.horizontalRotation;
            origVerticalRotation = myCameraMovement.verticalRotation;

            // We need to to determine the direction of the movement (clock-wise OR counter clock-wise).
            float degreePosition = CalculateXzPosition(pivot);

            if (degreePosition > origHorizontalRotation)
            {
                clockwiseTurn = (degreePosition - origHorizontalRotation > 180);
            }
            else
            {
                clockwiseTurn = (origHorizontalRotation - degreePosition <= 180);
            }

            // We calculate the new duration for the movement.
            turningDuration = CalculateTurningDuration(trans);
        }

        else     // In case no Pivot is found.
            Debug.LogError("Tried to find a CameraPivot in target " + trans.name + ", but none was found.");
    }


    // This function stops all focussing activities.
    public void StopFocussing()
    {
        //  Then we empty the currentFocussedObject variable, disabling the focussing.
        currentFocussedObject = null;
    }


    // This function updates the turningTimer. It also clamps it so that it does not exceed the value of turningDuration.
    void UpdateTimer()
    {
        turningTimer += Time.deltaTime;
        if (turningTimer > turningDuration)
        {
            turningTimer = turningDuration;
        }
    }



    // This function calculates (based on the minimum and maximum focus time values)
    // how long it should take players to focus on an object.
    float CalculateTurningDuration(Transform target)
    {
        // We first calculate how much we have to rotate our character (in degrees again).:
        Vector2 ourRotation = CalculateDegreesToRotate(target,
                                                       cameraPivotTransform,
                                                       myCameraMovement.horizontalRotation,
                                                       myCameraMovement.verticalRotation);


        float duration; // This is gonna hold our return.

        // We calculate the time with the magnitude of our Vector2.
        // We apply that difference in magnitude to a lerp.           We are dividing by the maximum rotation possible.
        float percentage = ourRotation.magnitude / new Vector2(maximumYRotation.x - maximumYRotation.y, 180).magnitude;
        duration = Mathf.Lerp(minTurningTime, maxTurningTime, percentage);

        return duration;
    }



    // This function will calculate the angles from our Transforms to the desired object.
    // It returns its result in the format:     < Vector2 (yAxis, xAxis) >
    Vector2 CalculateDegreesToRotate(Transform target,
                                     Transform originalCameraPivotTransform,
                                     float originalXRotation,
                                     float originalYRotation)
    {
        // We will store the result in this Vec2:
        Vector2 ourRotation;

        // We calculate the position of the object in 3D space with regards to our cameraPivot
        Vector3 targetDirection = target.position - cameraPivotTransform.position;
        // (When using POSITION, we use the NEW TRANSFORM)


        // We calculate the degrees neccesary to turn in the Y axis. It's range should be -360 to 360 degrees.

        // The position of the target in degrees in the XZ axis.
        float targetDegreePosition = CalculateXzPosition(target);

        // The original rotation of our character in the same axis is originalXRotation;

        float rotationY;

        if (clockwiseTurn)
        {
            if (originalXRotation < targetDegreePosition)    // If we cross the 0º threshold.
            {
                rotationY = -((360 - targetDegreePosition) + originalXRotation); // Distance to 0 and from 0, together.
            }
            else
            {
                rotationY = targetDegreePosition - originalXRotation;
            }
        }

        else // Counter clock-wise.
        {
            if (originalXRotation > targetDegreePosition)    // If we cross the 0º threshold.
            {
                rotationY = (360 - originalXRotation) + targetDegreePosition; // Distance to 0 and from 0, together.
            }
            else
            {
                rotationY = targetDegreePosition - originalXRotation;
            }
        }



        // Now we calculate the degrees to turn in the X axis. It's range is determined by our yRotation limits.
        // To do so we first need to place our vectors in the same plane (which is going to be a XY plane).
        // Since the Y value of both vectors is not going to change, we need to merge the X and Z values of both vectors.

        // We merge the X and Z values using the pytagoras.
        // If the magnitude of a vector is the "c" and Y is "a", from   < c^2 = a^2 + b^2 >
        // we obtain that   < b = Mathf.Sqrt(c^2 - a^2) >   . "b" = to the X in our vector2.
        // The Y value stays unchanged.

        Vector2 directionVec = new Vector2(Mathf.Sqrt(Mathf.Pow(targetDirection.magnitude, 2)
                                                      - Mathf.Pow(targetDirection.y, 2)),
                                            targetDirection.y);

        Vector2 forwardVec = new Vector2(Mathf.Sqrt(Mathf.Pow(originalCameraPivotTransform.forward.magnitude, 2)
                                                      - Mathf.Pow(originalCameraPivotTransform.forward.y, 2)),
                                            originalCameraPivotTransform.forward.y);

        // Calculating the angle between the height we are looking towards and our target height.
        float rotationX = Vector2.Angle(directionVec, forwardVec);


        // We check that our rotation in the Y axis does not exceed our Y rotation boundaries. If it does, we clamp it.

        // We first check the orientation of the target rotation.
        // (Vertical)
        if (targetDirection.normalized.y < forwardVec.normalized.y)
            rotationX *= -1;


        // We then sum the rotationX with our current rotation and compare it with our boundaries.
        if (rotationX < 0)   // Negative rotation values.
        {
            // Remember that maximumYRotation has the format:
            if ((originalYRotation - rotationX) > -maximumYRotation.y) // Vector2( topClamp, - bottomClamp)
            {
                // If we go over the boundary, we correct it.
                rotationX = maximumYRotation.y + originalYRotation;
            }
        }
        else                // Positive rotation values.
        {
            // Remember that maximumYRotation has the format:
            if ((originalYRotation - rotationX) < -maximumYRotation.x) // Vector2( topClamp, - bottomClamp)
            {
                // If we go over the boundary, we correct it.
                rotationX = maximumYRotation.x + originalYRotation;
            }
        }


        // We round the values to 1 and 2 integers.
        rotationX = ((float)System.Math.Round(rotationX, 1));
        rotationY = ((float)System.Math.Round(rotationY, 2));


        // And finally, we calculate our Vector2 with our final rotation.
        ourRotation = new Vector2(rotationY, rotationX);

        return ourRotation;
    }


    // This function returns the angle at which any object is with regards to the player.
    // If we visualize the point in a XZ Axis (x,z), 0º would be at (0,1), and 90º would be at (1,0)
    // 180º would be (0,-1) and 270º would be (-1,0). Quadrant ARE CLOCKWISE, which is counter-intuitive.
    float CalculateXzPosition(Transform target)
    {
        // Normalized vector with the XZ direction of the target, normalized.
        Vector2 dirVector = new Vector2(target.position.x - transform.position.x,
                                        target.position.z - transform.position.z).normalized;

        // Creating the variable to hold our result:
        float result = 0;


        // Finding at which quadrant the object is:

        // First quadrant.
        if (dirVector.x > 0 & dirVector.y > 0)
        {
            // Result is the angle with the Vector2(0, 1).
            result = Vector2.Angle(dirVector, new Vector2(0, 1));
        }

        // Second quadrant.
        else if (dirVector.x > 0 & dirVector.y < 0)
        {
            // Result is the angle with Vector2(1, 0) + 90.
            result = Vector2.Angle(dirVector, new Vector2(1, 0)) + 90;
        }

        // Third quadrant.
        else if (dirVector.x < 0 & dirVector.y < 0)
        {
            // Result is the angle with the Vector2(0, -1) + 180.
            result = Vector2.Angle(dirVector, new Vector2(0, -1)) + 180;
        }

        // Fourth quadrant.
        else if (dirVector.x < 0 & dirVector.y > 0)
        {
            // Result is the angle with the Vector2(-1, 0) + 270.
            result = Vector2.Angle(dirVector, new Vector2(-1, 0)) + 270;
        }

        // Those cases where the angle is between quadrants.
        // 0º
        else if (dirVector.x == 0 & dirVector.y == 1)
            result = 0;

        // 90º
        else if (dirVector.x == 1 & dirVector.y == 0)
            result = 90;

        // 180º
        else if (dirVector.x == 0 & dirVector.y == -1)
            result = 180;

        // 270º
        else if (dirVector.x == -1 & dirVector.y == 0)
            result = 270;


        return result;
    }
}
