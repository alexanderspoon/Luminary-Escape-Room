using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed; //player move speed
    [SerializeField] private float distanceFromMouse; //a value used to check if the player has gotten close enough to the mouse input position
    private Rigidbody2D rb; //2D rigidbody on player
    private Vector3 mousePos; //Vector3 of where the mouse was clicked on screen
    private Vector3 mouseDir; //Vector3 of the direction from the player to the mouse

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //set the rigidbody
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) //grabs mouse position and calculates the direction the player needs to move towards that position
        {
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.Log(mousePos);
            mouseDir = mousePos - gameObject.transform.position;
            mouseDir = new Vector3(mouseDir.x, mouseDir.y, 0);
            mouseDir = mouseDir.normalized;
        }

        rb.AddForce((mouseDir) * moveSpeed * Time.smoothDeltaTime); //moves player to mouse position
        
        if (Vector3.Distance(this.transform.position, mousePos) <= distanceFromMouse) //stops the player if it gets within range of the set distance to the mouse position
        {
            rb.velocity = Vector3.zero;
            this.transform.position = mousePos;
            mouseDir = new Vector3(0,0,0);
        }
    }
}