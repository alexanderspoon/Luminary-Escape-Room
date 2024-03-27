using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timeRemaining;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject gameOverImage;
    [SerializeField] private bool timerActive;

    private void Awake()
    {
        timerActive = true;
        gameOverImage.SetActive(false);
    }

    void Update()
    {
        if (timerActive)
        {
            if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
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
        gameOverImage.SetActive(true);
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
