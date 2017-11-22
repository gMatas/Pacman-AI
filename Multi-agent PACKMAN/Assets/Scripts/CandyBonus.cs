using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBonus : MonoBehaviour {

    public Transform pacman;
    public GameController gameController;
    int bonus = 10;

	// Use this for initialization
	void Start () {
        gameController = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (pacman.transform.position == transform.position)
        {
            GameController.Score += bonus;
            Destroy(gameObject);
        }
	}
}
