using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchParameters : MonoBehaviour {
	public Transform startLocation;
	public Transform endLocation;
	public MazeCell[,] cells{ get; set; }
}
