using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Text scoreField;
    public Text statusField;

    public PacmanMovement pacman;
    public GhostMovementA ghostA;
    public GhostMovementB ghostB;
       
    public bool Run = false;
    public static bool haveWon = false;
    public static bool haveLost = false;
    public static bool isOver = false;
    public static int Score = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        bool ready0 = true;
        bool ready1 = true;
        bool ready2 = true;
        bool ready3 = true;

        if (pacman != null) ready0 = pacman.onStandby;
        if (ghostA != null) ready2 = ghostA.onStandby;
        if (ghostB != null) ready3 = ghostB.onStandby;

        bool isAllOnStandby = ready0 && ready1 && ready2 && ready3 && Run;
        if (isAllOnStandby && !isOver)
        {
            // TODO: check if pacman is cought
            bool ghostA_success = (ghostA != null) && (pacman.transform.position.Equals(ghostA.transform.position));
            bool ghostB_success = (ghostB != null) && (pacman.transform.position.Equals(ghostB.transform.position));
            if (ghostA_success || ghostB_success)
                pacman.defeatPacman();
            else
            {
                Score--;
                if (pacman != null) pacman.doStart = true;
                if (ghostA != null) ghostA.doStart = true;
                if (ghostB != null) ghostB.doStart = true;
            }

            
        }
    }

    void Update()
    {
        int candiesLeft = FindObjectsOfType<CandyBonus>().Length;
        if (candiesLeft == 0 && !isOver)
        {
            Score += 400;
            haveWon = true;
            isOver = true;
        }
        if (haveLost && !isOver)
        {
            Score += -400;
            isOver = true;
        }
        string str = string.Format("Score: {0}", Score);
        scoreField.text = str;

        StatusField();
    }

    private void StatusField()
    {
        string str = "";
        if (haveWon) str = "VICTORY!";
        else if (haveLost) str = "DEFEAT";
        statusField.text = str;
    }

    public void ResetGame()
    {
        Score = 0;
        isOver = haveWon = haveLost = false;
        SceneManager.LoadScene("Level 1");
    }
}
