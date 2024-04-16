using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public AudioSource doorAudio;
    public AudioClip witchIntro2;

    private bool open = false;
    [SerializeField] private GameObject invisibleMaze;
    [SerializeField] private GameObject spellScroll;

    private void Awake()
    {
        spellScroll.SetActive(false);

        doorAudio.Play();
    }

    void Update()
    {
        if (open == true)
        {
            spellScroll.SetActive(true);
            invisibleMaze.SetActive(false);

            doorAudio.clip = witchIntro2;
            doorAudio.Play();
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
