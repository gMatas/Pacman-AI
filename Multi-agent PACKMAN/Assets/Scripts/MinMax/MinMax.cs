using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{
    public const string pacmanTag = "Pacman";
    public const string ghostTag = "Ghost";
    public const string candyTag = "Candy";

    public Vector2 targetDirection;
    public int levels;
    public Vector2[] path;
    public int[] directions;

    private Node root;
    public GameObject[] playerObjects;
    private int iterator1 = 0;  // iterator for playerObjects
    private int leveldepth;

    public void initiate(GameObject[] players)
    {
        playerObjects = players;
        leveldepth = players.Length;

        path = new Vector2[(leveldepth * levels) + leveldepth + 1];
        initialiseTreePath();

        int depth = 0;
        GameObject firstplayer = getNextPlayerObject(true, true);       
        root = gameObject.AddComponent<Node>();
        root.initiate(firstplayer.transform.position, calculateNodeState(firstplayer));
        directions = root.directionslist;
        root = populateTree(root, depth, leveldepth, levels);
        targetDirection = root.bestdirection;
        root.Kill();
    }

    public Vector2 getOptimalTarget()
    {
        return targetDirection;
    }

    private void initialiseTreePath()
    {
        int n = playerObjects.Length;
        for (int i = 0; i < n; i++)
        {
            path[i].x = Mathf.Round(playerObjects[i].transform.position.x);
            path[i].y = Mathf.Round(playerObjects[i].transform.position.y);
        }
            
    }

    private Node populateTree(Node parent, int depth, int leveldepth, int levels)
    {
        if (depth % (leveldepth * levels) == 0 && depth > 0)
        {
            // populate final depth nodes with heuristics nodes
            int[] actions;
            actions = parent.getAllAvailablePlayerActions();
            for (int i = 0; i < actions.Length; i++)
            {
                if (parent.alfa >= parent.beta)
                {
                    print("PRUNNING at depth: " + depth);
                    break;
                }
                else
                {
                    int action = actions[i];
                    Vector2 nextmove = parent.getNextMove(action);
                    path[depth + leveldepth] = nextmove;

                    // use heuristics to evaluate current position
                    double nodevalue = heuristicEvaluation();

                    Node child = gameObject.AddComponent<Node>();
                    child.initiate(nextmove, Node.STATE_VALUELEAF);
                    child.setValue(nodevalue);
                    parent.setNext(i, child); 

                    if (parent.state == Node.STATE_MAXIMISER) // parent => maximiser
                    {
                        if (child.getValue() > parent.alfa)
                        {
                            parent.alfa = child.getValue();
                            parent.bestdirection = nextmove;
                        }
                        if (child.getValue() > parent.getValue())                           
                            parent.setValue(child.getValue());    
                    }
                    if (parent.state == Node.STATE_MINIMISER) // parent => minimiser
                    {
                        if (child.getValue() < parent.beta)
                        {
                            parent.beta = child.getValue();              
                            parent.bestdirection = nextmove;
                        }
                        if (child.getValue() < parent.getValue())
                            parent.setValue(child.getValue());
                    }
                    child.Kill();
                }
            }
        }
        else
        {
            // move down the tree recursively           
            int[] actions;
            actions = parent.getAllAvailablePlayerActions();
            for (int i = 0; i < actions.Length; i++)
            {
                if (parent.alfa >= parent.beta)
                {
                    print("PRUNNING at depth: " + depth);
                    break;
                }
                else
                {
                    int action = actions[i];
                    Vector2 nextmove = parent.getNextMove(action);
                    path[depth + leveldepth] = nextmove;

                    GameObject nextplayer = getNextPlayerObject();
                    int nextnodestate = calculateNodeState(nextplayer);

                    Node child = gameObject.AddComponent<Node>();
                    child.initiate(path[depth + 1], nextnodestate);
                    child.updateAlfaBetaValues(parent);
                    child = populateTree(child, depth + 1, leveldepth, levels); // recursively go down the tree     
                    parent.setNext(i, child);

                    if (parent.state == Node.STATE_MAXIMISER) // parent => maximiser
                    {
                        if (child.getValue() > parent.alfa)
                        {
                            parent.alfa = child.getValue();
                            parent.bestdirection = nextmove;
                        }
                        if (child.getValue() > parent.getValue())
                            parent.setValue(child.getValue());
                    }
                    if (parent.state == Node.STATE_MINIMISER) // parent => minimiser
                    {
                        if (child.getValue() < parent.beta)
                        {
                            parent.beta = child.getValue();
                            parent.bestdirection = nextmove;
                        }
                        if (child.getValue() < parent.getValue())
                            parent.setValue(child.getValue());
                    }
                    child.Kill();
                }        
            }
        }
        getNextPlayerObject(false);         
        return parent;
    }

    private double heuristicEvaluation()
    {
        // h1 -- sum of move penalties
        // h2 -- sum of candy bonus
        // h3 -- defeat penalty
        // h4 -- victory bonus
        // he -- complete heuristic evaluation

        bool isPacman = gameObject.CompareTag(pacmanTag);

        /**
         h1 -- sum of maximiser move penalties: */
        double movepenalty = -1;
        double h1 = movepenalty * levels;
        if (isPacman)
            h1 += movepenalty;

        /**
         h2 -- sum of candy bonus: */
        int numOfcandies = 0;
        int numOfEatenCandies = 0;
        for (int pi = 0; pi < playerObjects.Length; pi++)
            if (playerObjects[pi].CompareTag(pacmanTag))
            {
                GameObject[] candies = GameObject.FindGameObjectsWithTag(candyTag);
                numOfcandies = candies.Length;
                for (int lv = 1; lv < levels + 1 + ((isPacman) ? 1 : 0); lv++)
                {
                    int i = pi + (leveldepth * lv);

                    foreach (GameObject c in candies)
                    {
                        if (c.transform.position.x == path[i].x &&
                            c.transform.position.y == path[i].y)
                        {
                            numOfEatenCandies++;
                        }
                    }
                }
                break;
            }
        double candybonus = 10;
        double h2 = candybonus * numOfEatenCandies;

        /**
         h3 -- defeat penalty: */
        bool defeat = false;
        for (int pacmanGOIdx = 0; pacmanGOIdx < playerObjects.Length; pacmanGOIdx++)
        {
            if (playerObjects[pacmanGOIdx].CompareTag(pacmanTag))
            {
                if (isPacman)
                {
                    for (int lv = 1; lv < levels + 2; lv++)
                    {
                        int pacmanPathIdx = pacmanGOIdx + (leveldepth * lv);
                        for (int ghostPathIdx = pacmanPathIdx - leveldepth + 1; ghostPathIdx < pacmanPathIdx; ghostPathIdx++)
                        {
                            if (path[pacmanPathIdx].Equals(path[ghostPathIdx]))
                            {
                                defeat = true;
                                break;
                            }
                        }
                        if (lv < levels + 1)
                            for (int ghostPathIdx = pacmanPathIdx + 1; ghostPathIdx < pacmanPathIdx + leveldepth; ghostPathIdx++)
                            {
                                if (path[pacmanPathIdx].Equals(path[ghostPathIdx]))
                                {
                                    defeat = true;
                                    break;
                                }
                            }
                        if (defeat) break;
                    }
                }
                else
                {
                    for (int lv = 1; lv < levels + 1; lv++)
                    {
                        int pacmanPathIdx = pacmanGOIdx + (leveldepth * lv);
                        for (int ghostPathIdx = pacmanPathIdx - leveldepth + 1; ghostPathIdx < pacmanPathIdx; ghostPathIdx++)
                        {
                            int ghostPrevPathIdx = ghostPathIdx - leveldepth;
                            if (path[pacmanPathIdx].Equals(path[ghostPathIdx]) && path[pacmanPathIdx].Equals(path[ghostPrevPathIdx]))
                            {
                                defeat = true;
                                break;
                            }
                        }
                        if (lv == levels)
                        {
                            int ghostLastPathIdx = leveldepth * (lv + 1);
                            if (path[pacmanPathIdx].Equals(path[ghostLastPathIdx]))
                            {
                                defeat = true;
                                break;
                            }
                        }
                    }
                }
                break;
            }
        }
        // TODO: jei pacmanas ir vaiduoklis uzimima ta pacia vieta

        // TODO: jei packmanas ir vaiduoklis apsimaino vietomis
        for (int pacmanGOIdx = 0; pacmanGOIdx < playerObjects.Length; pacmanGOIdx++)
        {
            if (playerObjects[pacmanGOIdx].CompareTag(pacmanTag))
            {
                if (isPacman)
                {
                    for (int lv = 1; lv < levels + 2; lv++)
                    {
                        int pacmanPrevPathIdx = pacmanGOIdx + (leveldepth * lv) - leveldepth;
                        int pacmanNextPathIdx = pacmanGOIdx + (leveldepth * lv);
                        for (int ghostNextPathIdx = pacmanNextPathIdx + 1; ghostNextPathIdx < pacmanNextPathIdx + leveldepth; ghostNextPathIdx++)
                        {
                            int ghostPrevPathIdx = ghostNextPathIdx - leveldepth;
                            if (path[pacmanNextPathIdx].Equals(path[ghostPrevPathIdx]) && path[pacmanPrevPathIdx].Equals(path[ghostNextPathIdx]))
                            {
                                defeat = true;
                                break;
                            }
                        }
                        if (defeat) break;
                    }
                }
                else
                {
                    for (int lv = 1; lv < levels + 1; lv++)
                    {
                        int pacmanPrevPathIdx = pacmanGOIdx + (leveldepth * lv) - leveldepth;
                        int pacmanNextPathIdx = pacmanGOIdx + (leveldepth * lv);
                        for (int ghostNextPathIdx = pacmanNextPathIdx + leveldepth + 1; ghostNextPathIdx < pacmanNextPathIdx; ghostNextPathIdx++)
                        {
                            int ghostPrevPathIdx = ghostNextPathIdx - leveldepth;
                            if (path[pacmanNextPathIdx].Equals(path[ghostPrevPathIdx]) && path[pacmanPrevPathIdx].Equals(path[ghostNextPathIdx]))
                            {
                                defeat = true;
                                break;
                            }
                        }
                        if (defeat) break;
                        if (lv == levels)
                        {
                            int ghostLastNextPathIdx = leveldepth * (lv + 1);
                            int ghostLastPrevPathIdx = ghostLastNextPathIdx - leveldepth;
                            if (path[pacmanNextPathIdx].Equals(path[ghostLastPrevPathIdx]) && path[pacmanPrevPathIdx].Equals(path[ghostLastNextPathIdx]))
                            {
                                defeat = true;
                                break;
                            }
                        }
                    }
                }
                break;
            }
        }

        double defeatpenalty = -400;
        double h3 = (defeat) ? defeatpenalty : 0;


        /**
         h4 -- victory bonus: */
        double victorybonus = 400;
        double h4 = (numOfEatenCandies >= numOfcandies) ? victorybonus : 0;

        /**
         he -- complete heuristic evaluation: */
        double he = h1 + h2 + h3 + h4;
        return he;
    }

    private GameObject getNextPlayerObject(bool goForward = true, bool isFirst = false)
    {
        if (goForward)
        {
            iterator1 = (isFirst) ? 0 : iterator1 + 1;
            iterator1 = (iterator1 >= leveldepth) ? 0 : iterator1;
            return playerObjects[iterator1];
        }
        else
        {
            iterator1--;
            iterator1 = (iterator1 < 0) ? leveldepth - 1 : iterator1;
            return playerObjects[iterator1];
        }
    }

    private int calculateNodeState(GameObject player)
    {
        if (player.CompareTag(pacmanTag)) return Node.STATE_MAXIMISER;
        else if (player.CompareTag(ghostTag)) return Node.STATE_MINIMISER;
        else return -1;
    }
}