using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public Button optionsButton;
    public Button optionsBackButton;
    public GameObject creditsPanel;
    public Button creditsButton;
    public Button creditsBackButton;

    AudioSource src;

    public Animator transition;
    public float transitionTime = 1f;
    private Texture2D blk;
    private bool _fadingToBlack;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        src = GetComponent<AudioSource>();
    }

    public void ShowOptions(bool show) 
    {
        optionsPanel.SetActive(show);
        ((show) ? optionsBackButton : optionsButton).Select();
    }

    public void ShowCredits(bool show) 
    {
        creditsPanel.SetActive(show);
        ((show) ? creditsBackButton : creditsButton).Select();
    }

    public void Play() 
    {
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelIndex) 
    {
        //transition.SetTrigger("Start");
        StartCoroutine(FadeOutCoroutine());
        yield return new WaitForSeconds(transitionTime);

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(sceneBuildIndex: levelIndex);
    }

    private IEnumerator FadeOutCoroutine()
    {
        //bool fade;
        float alpha = 0f;

        //make a tiny black texture
        blk = new Texture2D(1, 1);
        blk.SetPixel(0, 0, new Color(0, 0, 0, 0));
        blk.Apply();

        _fadingToBlack = true;

        float fadeDuration = transitionTime;
        while (alpha < 1f)
        {
            blk.SetPixel(0, 0, new Color(0, 0, 0, alpha));
            blk.Apply();

            alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    private void OnGUI()
    {
        if (_fadingToBlack)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blk);
    }

    public void PlaySound(AudioClip clip)
    {
        src.PlayOneShot(clip);
    }
    public void MainMenu()
    {
        StartCoroutine(LoadLevel(0));
    }
}
