using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    //Audio Var
    public AudioSource musicAudioSource;
    public AudioClip musicClip;
    
    [SerializeField] private float timeRemaining;
    [SerializeField] private GameObject timerText;
    //[SerializeField] private GameObject gameOverImage;
    [SerializeField] private bool timerActive;

    [SerializeField] private SocketWork server;

    private void Awake()
    {
        server = FindObjectOfType<SocketWork>();
        timerActive = true;
    }

    void Update()
    {
        if(server.green == true && server.blue == true)
        {
            timerEnded();
        }

        if (timerActive)
        {
            if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);

                //start music audio
                musicAudioSource.clip = musicClip;
                musicAudioSource.Play();
            }
            else
            {
                timeRemaining = 0;
                timerActive = false;
                timerEnded();
            }
        }
    }

    void timerEnded()
    {
        SceneManager.LoadScene("Outro Scene");
        //gameOverImage.SetActive(true);
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
