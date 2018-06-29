using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseMateri : MonoBehaviour {
	public void CloseInfo(){
		hideInfo (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject);
	}

	private void hideInfo(GameObject posObject){
		Vector3 posPosition = posObject.GetComponent<RectTransform> ().localPosition;
		posPosition.x = 1000; //throw it outside canvas, canvas x point is -800 to 800
		posObject.GetComponent<RectTransform> ().localPosition = posPosition;
	}
}
