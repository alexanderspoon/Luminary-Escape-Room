using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> targets = new List<Transform>(); //list of targets to move to
    private NavMeshAgent agent; //navmesh agent component on the AI
    private GameObject player; //player object
    public bool readyToFollow; //bool that will activate AI movement after text/voice line plays explaining where the player is and what the AI does in this area
    private int increment; //increment for AI to go to the next target in the list
    [SerializeField] private float AIMoveAhead; //value for how far ahead the AI will go before waiting for the player to catch up

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); //grab navmesh agent component
        agent.updateRotation = false; //stops AI rotation
        agent.updateUpAxis = false; 
        player = GameObject.FindGameObjectWithTag("Player"); //grab player object
        readyToFollow = false; //initialize as false
    }

    private void Update()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) >= AIMoveAhead) //stops the AI if it goes too far ahead of the player
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        /*if (readyToFollow == true) //if the explanation for the AI's purpose in the invisible maze is complete then allow it to move to the current target
        {*/
            agent.SetDestination(targets[increment].position);

            if (targets[increment].GetComponent<Button>().pushed == true) //if the current target button is pushed, add 1 to increment to go to the next button
            {
                increment++;
                IncrementWait(); 
            }
        //}
    }

    IEnumerator IncrementWait() //wait a bit so increment doesn't add more than 1 at a time
    {
        yield return new WaitForSeconds(0.1f);
    }
}