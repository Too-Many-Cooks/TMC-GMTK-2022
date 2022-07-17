using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseDecreaseUI : MonoBehaviour
{
    RectTransform rectTransform;
    Vector3 originalScale, originalPos;
    int repeatCounter = 0;
    float timer = 0;
    bool dir = true;
    [SerializeField] Vector3 targetDisplacement = Vector3.zero;
    [SerializeField] float targetScaleMultiplier = 1.5f;
    [SerializeField] float duration = 1.5f;
    [SerializeField] int repeats = 2;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPos = rectTransform.localPosition;
    }

    public void StartHighlight()
    {
        repeatCounter = 0;
        dir = true;
        timer = 0;

        StartCoroutine(Highlight());
    }

    IEnumerator Highlight()
    {
        while(repeats != repeatCounter)
        {
            UpdateTimer();
            rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * targetScaleMultiplier, 
                EasingFunctions.ApplyEase(timer / duration, EasingFunctions.Functions.InOutBack));

            rectTransform.localPosition = Vector3.Lerp(originalPos, originalPos + targetDisplacement,
                EasingFunctions.ApplyEase(timer / duration, EasingFunctions.Functions.InOutBack));

            yield return(null);
        }

        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPos;
    }

    void UpdateTimer()
    {
        if (dir)
        {
            timer += Time.deltaTime;

            if(timer >= duration)
            {
                timer = duration;
                dir = false;
            }
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                dir = true;
                repeatCounter++;
            }
        }
    }
}
