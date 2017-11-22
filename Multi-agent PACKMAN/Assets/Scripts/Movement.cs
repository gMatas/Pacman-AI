using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour {

    protected const int MOVE_TO_TOP = 0;
    protected const int MOVE_TO_BOTTOM = 1;
    protected const int MOVE_TO_LEFT = 2;
    protected const int MOVE_TO_RIGHT = 3;

    private bool top;
    private bool bottom;
    private bool left;
    private bool right;
    private string wallTag = "GameWall";
    private bool inProgress;
    private bool isHalfway;
    private bool isFirstHalf = true;

    protected bool Top
    {
        get
        {
            return top;
        }

        set
        {
            top = value;
        }
    }

    protected bool Bottom
    {
        get
        {
            return bottom;
        }

        set
        {
            bottom = value;
        }
    }

    protected bool Left
    {
        get
        {
            return left;
        }

        set
        {
            left = value;
        }
    }

    protected bool Right
    {
        get
        {
            return right;
        }

        set
        {
            right = value;
        }
    }


    protected bool isAnimating()
    {
        return inProgress;
    }

    protected bool animationHalfwayDone()
    {
        return isHalfway;
    }

    protected bool[] getAllAvailableDirections(Vector2 currentPosition)
    /**
     * Checks all four player directions and returns an array 
     * of possible movements, not obstraced by walls.
     * Values at indexes:
     * 1 -> top
     * 2 -> bottom
     * 3 -> left
     * 4 -> right
     * **/
    {
        bool[] availableDirections = { true, true, true, true };       
        for (int d = 0; d < 4; d++)
        {          
            Vector2 possiblePosition = getNextMove(currentPosition, d);
            Collider2D[] results = new Collider2D[10];
            Physics2D.OverlapPointNonAlloc(possiblePosition, results);
            foreach (Collider2D collider in results)
            {
                // checks if at the given point there is wall
                if (collider != null && collider.CompareTag(wallTag)) 
                {
                    availableDirections[d] = false;
                    break;
                }
            }
        }
        top = availableDirections[0];
        bottom = availableDirections[1];
        left = availableDirections[2];
        right = availableDirections[3];
        return availableDirections;
    }

    protected Vector2 getNextMove(Vector2 currentPosition, int direction)
    /**
     * Returns the coordinates for the next movement.
     * Values at indexes:
     * 1 -> top
     * 2 -> bottom
     * 3 -> left
     * 4 -> right
     * **/
    {
        float[,] possibleMovements = { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } };
        float newX = currentPosition.x + possibleMovements[ direction, 0 ];
        float newY = currentPosition.y + possibleMovements[ direction, 1 ];
        newX = Mathf.Round(newX);
        newY = Mathf.Round(newY);
        return new Vector2(newX, newY);
    }

    protected int[] getAllAvailableDirectionsAsIndexes()
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
            int[] newarray = new int[i];
            for (int ii = 0; ii < i; ii++)           
                newarray[ii] = availableDirections[ii];       
            availableDirections = newarray;
            return availableDirections;
        }       
        else
            return null;
    }

    protected Vector2 animateMovement(Vector2 origin, Vector2 current, Vector2 target, float speedScale)
    /**
     * Animates the given movement.
     * Returns Vector2 position at which the animating object is currently is.
     * **/
    {
        float originX = origin.x; float originY = origin.y;
        float targetX = target.x; float targetY = target.y;
        float currentX = current.x; float currentY = current.y;

        float fullLengthX = targetX - originX;
        float fullLengthY = targetY - originY;

        float remainingLengthX = targetX - currentX;
        float remainingLengthY = targetY - currentY;

        float numberOfSteps = 1000f;    // Suskaidytos animacijos segmentu skaicius
        float speed = numberOfSteps * speedScale;    // Animacijos greitis (apskaiciuotas suskaidytos animacijos segmentu skaicius)

        float stepSizeX = (fullLengthX / numberOfSteps) * speed;
        float stepSizeY = (fullLengthY / numberOfSteps) * speed;

        float allowedErrorX = stepSizeX * 0.1f;
        float allowedErrorY = stepSizeY * 0.1f;
        allowedErrorX = (allowedErrorX < 0) ? (-1 * allowedErrorX) : allowedErrorX;
        allowedErrorY = (allowedErrorY < 0) ? (-1 * allowedErrorY) : allowedErrorY;     

        // Object reached its goal & Animation is finished:
        if ( (remainingLengthX <= allowedErrorX && remainingLengthX >= allowedErrorX * -1) && 
                (remainingLengthY <= allowedErrorY && remainingLengthY >= allowedErrorY * -1) )
        {                    
            inProgress = false;
            isHalfway = false;
            isFirstHalf = true;
            return target;                             
        }  
        else // Animation is in progress:
        {    
            inProgress = true;
            if (isFirstHalf && checkIfAnimationIsHalfwayDone( // Animation is halfway done:
                new Vector2(fullLengthX, fullLengthY), new Vector2(remainingLengthX, remainingLengthY), 0.05f) )
            {
                isHalfway = true;
                isFirstHalf = false;
                float halfwayX = (stepSizeX != 0) ? Mathf.Floor(currentX) + 0.5f : currentX;
                float halfwayY = (stepSizeY != 0) ? Mathf.Floor(currentY) + 0.5f : currentY;
                Vector2 halfway = new Vector2(halfwayX, halfwayY);               
                return halfway;
            }              
            else
            {
                isHalfway = false;
                float nextX = currentX + stepSizeX;
                float nextY = currentY + stepSizeY;
                Vector2 next = new Vector2(nextX, nextY);               
                return next;
            }
                
        }

    }

    private bool checkIfAnimationIsHalfwayDone(Vector2 fullLength, Vector2 remainingLength, float allowedError = 0.1f)
    {
        fullLength.x = Mathf.Abs(fullLength.x);
        fullLength.y = Mathf.Abs(fullLength.y);
        remainingLength.x = Mathf.Abs(remainingLength.x);
        remainingLength.y = Mathf.Abs(remainingLength.y);

        Vector2 dif = fullLength - remainingLength;
        dif.x = Mathf.Abs(dif.x);
        dif.y = Mathf.Abs(dif.y);

        bool halfwayCheck = (
            (dif.x >= 0.5f - allowedError) && (dif.x <= 0.5f + allowedError) ||
            (dif.y >= 0.5f - allowedError) && (dif.y <= 0.5f + allowedError)
            ) ? true : false;
        return halfwayCheck;
     }
}