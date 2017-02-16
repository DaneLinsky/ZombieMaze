using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour {
	public IntVector2 coordinates;

	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];
	public List<MazeCellEdge> passages = new List<MazeCellEdge>();

	public MazeCellEdge GetEdge (MazeDirection direction) {
		return edges[(int)direction];
	}
	public MazeCellSettings settings;
	public int settingsIndex;

	public bool ammoOnCell{ get; set; }
	public AmmoPack ammo{ get; set; }

	public bool zombieOnCell{ get; set; }
	public Zombie zombieInstance{ get; set; }
	private int initializedEdgeCount;

	public PathFindingValues parameters;

	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == MazeDirections.Count;
		}
	}

	public void SetEdge (MazeDirection direction, MazeCellEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount += 1;
	}

	public MazeDirection RandomUninitializedDirection {
		get {
			int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
			for (int i = 0; i < MazeDirections.Count; i++) {
				if (edges[i] == null) {
					if (skips == 0) {
						return (MazeDirection)i;
					}
					skips -= 1;
				}
			}
			throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
		}
	}

	public void Initialize (MazeCellSettings setting) {
		settings = setting;
		ammoOnCell = false;
		transform.GetChild(0).GetComponent<Renderer>().material = settings.colorOfCell;
	}


	public void DestroyAmmoPack(){
		ammo.DestroyPack ();
		ammoOnCell = false;
	}

	public void FindAllPassages(){
		foreach(MazeCellEdge edge in edges){
			if (edge is MazePassage) {
				passages.Add (edge);
			}
		}
	}

	public MazeCellEdge RandomPassage(){
		return passages [Random.Range (0, passages.Count)];
	}
}
