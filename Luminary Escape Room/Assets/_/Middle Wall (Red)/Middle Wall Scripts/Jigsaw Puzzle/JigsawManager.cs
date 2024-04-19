using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawManager : MonoBehaviour
{
    //Audio for Witch
    public AudioSource middleScreenAudio;
    public AudioClip witchIntro;
    public AudioClip fireScroll;

    public static int slotsFilled;
    [SerializeField] private GameObject frame;
    [SerializeField] private GameObject pieces;
    [SerializeField] private GameObject scroll;

    private void Awake()
    {
        frame.SetActive(true);
        pieces.SetActive(true);
        scroll.SetActive(false);

        //start witch intro
        middleScreenAudio.clip = witchIntro;
        middleScreenAudio.Play();
    }

    void Update()
    {
        // Check if the slots are full
        if (slotsFilled == 9)
        {
            // Fill in progression code here
            Debug.Log("The Puzzle is Finished!");
            frame.SetActive(false);
            pieces.SetActive(false);
            scroll.SetActive(true);

            //play fire scroll audio
            middleScreenAudio.clip = fireScroll;
            middleScreenAudio.Play();
        }
    }
}

