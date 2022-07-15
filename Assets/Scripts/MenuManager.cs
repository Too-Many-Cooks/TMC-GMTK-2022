using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private void Start()
    {
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
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneBuildIndex: levelIndex);
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
