using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour {
	public float floatingPackDelay;
	private bool directionStatus = false;
	private const float deltaHeight = .02f;
	public IEnumerator FloatingAmmoPacks(){
		WaitForSeconds delay = new WaitForSeconds(floatingPackDelay);
		while (true) {
			SetDirection ();
			if (directionStatus) {
				yield return delay;
				MoveAmmoPrefabUp ();
			} else if(!directionStatus){
				yield return delay;
				MoveAmmoPrefabDown ();
			}
		}
	}

	private void MoveAmmoPrefabUp(){
		float yValue = transform.localPosition.y + deltaHeight;
		transform.localPosition =
			new Vector3 (transform.localPosition.x, yValue, transform.localPosition.z);
	}

	private void MoveAmmoPrefabDown(){
		float yValue = transform.localPosition.y - deltaHeight;
		transform.localPosition = 
			new Vector3 (transform.localPosition.x, yValue, transform.localPosition.z);
	}

	private void SetDirection ()
	{
		if (transform.localPosition.y >= .3) {
			//MoveDown
			directionStatus = false;
		}
		if (transform.localPosition.y <= .1) {
			//MoveUp
			directionStatus = true;
		}
	}

	public void DestroyPack(){
		StopCoroutine (FloatingAmmoPacks ());
		DestroyImmediate (this.gameObject);
	}

	public void StartMovingPack(){
		StartCoroutine (FloatingAmmoPacks ());
	}
}
