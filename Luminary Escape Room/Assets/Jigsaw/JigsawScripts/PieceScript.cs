using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    public string pieceIdentifier;
    public string objectName;
    public string destinationName;

    private GameObject selectedCube;
    private GameObject selectedSphere;
    private bool isMoving = false;
    private float startTime;
    private float journeyLength;
    private Vector3 startPos;
    public float pieceSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Identify Piece Group
        pieceIdentifier = gameObject.name;

        // Identify Call Tag
        objectName = "Cube" + gameObject.name;

        // Identify Destination Tag
        destinationName = "Sphere" + pieceIdentifier;

        // Print the name to the console
        //Debug.Log("Initializing group: " + pieceIdentifier);
        //Debug.Log("Initializing piece name: " + objectName);
        //Debug.Log("Initializing piece destination: " +  destinationName);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray hits an object
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag(objectName))
                {
                    // Select the cube
                    selectedCube = hitObject;
                }
                else if (hitObject.CompareTag(destinationName))
                {
                    // Select the sphere
                    selectedSphere = hitObject;

                    // If both cube and sphere are selected
                    if (selectedCube != null)
                    {
                        //Count the section of the puzzle as completed
                        JigsawManager.slotsFilled++;
                        Debug.Log("Puzzle Completion: " + JigsawManager.slotsFilled + "/9");

                        // Start moving the cube towards the sphere
                        isMoving = true;
                        startTime = Time.time;
                        startPos = selectedCube.transform.position;
                        journeyLength = Vector3.Distance(startPos, selectedSphere.transform.position);
                    }
                }
            }
        }

        // If cube is moving
        if (isMoving)
        {
            // Calculate the journey length and move the cube towards the sphere
            float distCovered = (Time.time - startTime) * pieceSpeed; // Change 1f to adjust speed
            float fracJourney = distCovered / journeyLength;
            selectedCube.transform.position = Vector3.Lerp(startPos, selectedSphere.transform.position, fracJourney);

            // If the cube has reached the sphere
            if (fracJourney >= 1.0f)
            {
                isMoving = false;
            }
        }
    }
}
