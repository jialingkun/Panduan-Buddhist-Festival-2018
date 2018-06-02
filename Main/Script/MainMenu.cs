using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public void VisiMisi(){
		SceneManager.LoadScene (2);
	}
	public void DenahLokasi(){
		SceneManager.LoadScene (3);
	}
	public void Galeri(){
		SceneManager.LoadScene (4);
	}
	public void DaftarAcara(){
		SceneManager.LoadScene (5);
	}
	public void KesanPesan(){
		SceneManager.LoadScene (6);
	}
	public void Tentang(){
		SceneManager.LoadScene (7);
	}
	public void Website(){
		Application.OpenURL("http://festivalbuddhist.org/");
	}
	public void Twitter(){
		Application.OpenURL ("twitter://user?user_id=bcbuddhistfest");
	}
	public void Facebook(){
		Application.OpenURL("https://www.facebook.com/Buddhist-Festival-1673178842767365/");
	}
	public void Instagram(){
		Application.OpenURL ("instagram://user?username=buddhistfestival");
	}
}
