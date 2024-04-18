using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    //audio var
    public AudioSource middleScreenAudio;
    public AudioClip timeScroll;

    private bool open = false;
    [SerializeField] private GameObject invisibleMaze;
    [SerializeField] private GameObject spellScroll;

    private void Awake()
    {
        spellScroll.SetActive(false);
    }

    void Update()
    {
        if (open == true)
        {
            spellScroll.SetActive(true);
            invisibleMaze.SetActive(false);

            //audio time scroll trigger
            middleScreenAudio.clip = timeScroll;
            middleScreenAudio.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            open = true;
        }
    }
}
