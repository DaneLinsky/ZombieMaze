using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maze : MonoBehaviour {
	public MazeCell cellPrefab;
	private MazeCell[,] cells;

	public Text textRef;

	private List<Zombie> zombies;

	public MazePassage passagePrefab;
	public MazeWall wallPrefab;

	public IntVector2 size;

	public MazeCellSettings[] cellSettings;

	[Range(0f,1f)]
	public float numberOfZombiesProbability;
	public Zombie zombiePrefab;

	public int levelNumber;

	public AmmoPack ammoPrefab;
	[Range(0f, 1f)]
	public float ammoPackProbability;

	public int zombieCounter;

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}

	public void Generate () {
		cells = new MazeCell[size.x, size.z];
		zombies = new List<Zombie> ();
		zombieCounter = 0;
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		while (activeCells.Count > 0) {
			DoNextGenerationStep(activeCells);
		}
	}

	private void SetCellProperties (IntVector2 coordinates, MazeCell newCell){
		cells [coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.settingsIndex = Random.Range (0, cellSettings.Length);
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3 (coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		newCell.Initialize (cellSettings [newCell.settingsIndex]);
	}

	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		SetCellProperties (coordinates, newCell);
		if (Random.value < ammoPackProbability) {
			CreateAmmoOnCell (newCell);
		}
		if (Random.value < numberOfZombiesProbability) {
			CreateZombieOnCell (newCell);
		}
		return newCell;
	}

	private void CreateZombieOnCell(MazeCell cell){
		Zombie zombie = Instantiate (zombiePrefab) as Zombie;
		zombie.InitialSetUp (cell);
		zombie.transform.parent = this.transform;
		//zombie.pathFindingValues.cells = cells;
		//zombie.pathFindingValues.startLocation = zombie.transform;
		zombies.Add (zombie);
		cell.zombieInstance = zombie;
		zombieCounter++;
	}
	private void CreateAmmoOnCell(MazeCell cell){
		AmmoPack ammo = Instantiate (ammoPrefab) as AmmoPack;
		cell.ammoOnCell = true;
		ammo.transform.localPosition = cell.transform.localPosition;
		ammo.transform.parent = cell.transform;
		ammo.name = "Ammo " + cell.transform.localPosition.x + ", " + cell.transform.localPosition.z;
		cell.ammo = ammo;
		ammo.StartMovingPack ();
	}

	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}

	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		MazeCell newCell = CreateCell(RandomCoordinates);
		activeCells.Add(newCell);
	}


	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];
		if (currentCell.IsFullyInitialized) {
			currentCell.FindAllPassages ();
			activeCells.RemoveAt(currentIndex);
			return;
		}
		MazeDirection direction = currentCell.RandomUninitializedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
		if (ContainsCoordinates(coordinates)) {
			MazeCell neighbor = GetCell(coordinates);
			if (neighbor == null) {
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			}
			else {
				CreateWall(currentCell, neighbor, direction);
			}
		}
		else {
			CreateWall(currentCell, null, direction);
		}
	}

	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazeWall wall = Instantiate(wallPrefab) as MazeWall;
		wall.Initialize(cell, otherCell, direction);
		wall.transform.GetChild(0).GetComponent<Renderer>().material = wall.color;
		if (otherCell != null) {
			wall = Instantiate(wallPrefab) as MazeWall;
			wall.transform.GetChild(0).GetComponent<Renderer>().material = wall.color;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}

	public void StartMovingZombies(){
		foreach(Zombie z in zombies){
			z.StartMoving ();
		}

	}

	public IEnumerator CheckNumberOfZombies(){
		int childCount = transform.childCount;
		WaitForSeconds delay = new WaitForSeconds (.5f);
		while (true) {
			if (transform.childCount != childCount) {
				childCount = transform.childCount;
				zombieCounter--;
			}
			if (childCount == 0) {
				StopAllCoroutines ();
				DestroyImmediate (this.gameObject);
			}
			yield return delay;
		}
	}

	//public IEnumerator UpdateZombiesWithPlayerLocation(Transform location){
		//WaitForSeconds delay = new WaitForSeconds (zombies [0].delayBetweenMovements);
		//foreach (Zombie z in zombies) {
			//z.pathFindingValues.endLocation = location;
		//}
		//yield return delay;
	//}

}
