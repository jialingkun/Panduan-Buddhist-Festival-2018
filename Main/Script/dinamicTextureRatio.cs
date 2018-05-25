using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dinamicTextureRatio : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Texture texture = this.gameObject.GetComponent<RawImage> ().texture;
		float ratio = (float)texture.width / (float)texture.height;
		this.gameObject.GetComponent<AspectRatioFitter> ().aspectRatio = ratio;
	}
}
