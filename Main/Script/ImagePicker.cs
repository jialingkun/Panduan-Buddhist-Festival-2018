using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//NOTE: we're using LukeWaffel.AndroidGallery, without this it won't work
using LukeWaffel.AndroidGallery;

public class ImagePicker : MonoBehaviour {

	private Texture2D loadedTexture;
	private GameObject previewDaftar;
	private GameObject formDaftar;
	private GameObject confirmLomba;
	// Use this for initialization
	void Start () {
		previewDaftar = GameObject.Find("PreviewDaftar");
		formDaftar = GameObject.Find("FormDaftar");
		confirmLomba = GameObject.Find("ConfirmLomba");
		formDaftar.SetActive (false);
		confirmLomba.SetActive (false);
	}

	//This function is called by the Button
	public void OpenGalleryButton(){

		//NOTE: we're using LukeWaffel.AndroidGallery (As seen at the top of this script), without this it won't work

		//This line of code opens the Android image picker, the parameter is a callback function the AndroidGallery script will call when the image has finished loading

		AndroidGallery.Instance.OpenGallery (ImageLoaded);
		confirmLomba.SetActive (true);
		formDaftar.SetActive (true);


	}

	//This is the callback function we created
	public void ImageLoaded(){

		//You can put anything in the callback function. You can either just grab the image, or tell your other scripts the custom image is available

		Debug.Log("The image has succesfully loaded!");
		loadedTexture = AndroidGallery.Instance.GetTexture() as Texture2D;


		float ratio = (float)loadedTexture.width / (float)loadedTexture.height;

		//change aspect ratio texture
		previewDaftar.GetComponent<AspectRatioFitter> ().aspectRatio = ratio;
		previewDaftar.GetComponent<RawImage> ().texture = loadedTexture;

	}
}
