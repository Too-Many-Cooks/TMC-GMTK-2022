using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasingFunctions
{
    // This class was made with the help from the following page
    // https://gist.github.com/Kryzarel/bba64622057f21a1d6d44879f9cd7bd4.
    // And this other list of Functions with animations: https://easings.net/


    // All of these easing equations are used through a common function with an enum parameter,
    // which allows other scripts to also pass the same enum as a parameter.
    // This eliminates the need in other scripts of creating a switch like the one in this script.
    public enum Functions
    {
        InSine, OutSine, InOutSine,
        InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint,
        InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc,
        InBack, OutBack, InOutBack,
        InElastic, OutElastic, InOutElastic,
        InBounce, OutBounce, InOutBounce
    }


    public static float ApplyEase(float x, Functions typeOfFunction)
    {
        switch (typeOfFunction)
        {
            // Sine functions:
            case Functions.InSine:
                return InSine(x);
            case Functions.OutSine:
                return OutSine(x);
            case Functions.InOutSine:
                return InOutSine(x);

            // Quad functions:
            case Functions.InQuad:
                return InQuad(x);
            case Functions.OutQuad:
                return OutQuad(x);
            case Functions.InOutQuad:
                return InOutQuad(x);

            // Cubic functions:
            case Functions.InCubic:
                return InCubic(x);
            case Functions.OutCubic:
                return OutCubic(x);
            case Functions.InOutCubic:
                return InOutCubic(x);

            // Quart functions:
            case Functions.InQuart:
                return InQuart(x);
            case Functions.OutQuart:
                return OutQuart(x);
            case Functions.InOutQuart:
                return InOutQuart(x);

            // Quint functions:
            case Functions.InQuint:
                return InQuint(x);
            case Functions.OutQuint:
                return OutQuint(x);
            case Functions.InOutQuint:
                return InOutQuint(x);

            // Expo functions:
            case Functions.InExpo:
                return InExpo(x);
            case Functions.OutExpo:
                return OutExpo(x);
            case Functions.InOutExpo:
                return InOutExpo(x);

            // Circ functions:
            case Functions.InCirc:
                return InCirc(x);
            case Functions.OutCirc:
                return OutCirc(x);
            case Functions.InOutCirc:
                return InOutCirc(x);

            // Back functions:
            case Functions.InBack:
                return InBack(x);
            case Functions.OutBack:
                return OutBack(x);
            case Functions.InOutBack:
                return InOutBack(x);

            // Elastic functions:
            case Functions.InElastic:
                return InElastic(x);
            case Functions.OutElastic:
                return OutElastic(x);
            case Functions.InOutElastic:
                return InOutElastic(x);

            // Bounce functions:
            case Functions.InBounce:
                return InBounce(x);
            case Functions.OutBounce:
                return OutBounce(x);
            case Functions.InOutBounce:
                return InOutBounce(x);
        }

        // If the switch does not return anything, something has gone wrong.
        Debug.LogWarning("Type of function " + typeOfFunction.ToString() + " not recognized in ApplyEase()'s code." +
                         "Returning 0.");
        return 0;
    }



    // All of these functions require a X input in the Range(0, 1).

    // This function is used to check the Range of the inputs of all the other functions in this code.
    static bool CheckXRange(float x)
    {
        bool result = false;

        if (x >= 0 & x <= 1)
            result = true;

        return result;
    }



    // Sine functions.
    static float InSine(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - Mathf.Cos((Mathf.PI * x) / 2);
        }

        else
        {
            Debug.LogWarning("Easing InSine function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutSine(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return Mathf.Sin((Mathf.PI * x) / 2);
        }

        else
        {
            Debug.LogWarning("Easing OutSine function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutSine(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutSine function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Quad functions.
    static float InQuad(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return x * x;
        }

        else
        {
            Debug.LogWarning("Easing InQuad function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutQuad(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InQuad(1 - x); ;
        }

        else
        {
            Debug.LogWarning("Easing OutQuad function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutQuad(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InQuad(x * 2) / 2;
            else
                return 1 - InQuad((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutQuad function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Cubic functions.
    static float InCubic(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return x * x * x;
        }

        else
        {
            Debug.LogWarning("Easing InCubic function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutCubic(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InCubic(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing OutCubic function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutCubic(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InCubic(x * 2) / 2;
            else
                return 1 - InCubic((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutCubic function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Quart functions.
    static float InQuart(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return x * x * x * x;
        }

        else
        {
            Debug.LogWarning("Easing InQuart function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutQuart(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InQuart(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing OutQuart function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutQuart(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InQuart(x * 2) / 2;
            else
                return 1 - InQuart((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutQuart function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Quint functions.
    static float InQuint(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return x * x * x * x * x;
        }

        else
        {
            Debug.LogWarning("Easing InQuint function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutQuint(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InQuint(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing OutQuint function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutQuint(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InQuint(x * 2) / 2;
            else
                return 1 - InQuint((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutQuint function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Expo functions.
    static float InExpo(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return (float)Mathf.Pow(2, 10 * (x - 1));
        }

        else
        {
            Debug.LogWarning("Easing InExpo function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutExpo(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InExpo(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing OutExpo function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutExpo(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InExpo(x * 2) / 2;
            else
                return 1 - InExpo((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutExpo function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Circ functions.
    static float InCirc(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
        }

        else
        {
            Debug.LogWarning("Easing InCirc function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutCirc(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InCirc(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing OutCirc function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutCirc(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InCirc(x * 2) / 2;
            else
                return 1 - InCirc((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutCirc function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Back functions.
    static float InBack(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return x * x * ((1.70158f + 1) * x - 1.70158f);
        }

        else
        {
            Debug.LogWarning("Easing InBack function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutBack(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - InBack(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing OutBack function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutBack(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InBack(x * 2) / 2;
            else
                return 1 - InBack((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutBack function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Elastic functions.
    static float InElastic(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - OutElastic(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing InElastic function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutElastic(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return (float)Mathf.Pow(2, -10 * x) * (float)Mathf.Sin((x - 0.3f / 4) * (2 * Mathf.PI) / 0.3f) + 1;
        }

        else
        {
            Debug.LogWarning("Easing OutElastic function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutElastic(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InElastic(x * 2) / 2;
            else
                return 1 - InElastic((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutElastic function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }


    // Bounce functions.
    static float InBounce(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            return 1 - OutBounce(1 - x);
        }

        else
        {
            Debug.LogWarning("Easing InBounce function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float OutBounce(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 1 / 2.75f)
            {
                return 7.5625f * x * x;
            }
            else if (x < 2 / 2.75f)
            {
                x -= 1.5f / 2.75f;
                return 7.5625f * x * x + 0.75f;
            }
            else if (x < 2.5 / 2.75f)
            {
                x -= 2.25f / 2.75f;
                return 7.5625f * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / 2.75f;
                return 7.5625f * x * x + 0.984375f;
            }
        }

        else
        {
            Debug.LogWarning("Easing OutBounce function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }
    static float InOutBounce(float x)
    {
        // If X is in the Range(0,1).
        if (CheckXRange(x))
        {
            if (x < 0.5)
                return InBounce(x * 2) / 2;
            else
                return 1 - InBounce((1 - x) * 2) / 2;
        }

        else
        {
            Debug.LogWarning("Easing InOutBounce function failed, returning 0. Parameter x == " + x + "." +
                             "/nParameter x must be in the (0,1) range.");
            return 0;
        }   // If X is not in the required Range(0,1), we return 0 and send a Warning.
    }

}
