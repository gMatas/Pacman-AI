using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BayesController 
{
	private GameObject[] players;
	private Hashtable probabilityTable = new Hashtable();
	private Hashtable hitCountTable = new Hashtable();
	private int N = 0;
	private float startingProbability = 0.3f;		// 0 - is random, 1 - is MinMax 


	public BayesController(GameObject[] players)
	{
		this.players = players;
		for (int playerID = 0; playerID < players.Length; playerID ++)
		{
			probabilityTable.Add(playerID, startingProbability);
			hitCountTable.Add(playerID, 0);
		}
	}

	public float[] getAllProbabilities() 
	{
		float[] results = new float[players.Length];
		for (int playerID = 0; playerID < players.Length; playerID ++) 
		{
			results[playerID] = (float) probabilityTable[playerID];
		}
		return results;
	}

	public float getProbability(int playerID)
	{
		return (float) probabilityTable[playerID];
	}

	public void train(bool[] decisions) 
	{
		N ++;
		for (int playerID = 0; playerID < players.Length; playerID ++)
		{
			if (decisions[playerID]) 
			{
				int hitCount = (int) hitCountTable[playerID] + 1;
				hitCountTable[playerID] = hitCount;
				float p = calculate((float) probabilityTable[playerID], hitCount, true);
				probabilityTable[playerID] = p;
			}
			else 
			{
				float p = calculate((float) probabilityTable[playerID], (int) hitCountTable[playerID], false);
				probabilityTable[playerID] = p;
			}
		}
	}

	private float calculate(float prevPobability, int hitCount, bool isPositive) // calculate new probability
	{
		float hitRatio = (float) hitCount / N;
		
		float peh1 = 1f;
		float peh2 = 0.5f;

        float ph1 = prevPobability * hitRatio;
		ph1 = (ph1 >= 0.99f) ? 0.99f : ph1;
		ph1 = (ph1 <= hitRatio/5) ? hitRatio/5 : ph1;
		float ph2 = 1f - ph1;
		float pe = (peh1 * ph1) + (peh2 * ph2);

		float ph1e = ph1 / pe;
	
		return ph1e;
	}
}
