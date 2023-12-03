using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool pushed; //public bool to be called by AI movement and door opening scripts
    private GameObject player; //player object
    [SerializeField] private float distanceFromPlayer; //value used to check if the player is close enough to the button

    void Start()
    {
        pushed = false; //initialize as false
        player = GameObject.FindGameObjectWithTag("Player"); //grab player object
    }

    void Update()
    {
        if(Vector3.Distance(this.transform.position, player.transform.position) <= distanceFromPlayer && pushed == false) //if player is close enough, disable sprite and set pushed to true
        {
            pushed = true;
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}