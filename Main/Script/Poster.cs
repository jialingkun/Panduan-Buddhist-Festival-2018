using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Poster : MonoBehaviour {

	// Use this for initialization
	/*void Start () {
		//load all, should be execute at first scene
		//SaveLoad.loadAll();
	}*/
	
	public void nextButton(){
		SceneManager.LoadScene (1);
	}
}
