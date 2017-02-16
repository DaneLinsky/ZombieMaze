using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingValues : MonoBehaviour {
	public MazeCell cell;
	public int lengthOfPathFromStartToCurrent;
	public int straightLineDistanceToFinish;
	public int totalDistance;

	public int CalcualteTotalDistance(){
		return lengthOfPathFromStartToCurrent + straightLineDistanceToFinish;
	}
}
