using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiceContainerGraphic : MonoBehaviour
{
    public Text label;

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = gameObject.GetComponent<RectTransform>();

            return _rectTransform;
        }
    }

    private RectTransform _rectTransform;
}
