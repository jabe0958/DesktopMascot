using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {

	private Animator animator;

	private bool isMouseOver;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		isMouseOver = false;
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log ("[Start] Update()");
		if (Input.GetKey (KeyCode.Mouse0) && isMouseOver) {
//			Debug.Log ("[Start] Get Key && isMouseOver");
			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				Debug.Log ("[Start] Get Key Down");
				animator.SetBool ("is_jumping", true);
				Debug.Log ("[End  ] Get Key Down");
			}
//			Debug.Log ("[End  ] Get Key && isMouseOver");
		} else if (Input.GetKey (KeyCode.UpArrow)) {
			animator.SetBool ("is_jumping", true);
		} else {
			animator.SetBool ("is_jumping", false);
		}
		isMouseOver = false;
//		Debug.Log ("[End  ] Update()");
	}

	void OnMouseOver() {
//		Debug.Log ("[Start] OnMouseOver().");
		isMouseOver = true;
//		Debug.Log ("[End  ] OnMouseOver().");
	}

}
