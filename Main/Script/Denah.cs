using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Denah : MonoBehaviour {
	//user guide
	private GameObject guide;


	//zoom and pan
	//specific touch
	private bool isTouching;

	//canvas scaler to fix diffferent speed in different screen issue
	private CanvasScaler canvasScaler;
	private Vector2 ScreenScale;


	//speed
	public float zoomSpeed = 5f;
	public float panSpeed = 250f;

	public float nudgeRange=0.5f;

	private RectTransform imageTransform;
	private float scaling = 1.0f;
	private Vector2 currentScale;
	private Vector2 currentPosition;

	private float defaultlimitX;
	private float defaultlimitY;
	private float limitX;  //pan limit,max image tranformation
	private float limitY;

	private float deltaPosX = 0.0f;
	private float deltaPosY = 0.0f;

	private Touch touch;
	private Touch touchZero;
	private Touch touchOne;
	private Vector2 firstPos;

	private Vector2 secPos;



	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Landscape;

		//user guide
		guide = GameObject.Find ("Guide");

		//zoom and pan

		isTouching = false;
		imageTransform = GameObject.Find("DenahModel").GetComponent<RectTransform> ();

		//initialize for object creation only
		currentScale = new Vector2 (imageTransform.localScale.x, imageTransform.localScale.y);
		currentPosition = new Vector2 (0,0);

		//default position limit
		//RectTransform ImagePanel = imageTransform.parent.GetComponent<RectTransform> ();
		defaultlimitX = imageTransform.offsetMax.x;
		defaultlimitY = imageTransform.offsetMax.y;

		canvasScaler = this.GetComponent<CanvasScaler> ();
		ScreenScale = new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);

	}
	
	// Update is called once per frame
	void Update () {
		if (isTouching) {
			if (Input.touchCount == 2)      //For Detecting Multiple Touch On Screen
			{
				// Store both touches.
				touchZero = Input.GetTouch(0);
				touchOne = Input.GetTouch(1);


				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				// Change image scale
				scaling = imageTransform.localScale.x - (deltaMagnitudeDiff * ScreenScale.x * ScreenScale.y * zoomSpeed * Time.deltaTime);

				// code for maximum and minimum zoom limit
				scaling = Mathf.Max(scaling, 1.0f);
				scaling = Mathf.Min(scaling, 6f);

				//implement the scale value to image
				currentScale.x = scaling;
				currentScale.y = scaling;
				imageTransform.localScale = currentScale; 

				//set x and y limit depend on scale
				limitX = defaultlimitX * (scaling-1);
				limitY = defaultlimitY * (scaling-1);

				//get image position
				currentPosition.x = imageTransform.localPosition.x;
				currentPosition.y = imageTransform.localPosition.y;

				//limit image position
				currentPosition.x = Mathf.Min (currentPosition.x, limitX);
				currentPosition.x = Mathf.Max (currentPosition.x, -limitX);

				currentPosition.y = Mathf.Min (currentPosition.y, limitY);
				currentPosition.y = Mathf.Max (currentPosition.y, -limitY);

				//implement limit position
				imageTransform.localPosition = currentPosition;
			}


			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)       //Single Touch Began
			{

				//store the first touch position
				touch = Input.GetTouch(0);
				firstPos.x = touch.position.x;
				firstPos.y = touch.position.y;
			}

			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) //single touch move or hold     //Single Touch Moved
			{

				if (scaling > 1.01f)
				{
					//store the next touch position
					touch = Input.GetTouch(0);
					secPos.x = touch.position.x;
					secPos.y = touch.position.y;

					//calculate range from first touch, normalize by screen resolution
					deltaPosX = (secPos.x - firstPos.x) * ScreenScale.x;
					deltaPosY = (secPos.y - firstPos.y) * ScreenScale.y;

					if (Mathf.Abs(deltaPosX)>nudgeRange && Mathf.Abs(deltaPosY)>nudgeRange) { //Avoid moving while finger only nudge a little
						//calculate panning value
						deltaPosX = deltaPosX*Time.deltaTime*panSpeed;
						deltaPosY = deltaPosY*Time.deltaTime*panSpeed;

						//add panning value to current position
						currentPosition.x = imageTransform.localPosition.x + deltaPosX;
						currentPosition.y = imageTransform.localPosition.y + deltaPosY;

						//limit panning
						currentPosition.x = Mathf.Min (currentPosition.x, limitX);
						currentPosition.x = Mathf.Max (currentPosition.x, -limitX);

						currentPosition.y = Mathf.Min (currentPosition.y, limitY);
						currentPosition.y = Mathf.Max (currentPosition.y, -limitY);

						//implement panning position
						imageTransform.localPosition = currentPosition;

						//refresh first position
						firstPos.x = secPos.x;
						firstPos.y = secPos.y;
					}

				}
			}
		}
	}

	public void DenahTouched(){
		isTouching = true;
	}

	public void DenahReleased(){
		isTouching = false;
	}

	public void CloseInfo(){
		hideInfo (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject);
	}

	public void clickPos(GameObject posObject){
		showInfo (posObject);
	}

	public void CloseGuide(){
		guide.SetActive (false);
	}

	private void showInfo(GameObject posObject){
		
		posObject.GetComponent<RectTransform> ().localPosition = Vector3.zero;

		RectTransform content = posObject.transform.Find ("Scroll View/Viewport/Content").GetComponent<RectTransform> ();
		Vector3 contentPosition = content.anchoredPosition;
		contentPosition.y = 0;
		content.anchoredPosition = contentPosition;
	}

	private void hideInfo(GameObject posObject){
		Vector3 posPosition = posObject.GetComponent<RectTransform> ().localPosition;
		posPosition.x = 1000; //throw it outside canvas, canvas x point is -800 to 800
		posObject.GetComponent<RectTransform> ().localPosition = posPosition;
	}
}
