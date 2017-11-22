using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovementA : Movement {

    public bool doStart = false;
    public bool onStandby;

    public float animationSpeed; 
    private Vector2 originPosition;
    private Vector2 targetPosition;
    private GameObject[] playerList;


    // Use this for initialization
    void Start () {
        // Set-up animation speed value
        if (animationSpeed < 0)
            animationSpeed = 0;
        else if (animationSpeed > 1)
            animationSpeed = 1;
        getPlayerList();
        onStandby = !isAnimating();
    }
	
	// Update is called once per frame
	public void FixedUpdate ()
    {
        Vector2 playerPosition = transform.position;
        Vector2 nextPosition;

        if ((onStandby && doStart) || (!onStandby && !doStart))
        {
            doStart = false;
            if (!isAnimating())
            {
                originPosition = playerPosition;
                MinMax tree = gameObject.GetComponent<MinMax>();
                tree.initiate(playerList);
                targetPosition = tree.getOptimalTarget();
                nextPosition = animateMovement(originPosition, playerPosition, targetPosition, animationSpeed);
            }
            else
            {
                // TODO: Check if the current player position is the same as of pacman
                nextPosition = animateMovement(originPosition, playerPosition, targetPosition, animationSpeed);
            }
            transform.position = nextPosition;
            onStandby = !isAnimating() || animationHalfwayDone();
        }   
    }



    private void getPlayerList()
    {    
        GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        int n = 1 + ghosts.Length;
        GameObject[] players = new GameObject[n];
        players[0] = gameObject;
        int i = 1;
        foreach (GameObject g in ghosts)    
            if (!gameObject.Equals(g))         
                players[i++] = g;
        players[i++] = pacman;
        this.playerList = players;
    }
}
