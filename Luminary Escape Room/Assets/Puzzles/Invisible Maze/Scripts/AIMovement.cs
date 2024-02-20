using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private Transform targetsParent; //empty game object holding targets
    [SerializeField] private List<Transform> targets = new List<Transform>(); //list of targets to move to
    private NavMeshAgent agent; //navmesh agent component on the AI
    private GameObject player; //player object
    //public bool readyToFollow; //bool that will activate AI movement after text/voice line plays explaining where the player is and what the AI does in this area
    private int increment; //increment for AI to go to the next target in the list
    [SerializeField] private float AIMoveAhead; //value for how far ahead the AI will go before waiting for the player to catch up

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); //grab navmesh agent component
        agent.updateRotation = false; //stops AI rotation
        agent.updateUpAxis = false; 
        player = GameObject.FindGameObjectWithTag("Player"); //grab player object
        //readyToFollow = false; //initialize as false

        //grabs all the patrolling points
        foreach (Transform child in targetsParent.transform)
        {
            targets.Add(child);
        }
        GotoNextPoint();
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
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
        //}
    }
    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (targets.Count == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = targets[increment].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        increment++;
    }
}