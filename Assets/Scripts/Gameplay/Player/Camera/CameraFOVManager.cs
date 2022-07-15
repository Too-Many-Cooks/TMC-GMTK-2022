using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera myCamera;   // The character's camara, used to control the field of view.


    [Header("FOV Values")]
    [SerializeField] [Range(1, 100)] float highFOV = 75;        // (it should be HIGHER that the originalFOV).
    [SerializeField] [Range(1, 100)] float lowFOV = 45;         // (it should be LOWER that the originalFOV).
    [SerializeField] [Range(1, 100)] float lowestFOV = 35;      // (it should be LOWER that the originalFOV and lowFOV).

    float originalFOV;                                          // It stores the original player FOV.


    [Header("Transition times")]    // Used to calculate transition times based on the lenght of the transition.
    [SerializeField] [Range(0.01f, 1)] float minDuration = 0.1f;
    [SerializeField] [Range(0.01f, 1)] float maxDuration = 0.5f;


    // We create an enum to track the current state of FOV.
    public enum levelsOfFOV { FOV_original, FOV_high, FOV_low, FOV_lowest };

    // This variable keeps track of the target FOV to have.
    // If the target FOV is other than the current FOV, the FOV changes in the LateUpdate() method.
    levelsOfFOV targetFOV = levelsOfFOV.FOV_original;


    // We also need a list of commands to keep track of them.
    List<FOVCommand> myCommands = new List<FOVCommand>();



    class FOVCommand
    {
        // Storing values to perform the Lerp between different FOV values.
        public float initialFOV;
        public float targetFOV;
        public float durationOfTransition;
        public float transitionTimer = 0;

        public FOVCommand(float FOV_initial, float FOV_target, float duration)
        {
            initialFOV = FOV_initial;
            targetFOV = FOV_target;
            durationOfTransition = duration;
        }


        // This function updates the transitionTimer.
        // It also clamps it so that it does not exceed the value of durationOfTransition.
        public void UpdateTimer()
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer > durationOfTransition)
            {
                transitionTimer = durationOfTransition;
            }
        }


        // This function will return the current FOV according to this order.
        public float ObtainFOV()
        {
            float percentageOfCompletion =     // We calculate the percentage and apply an easing function to it.
                EasingFunctions.ApplyEase(transitionTimer / durationOfTransition, EasingFunctions.Functions.InOutSine);

            float result = Mathf.Lerp(initialFOV, targetFOV, percentageOfCompletion);

            return result;
        }


        // This function is used to prematurely end a command.
        public void EndCommand()
        {
            transitionTimer = durationOfTransition;
        }
    }




    private void Start()
    {
        // Storing the original field of view of the camera.
        originalFOV = myCamera.fieldOfView;


        // And we then check that all of our FOV values are correct (Ex. We check that lowFOV is not higher than originalFOV).
        if (highFOV < originalFOV)
            Debug.LogWarning("highFOV value is SMALLER than the original camera FOV.");
        if (lowFOV > originalFOV)
            Debug.LogWarning("lowFOV value is BIGGER than the original camera FOV.");
        if (lowestFOV > originalFOV)
            Debug.LogWarning("lowestFOV value is BIGGER than the original camera FOV.");
        if (lowestFOV > lowFOV)
            Debug.LogWarning("lowestFOV value is BIGGER than lowFOV value.");
    }


    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ChangeFOV(levelsOfFOV.FOV_high);
        else if (Input.GetKeyDown(KeyCode.N))
            ChangeFOV(levelsOfFOV.FOV_original);
        else if (Input.GetKeyDown(KeyCode.B))
            ChangeFOV(levelsOfFOV.FOV_low);
        else if (Input.GetKeyDown(KeyCode.V))
            ChangeFOV(levelsOfFOV.FOV_lowest);



        // If our command list is not empty, we are still transitioning.
        if (myCommands.Count > 0)
        {
            CheckForFinishedFunctions();
            UpdateAllTimers();
            CalculateFOVValue();
        }
    }


    // Used to return the value of any FOV contained in this script.
    float FovValue(levelsOfFOV fov)
    {
        switch (fov)
        {
            case levelsOfFOV.FOV_original:
                return originalFOV;

            case levelsOfFOV.FOV_high:
                return highFOV;

            case levelsOfFOV.FOV_low:
                return lowFOV;

            case levelsOfFOV.FOV_lowest:
                return lowestFOV;
        }

        Debug.LogError("FOV level not recognized. Returning 1 FOV");
        return 1;
    }


    // This function calculates the time it will take to change from one FOV to another.
    float CalculateTimeToTransition(float initialFOV, float targetFOV)
    {
        float maxFOVChange = highFOV - lowestFOV;

        float currentChange = Mathf.Abs(initialFOV - targetFOV);

        float result = Mathf.Lerp(minDuration, maxDuration,
                       EasingFunctions.ApplyEase(currentChange / maxFOVChange, EasingFunctions.Functions.OutCubic));

        return result;
    }


    // This function is used to log a FOV command.
    public void ChangeFOV(levelsOfFOV commandedTarget)
    {
        float fromFOV;  // Holds the start of our transition.

        // We also check for command overloads (we can't handle infinite commands).
        // If there is overload, we delete all previous commands,, and set the initial FOV to our current FOV.
        if (myCommands.Count > 25)
        {
            myCommands.Clear();

            fromFOV = myCamera.fieldOfView;
        }
        else // Without overload, we give our last targetFOV as initial FOV for our command.
        {
            fromFOV = FovValue(targetFOV);
        }


        if (fromFOV != FovValue(commandedTarget))   // We don't want redundant orders.
        {
            // We store the lenght of the transition.
            float lenght = CalculateTimeToTransition(fromFOV, FovValue(commandedTarget));

            // We create the command.
            FOVCommand command = new FOVCommand(fromFOV, FovValue(commandedTarget), lenght);

            // We replace the targetFOV with the new FOV.
            targetFOV = commandedTarget;

            // And we add this command to our commands list.
            myCommands.Add(command);
        }
    }


    // This function checks all commands for finished ones. If it finds one, it clears all previous commands.
    void CheckForFinishedFunctions()
    {
        // We need to create an int to check if we need to eliminate any functions
        // (we can't eliminate in the middle of the for loop).
        int commandsToEliminate = 0;

        for (int i = 0; i < myCommands.Count; i++)
        {
            // If the timer is filled.
            if (myCommands[i].transitionTimer == myCommands[i].durationOfTransition)
            {
                commandsToEliminate = i + 1;  // We store that we have to eliminate i +1 commands (because i starts at 0).
            }
        }


        if (commandsToEliminate != 0)
        {
            for (int i = commandsToEliminate; i > 0; i--)
            {
                myCommands.RemoveAt(0);
            }
        }
    }


    // This function updates all of the timers in our commands.
    void UpdateAllTimers()
    {
        foreach (FOVCommand command in myCommands)
        {
            command.UpdateTimer();
        }
    }


    // This function sets the value of the FOV, by lerping between all the different commands.
    void CalculateFOVValue()
    {
        // If we only have 1 command, the change is easy.
        if (myCommands.Count == 1)
            myCamera.fieldOfView = myCommands[0].ObtainFOV();

        // If we have more, we need to lerp between values.
        else if (myCommands.Count > 1)
        {
            // We first create a variable to hold the last lerped value.
            // We give it the first command's value.
            float lastLerpedValue = myCommands[0].ObtainFOV();

            // We set up a loop that will lerp all values in order of command.
            for (int i = 0; i + 1 < myCommands.Count; i++)
            {
                // We first store both commands.
                FOVCommand formerCommand = myCommands[i];
                FOVCommand latterCommand = myCommands[i + 1];


                // At each loop, we need to define the common passedTime and finishingTime.
                // In other words, how much time has already passed and how much time it takes to transition.

                // Commands are ordered by arrival, so the earliest command will always be higher in the index.
                // Thus, we take our time from the second command.
                float timePassed = latterCommand.transitionTimer;


                // The finishing time will be the smaller duration of the transition between both commands.
                float finishingTime;

                if (formerCommand.durationOfTransition < latterCommand.durationOfTransition)
                    finishingTime = formerCommand.durationOfTransition;
                else
                    finishingTime = latterCommand.durationOfTransition;


                lastLerpedValue = Mathf.Lerp(lastLerpedValue, latterCommand.ObtainFOV(),
                    EasingFunctions.ApplyEase(timePassed / finishingTime, EasingFunctions.Functions.InOutSine));
            }

            // The result is the lerped curve, which produces a smooth transition between orders.
            myCamera.fieldOfView = lastLerpedValue;
        }
    }
}
