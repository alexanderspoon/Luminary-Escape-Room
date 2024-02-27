using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawManager : MonoBehaviour
{
    public static int slotsFilled; 

    void Update()
    {
        // Check if the slots are full
        if (slotsFilled == 9)
        {
            // Fill in progression code here
            Debug.Log("The Puzzle is Finished!");
        }
    }
}

