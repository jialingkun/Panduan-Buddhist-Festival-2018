using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour {

	public void clickDenah(){
		SceneManager.LoadScene (0);
	}

	public void clickKamera(){
		SceneManager.LoadScene (1);
	}

	public void clickGaleri(){
		SceneManager.LoadScene (2);
	}

	public void clickTentang(){
		SceneManager.LoadScene (3);
	}
}
