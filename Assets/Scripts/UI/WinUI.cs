using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WinUI : MonoBehaviour
{
    public float speed = 2f;
    private bool _closing = false;

    void Update()
    {
        if (!_closing && Keyboard.current.fKey.wasReleasedThisFrame)
        {
            StartCoroutine(FadeOut());
            _closing = true;
        }
    }

    IEnumerator FadeOut()
    {
        CanvasGroup g = GetComponent<CanvasGroup>();

        while (g.alpha > 0f)
        {
            g.alpha -= speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        gameObject.SetActive(false);
    }
}
