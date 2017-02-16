using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private MazeCell currentCell;
	private MazeDirection currentDirection;

	public AmmoSettings[] settings;
	private const int shootDelay = 2;
	private const float timeToTurnCamera = .002f;
	public int numberOfBullets;
	private const float deltaRotationChange = 5f;

	public float deltaMove;

	public void SetLocation (MazeCell cell) {
		currentCell = cell;
		transform.localPosition = cell.transform.localPosition;
	}

	public void FillGun(){
		numberOfBullets = 5;
	}

	private void Move (MazeDirection direction) {
		MazeCellEdge edge = currentCell.GetEdge(direction);
		if (edge is MazePassage) {
			SetLocation(edge.otherCell);
		}
	}

	private void Rotate (MazeDirection direction) {
		transform.localRotation = direction.ToRotation ();
		currentDirection = direction;
	}

	private void Update () {
			GunCheck ();
			ColorGun ();
			if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) {
				Move (currentDirection);
			} else if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
				Move (currentDirection.GetNextClockwise ());
			} else if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
				Move (currentDirection.GetOpposite ());
			} else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
				Move (currentDirection.GetNextCounterclockwise ());
			} else if (Input.GetKeyDown (KeyCode.Q)) {
				Rotate (currentDirection.GetNextCounterclockwise ());
			} else if (Input.GetKeyDown (KeyCode.E)) {
				Rotate (currentDirection.GetNextClockwise ());
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				StartCoroutine(Shoot ());
			}
	}

	private void GunCheck ()
	{
		if (currentCell.ammoOnCell && !AmmoIsFull ()) {
			FillGun ();
			currentCell.DestroyAmmoPack ();
		}
	}

	private void ColorGun ()
	{
		int childIndex = 7;
		while (childIndex >= 8 - numberOfBullets) {
			transform.GetChild (childIndex).GetComponent<Renderer> ().material = settings [0].colorOfAmmo;
			childIndex--;
		}
		while (childIndex >= 3) {
			transform.GetChild (childIndex).GetComponent<Renderer> ().material = settings [1].colorOfAmmo;
			childIndex--;
		}
	}

	private IEnumerator Shoot(){
		if (numberOfBullets > 0) {
			MazeCellEdge edge = currentCell.GetEdge (currentDirection);
			if (currentCell.zombieOnCell) {
				currentCell.zombieOnCell = false;
				currentCell.zombieInstance.KillZombie ();
			} else {
				while (edge is MazePassage) {
					MazeCell newCell = edge.otherCell;
					if (newCell.zombieOnCell) {
						newCell.zombieOnCell = false;
						newCell.zombieInstance.KillZombie ();
						break;
					} else {
						edge = edge.otherCell.GetEdge (currentDirection);
					}
				}
			}
			
			numberOfBullets--;
			yield return shootDelay;
		}
	}

	private bool AmmoIsFull(){
		return numberOfBullets == 5;
	}
}