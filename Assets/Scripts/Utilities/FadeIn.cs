using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    bool _fadingIn = false;
    Texture2D blk;


    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        //bool fade;
        float alpha = 1f;

        //make a tiny black texture
        blk = new Texture2D(1, 1);
        blk.SetPixel(0, 0, new Color(0, 0, 0, 0));
        blk.Apply();

        _fadingIn = true;

        float fadeDuration = 1f;
        while (alpha > 0f)
        {
            blk.SetPixel(0, 0, new Color(0, 0, 0, alpha));
            blk.Apply();

            alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (_fadingIn)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blk);
    }
}
