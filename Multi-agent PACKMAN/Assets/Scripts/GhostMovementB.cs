using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovementB : Movement
{
    public bool doStart = false;
    public bool onStandby;
    
    public float animationSpeed;
    private Vector2 originPosition;
    private Vector2 targetPosition;
    private System.Random ran;

    // Use this for initialization
    void Start()
    {
        ran = new System.Random();
        // Set-up animation speed value
        if (animationSpeed < 0)
            animationSpeed = 0;
        else if (animationSpeed > 1)
            animationSpeed = 1;
        onStandby = !isAnimating();
    }

    public void FixedUpdate()
    {
        Vector2 playerPosition = transform.position;
        Vector2 nextPosition;      

        if ((onStandby && doStart) || (!onStandby && !doStart))
        {
            doStart = false;
            if (!isAnimating())
            {
                originPosition = playerPosition;
                getAllAvailableDirections(originPosition);
                targetPosition = moveRandom(originPosition);
                nextPosition = animateMovement(originPosition, playerPosition, targetPosition, animationSpeed);               
            }
            else
            {
                nextPosition = animateMovement(originPosition, playerPosition, targetPosition, animationSpeed);           
            }
            transform.position = nextPosition;
            onStandby = !isAnimating() || animationHalfwayDone();
        }        
    }

    private Vector2 moveRandom(Vector2 currentPosition)
    {
        int i = 0;
        int[] availableDirections = new int[4];
        if (Top)
            availableDirections[i++] = Movement.MOVE_TO_TOP;
        if (Bottom)
            availableDirections[i++] = Movement.MOVE_TO_BOTTOM;
        if (Left)
            availableDirections[i++] = Movement.MOVE_TO_LEFT;
        if (Right)
            availableDirections[i++] = Movement.MOVE_TO_RIGHT;
        if (i > 0)
        {          
            int ridx = ran.Next(i);
            Vector2 targetPosition;
            targetPosition = getNextMove(currentPosition, availableDirections[ridx]);
            return targetPosition;                            
        }
        else return currentPosition;
    }
}
