using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_Node
{
	public bool walkable;
	public Vector3 worldPosition;
	
	//�����Ϳ� ��ǥ
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	//�������� == ������ (linked list) 
	public JKH_Node parent; 

	public JKH_Node(bool _walkable, Vector3 _worldPos,int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}
