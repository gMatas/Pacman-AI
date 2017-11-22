using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : Movement
{
    public const int STATE_MINIMISER = 0;
    public const int STATE_MAXIMISER = 1;
    public const int STATE_CHANCE = 2;
    public const int STATE_VALUELEAF = 3;

    private Vector2 playerPosition;
    public Node[] nextNodes = null;
    public Node bestnextnode = null;
    private Node parentNode = null;

	public int[] directionslist = null;
	public Vector2[] nextDirections = null;
	public Vector2 bestdirection;

    public int state;
    public double value;
     	

    public double alfa = double.NegativeInfinity;
    public double beta = double.PositiveInfinity;

    public void initiate(Vector2 playerPosition, int state)
    {
        if (state == STATE_MAXIMISER) value = alfa;
        if (state == STATE_MINIMISER) value = beta;
        if (state == STATE_CHANCE) value = beta;        // hardcoded chance node type
        this.state = state;
        this.playerPosition = playerPosition;
        getAllAvailableDirections(playerPosition);
        directionslist = getAllAvailableDirectionsAsIndexes();
        nextDirections = new Vector2[directionslist.Length];
        nextNodes = new Node[directionslist.Length];
    }

    public void setValue(double nodevalue)
    {
        value = nodevalue;
    }

    public void setNext(int direction, Node nextNode)
    {
        //print("direction=" + direction);
        //print("nextNodes=" + nextNodes.Length);
        nextNodes[direction] = nextNode;
    }

    public double getValue()
    {
        return value;
    }

    public int getNext()
    {
        return nextNodes.Length;
    }

    public Node getNext(int direction)
    {
        return nextNodes[direction];
    } 
    
    public int[] getAllAvailablePlayerActions()
    {
        // get all possible moves from where this player is        
        return directionslist;
    }

    public Vector2 getNextMove(int direction)
    /**
     * Returns the coordinates for the next movement.
     * Values at indexes:
     * 1 -> top
     * 2 -> bottom
     * 3 -> left
     * 4 -> right
     * **/
    {
        return getNextMove(playerPosition, direction);
    }

    public void updateAlfaBetaValues(Node other)
    {
        alfa = other.alfa;
        beta = other.beta;
    }

    public double calculateChanceNodeValue(int playerID, PacmanMovement controller = null)
    {
        double sum = 0.0;
        int bestIdx = 0;
        double minVal = double.PositiveInfinity;
        int n = nextNodes.Length;
        for (int i = 0; i < n; i++) // randame maziausia reiksme turinti kelia
        {
            Node node = nextNodes[i];
            if (node.value < minVal)
            {
                bestIdx = i;
                minVal = node.value;
            }              
        }
        float p = (controller != null) ? (1f / n) + (1f - (1f / n)) * controller.getProbabilityThatPlayerIsMinMax(playerID) : 0.65f;
        float p1 = (1f - p) / (n - 1);
        for (int i = 0; i < n; i++)
        {
            Node node = nextNodes[i];
            //double nodevalue = node.value;
            double nodevalue = Mathf.Pow((float) node.value, 2f);
            nodevalue = (node.value < 0) ? -1 : 1;
            if (i == bestIdx)
                nodevalue *= p;
            else
                nodevalue *= p1;
            sum += nodevalue;
        }
        return Mathf.Sqrt(Mathf.Abs((float) sum)) * ((sum < 0) ? -1 : 1);
    }

    public Node ParentNode
    {
        get
        {
            return parentNode;
        }

        set
        {
            parentNode = value;
        }
    }

    public Vector2 MaxDirection()
    {
        Node bestNode = null;
        double bestValue = double.NegativeInfinity;
        foreach (Node node in nextNodes)
        {
            if (node.value > bestValue)
            {
                bestNode = node;
                bestValue = node.value;
            }
        }
        return bestNode.playerPosition;
    }

    public void KillAllChildNodes(Node node)
    {
        if (node != null)
        {
            int n = node.nextNodes.Length;
            for (int i = 0; i < n; i++)
            {
                if (node.nextNodes[i] != null)
                {
                    node.KillAllChildNodes(node.nextNodes[i]);
                }                           
            }
            node.Kill();
        }
    }

    public void Kill()
    {
        Destroy(this);
    }

    public bool IsSameNext(Node other)
    {
        if (bestdirection == other.bestdirection && value == other.value)
            return true;
        return false;
    }
}