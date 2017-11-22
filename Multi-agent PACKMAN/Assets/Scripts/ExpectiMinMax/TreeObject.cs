using UnityEngine;

public class TreeObject 
{
	public Vector2 playerPosition;
	public double value;

	public TreeObject[] nextNodes = null;
	public TreeObject bestnextnode = null;
	  
	public int[] directionslist = null;
	public Vector2[] nextDirections = null;
	public Vector2 bestdirection;

	public TreeObject(int numberOfDirections) 
	{
		directionslist = new int[numberOfDirections];
		nextDirections = new Vector2[numberOfDirections];
		nextNodes = new TreeObject[numberOfDirections];
	}
}	

