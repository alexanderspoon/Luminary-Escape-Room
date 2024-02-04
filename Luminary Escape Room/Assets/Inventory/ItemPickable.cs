using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=HGol5qhqjOE&ab_channel=FreedomCoding
//https://www.youtube.com/watch?v=Josw0x2geuQ&ab_channel=FreedomCoding

public class ItemPickable : MonoBehaviour, IPickable
{
    public ItemScriptableObject itemScriptableObject;

    public void PickItem()
    {
        Destroy(gameObject);
    }
}