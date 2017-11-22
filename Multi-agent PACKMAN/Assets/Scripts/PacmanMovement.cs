using UnityEngine;

public class PacmanMovement : Movement
{
    public bool doStart = false;
    public bool onStandby;

    private ExpectiMinMax tree;
    public float animationSpeed;
    private Vector2 originPosition;
    private Vector2 targetPosition;
    private GameObject[] playerList;

    private ExpectiMinMax oldtree = null;
    private BayesController classificator;
    public float[] allProbabilities;
    public bool[] predictions;

    // Use this for initialization
    void Start()
    {
        // Set-up animation speed value
        if (animationSpeed < 0)
            animationSpeed = 0;
        else if (animationSpeed > 1)
            animationSpeed = 1;
        getPlayerList();
        classificator = new BayesController(playerList);
        onStandby = !isAnimating();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        Vector2 playerPosition = transform.position;
        Vector2 nextPosition;

        if ((onStandby && doStart) || (!onStandby && !doStart))
        {
            doStart = false;
            if (!isAnimating())
            {
                if (oldtree != null)
                {
                    predictions = oldtree.predictMoves();
                    classificator.train(predictions);
                }       
                allProbabilities = classificator.getAllProbabilities();
                
                originPosition = playerPosition;
                tree = gameObject.GetComponent<ExpectiMinMax>();               
                tree.initiate(playerList);
                oldtree = tree;
                targetPosition = tree.getOptimalTarget();
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

    private void getPlayerList()
    {
        GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        int n = 1 + ghosts.Length;
        GameObject[] players = new GameObject[n];
        players[0] = gameObject;
        int i = 1;
        for (int j = ghosts.Length - 1; j >= 0; j--)
            if (!gameObject.Equals(ghosts[j]))
                players[i++] = ghosts[j];
        //foreach (GameObject g in ghosts)
        //    if (!gameObject.Equals(g))
        //        players[i++] = g;
        if (!gameObject.Equals(pacman))
            players[i++] = pacman;
        playerList = players;
    }

    public float getProbabilityThatPlayerIsMinMax(int playerID) 
    {
        return classificator.getProbability(playerID);
    }

    public void defeatPacman()
    {
        GameController.haveLost = true;
    }
}

