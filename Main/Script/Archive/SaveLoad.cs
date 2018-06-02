using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveLoad{
	public static List<string> imageName;
	public static List<Texture2D> imageTexture;
	public static string path = Application.persistentDataPath + "/../../../../Pictures/ARBuddhistFestival2018";
	public static bool firstrun = true;

	//call this method first at the start of the app
	public static void loadImageName(){
		string tempImageList = PlayerPrefs.GetString("ImageName","").Trim();
		if (tempImageList == "") {
			imageName = new List<string> ();
		} else {
			imageName = new List<string> (tempImageList.Split(","[0]));
		}
	}

	//call this method second at the start of the app
	public static void loadImageTexture(){
		imageTexture = new List<Texture2D> ();
		string imagePath;
		byte[] fileData;
		Texture2D tex;
		// for remove missing file
		List<int> missingIndex = new List<int>();

		for (int i = 0; i < imageName.Count; i++) {
			imagePath = path + "/"+ imageName[i];
			if (File.Exists (imagePath)) {
				fileData = File.ReadAllBytes (imagePath);
				tex = new Texture2D (2, 2);
				tex.LoadImage (fileData); //..this will auto-resize the texture dimensions.
				imageTexture.Add (tex);
			} else {
				//register missing file index
				missingIndex.Add (i);
			}
		}
		if (missingIndex.Count>0) {
			//remove missing file
			for (int i = missingIndex.Count-1; i >= 0; i--) {
				imageName.RemoveAt (missingIndex [i]);

			}
			//refresh save file
			PlayerPrefs.SetString("ImageName",string.Join(",", imageName.ToArray()));
		}
	}

	public static void loadAll(){
		if (firstrun) {
			SaveLoad.loadImageName();
			SaveLoad.loadImageTexture();
			firstrun = false;
		}
	}

	public static void saveImageName(string filename, Texture2D texture){
		imageName.Add (filename);
		imageTexture.Add (texture);
		PlayerPrefs.SetString("ImageName",string.Join(",", imageName.ToArray()));
	}

	public static void deleteImage(int deleteIndex){
		//deleting
		string deletePath = path + "/" + imageName [deleteIndex];
		if (File.Exists (deletePath)) {
			File.Delete (deletePath);
		}

		imageName.RemoveAt (deleteIndex);
		imageTexture.RemoveAt (deleteIndex);
		PlayerPrefs.SetString("ImageName",string.Join(",", imageName.ToArray()));
	}


}
