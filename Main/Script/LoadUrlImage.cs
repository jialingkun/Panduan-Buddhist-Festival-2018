using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadUrlImage : MonoBehaviour {

	// The source image
	public string url;
	public Sprite noimage;

	IEnumerator Start() {
		using (WWW www = new WWW (url)) {
			yield return www; //Bug download failed when setactive(false) triggered on this object
			if (!string.IsNullOrEmpty (www.error)) {
				this.GetComponent<Image> ().sprite = noimage;
			} else {
				this.GetComponent<AspectRatioFitter>().aspectRatio = (float)www.texture.width / (float)www.texture.height;
				this.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
			}
		}

	}
}
