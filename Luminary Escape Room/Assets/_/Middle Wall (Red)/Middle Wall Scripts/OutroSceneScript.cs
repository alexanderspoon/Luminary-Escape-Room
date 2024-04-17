using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroSceneScript : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    [SerializeField] private AudioClip loseAudio;
    [SerializeField] private AudioClip winAudio1;
    [SerializeField] private AudioClip winAudio2;
    [SerializeField] private AudioClip winAudio3;
    [SerializeField] private SocketWork server;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        credits.SetActive(false);
        server = FindObjectOfType<SocketWork>();
        /*if (server.green == true && server.blue == true)
        {
            StartCoroutine(playWinAudio());
        }
        else
        {
            StartCoroutine(playLoseAudio());
        }*/

    }
    private IEnumerator playWinAudio()
    {
        audioSource.clip = winAudio1;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.clip = winAudio2;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.clip = winAudio3;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        credits.SetActive(true);
    }

    private IEnumerator playLoseAudio()
    {
        audioSource.clip = loseAudio;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        credits.SetActive(true);
    }
}