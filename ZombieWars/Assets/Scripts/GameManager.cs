using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public Maze mazePrefab;
	private Maze mazeInstance;
	public Player playerPrefab;
	private Player playerInstance;
	private Text zombieCounter;
	// Use this for initialization
	private void Start () {
		BeginGame();

	}

	private void Update(){
		if (zombieCounter != null) {
			zombieCounter.text = "Number of Zombies: " + mazeInstance.zombieCounter;
		}
	}

	private void BeginGame () {
		InstantiateMaze ();
		InstantiatePlayer ();
		CreateLabel ();
		CreateMiniMap ();
	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		if (playerInstance != null) {
			Destroy(playerInstance.gameObject);
		}
		BeginGame();
	}

	private void InstantiatePlayer ()
	{
		playerInstance = Instantiate (playerPrefab) as Player;
		playerInstance.FillGun ();
		playerInstance.SetLocation (mazeInstance.GetCell (mazeInstance.RandomCoordinates));
	}

	private void CreateMiniMap ()
	{
		Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect (0f, 0f, 1f, 1f);
		Camera.main.clearFlags = CameraClearFlags.Depth;
		Camera.main.rect = new Rect (-.15f, 0f, 0.5f, 0.5f);
	}

	private void CreateLabel ()
	{
		zombieCounter = GameObject.Find ("Canvas").GetComponentInChildren<Text> ();
	}

	void InstantiateMaze ()
	{
		mazeInstance = Instantiate (mazePrefab) as Maze;
		mazeInstance.Generate ();
		StartCoroutine (mazeInstance.CheckNumberOfZombies ());
		mazeInstance.StartMovingZombies ();
	}
}
