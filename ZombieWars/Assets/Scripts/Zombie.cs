using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {
	private MazeCell currentCell;

	private const float deltaMove = .02f;
	private const float delayAnimatedMovement = .01f;

	public int delayBetweenMovements;
	public int zombieIndex{ get; set; }
	//public SearchParameters pathFindingValues;

	public void InitialSetUp(MazeCell cell){
		transform.localPosition = cell.transform.localPosition;
		currentCell = cell;
		currentCell.zombieOnCell = true;
		currentCell.zombieInstance = this;
	}

	public void SetLocation (MazeCell cell) {
		currentCell.zombieOnCell = false;
		currentCell.zombieInstance = null;
		currentCell = cell;
		currentCell.zombieOnCell = true;
		currentCell.zombieInstance = this;
	}

	public IEnumerator Move(){
		WaitForSeconds delay = new WaitForSeconds (delayBetweenMovements);
		while (true) {
			MazeCellEdge edge = currentCell.RandomPassage ();
			yield return delay;
			if (!edge.otherCell.zombieOnCell) {
				StartCoroutine(AnimateMovements (edge.direction));
				yield return 1;
				SetLocation (edge.otherCell);
				Rotate (edge.direction);
			}
		}
	}

	public void Rotate(MazeDirection direction){
		transform.localRotation = direction.GetNextClockwise().ToRotation();
	}

	//private void SearchForPlayer(){
		//List<MazeCellEdge> passages = currentCell.GetAllPassages ();

	//}

	public void StartMoving(){
		StartCoroutine (Move ());
	}

	private IEnumerator AnimateMovements(MazeDirection direction){
		WaitForSeconds delay = new WaitForSeconds (delayAnimatedMovement);
		Vector3 direcitonVector = IntVector2.ToVector3(direction.ToIntVector2 ());
		Vector3 position = transform.localPosition;
		float currentAngle = 0;
		float increment = 180f/50f;
		for (float i = 0; i <= 1f; i += deltaMove) {
			Vector3 temporaryLocation = position;
			temporaryLocation.x += direcitonVector.x * i;
			temporaryLocation.y += (Mathf.Sin((Mathf.PI / 180) * currentAngle))/2;
			temporaryLocation.z += direcitonVector.z * i;
			transform.localPosition = temporaryLocation;
			currentAngle += increment;
			yield return delay;
		}
		transform.localPosition = new Vector3 (position.x + direcitonVector.x, position.y, position.z + direcitonVector.z);
		StopCoroutine (AnimateMovements (direction));
	}

	public void KillZombie(){
		StopAllCoroutines ();
		DestroyImmediate (this.gameObject);
	}
}

