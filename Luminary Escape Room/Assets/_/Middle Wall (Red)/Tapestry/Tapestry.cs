using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tapestry : MonoBehaviour
{
    public Texture[] stageOne;
    public Texture[] stageTwo;
    public Texture[] stageThree;
    public int framesPerSecond = 10;

    [SerializeField] private float stageTimeRemaining;

    void Update()
    {
        stageTimeRemaining -= Time.deltaTime;
        if (stageTimeRemaining <= 480 && stageTimeRemaining > 180)
        {
            int index = (int)(Time.time * framesPerSecond) % stageOne.Length;
            GetComponent<Renderer>().material.mainTexture = stageOne[index];
        }
        else if (stageTimeRemaining <= 180 && stageTimeRemaining > 90)
        {
            int index = (int)(Time.time * framesPerSecond) % stageTwo.Length;
            GetComponent<Renderer>().material.mainTexture = stageTwo[index];
        }
        else if (stageTimeRemaining <= 90)
        {
            int index = (int)(Time.time * framesPerSecond) % stageThree.Length;
            GetComponent<Renderer>().material.mainTexture = stageThree[index];
        }

    }
}