using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_Node
{
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public JKH_Node parent;

	public JKH_Node(bool _walkable, Vector3 _worldPos)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		//gridX = _gridX;
		//gridY = _gridY;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}
