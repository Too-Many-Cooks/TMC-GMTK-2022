using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameOverManager : MonoBehaviour
{
    AudioSource src;

    public Animator transition;
    public float transitionTime = 1f;

    private void Start()
    {
        src = GetComponent<AudioSource>();
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
}
