using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour {
	public GameObject galleryImagePrefab;

	private RectTransform content;
	//x coordinate spawn
	private float spawnPointLeft;

	public int padding;
	public int gap;

	//delete
	private GameObject confirmDelete;
	private int activeIndex = 0;

	//form daftar
	private InputField nama;
	private InputField telp;
	private GameObject peringatan;
	private GameObject formDaftar;
	//tanpa daftar
	private GameObject uploadPanel;


	//upload status
	private Text status;
	private GameObject closeStatus;
	private GameObject uploadStatus;
	//animate wait status
	private Text waitAnimate;
	private bool isanimate;
	//lomba
	private GameObject confirmLomba;

	//preview image
	private GameObject preview;
	private GameObject imagePreview;






	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;

		//delete
		confirmDelete = GameObject.Find ("ConfirmDelete");
		confirmDelete.SetActive (false);

		//lomba
		nama = GameObject.Find("Nama").GetComponent<InputField>();
		telp = GameObject.Find("Telp").GetComponent<InputField>();
		peringatan = GameObject.Find("Peringatan");
		formDaftar = GameObject.Find("FormDaftar");
		uploadPanel = GameObject.Find("UploadPanel");
		status = GameObject.Find("Status").GetComponent<Text>();
		waitAnimate = GameObject.Find("WaitAnimate").GetComponent<Text>();
		closeStatus = GameObject.Find("CloseStatus");
		uploadStatus = GameObject.Find("UploadStatus");
		confirmLomba = GameObject.Find("ConfirmLomba");
		peringatan.SetActive (false);
		formDaftar.SetActive (false);
		uploadPanel.SetActive (false);
		closeStatus.SetActive (false);
		uploadStatus.SetActive (false);
		confirmLomba.SetActive (false);


		//preview image
		preview = GameObject.Find ("Preview");
		imagePreview = GameObject.Find ("ImagePreview");
		preview.SetActive (false);



		//scroll content
		content = GameObject.Find ("Content").GetComponent<RectTransform> ();
		//x Coordinate spawn
		spawnPointLeft = GameObject.Find ("SpawnPointLeft").GetComponent<RectTransform> ().anchoredPosition.x;

		refreshGallery ();
	}

	public void refreshGallery(){
		//remove all gallery image
		GameObject[] collectionClone;
		collectionClone = GameObject.FindGameObjectsWithTag ("GalleryImage");
		foreach (GameObject buttonObject in collectionClone) {
			Destroy (buttonObject);
		}


		RectTransform galleryImageTransform;
		float galleryImageOffset = galleryImagePrefab.GetComponent<RectTransform> ().sizeDelta.y;
		Vector2 galleryImagePosition = new Vector2 (spawnPointLeft, -(galleryImageOffset/2+galleryImageOffset/padding));
		float galleryNextPositionY = 0f;
		int galleryCount = SaveLoad.imageTexture.Count;
		float ratio = 1f;

		GameObject galleryImageObject;
		bool isleft = true;

		for (int i = 0; i < galleryCount; i++) {
			galleryImageObject = Instantiate (galleryImagePrefab);

			//fix bug call by reference on clickedit() parameter
			int index = i;
			galleryImageObject.GetComponent<Button> ().onClick.AddListener (delegate() {
				clickGalleryImage(index);
			});

			galleryImageObject.transform.Find("RawImage").GetComponent<RawImage> ().texture = SaveLoad.imageTexture[i];

			//change aspect ratio texture
			ratio = (float)SaveLoad.imageTexture[i].width / (float)SaveLoad.imageTexture[i].height;
			galleryImageObject.transform.Find("RawImage").GetComponent<AspectRatioFitter> ().aspectRatio = ratio;

			galleryImageTransform = galleryImageObject.GetComponent<RectTransform> ();
			galleryImageTransform.SetParent (content,false);
			galleryImageTransform.anchoredPosition = galleryImagePosition;

			galleryImagePosition.x = -galleryImagePosition.x;
			if (isleft) {
				galleryNextPositionY = galleryImagePosition.y - galleryImageOffset - galleryImageOffset / gap;
				isleft = false;
			} else {
				galleryImagePosition.y = galleryNextPositionY;
				isleft = true;
			}

		}

		//scroll space width = (last position + gap) - gap + padding
		//The last position after for loop only store (last position + gap), so convert it back to last position by - gap
		content.sizeDelta = new Vector2 (content.sizeDelta.x, - galleryNextPositionY - (galleryImageOffset + galleryImageOffset / gap) + (galleryImageOffset/2 + galleryImageOffset/padding));

	}


	public void clickGalleryImage(int index){
		imagePreview.GetComponent<RawImage> ().texture = SaveLoad.imageTexture[index];
		float ratio = (float)SaveLoad.imageTexture[index].width / (float)SaveLoad.imageTexture[index].height;
		imagePreview.GetComponent<AspectRatioFitter> ().aspectRatio = ratio;
		preview.SetActive (true);

		//store index for delete
		activeIndex = index;
	}

	public void clickBackToGallery(){
		preview.SetActive (false);
	}

	public void clickDelete(){
		confirmDelete.SetActive (true);
	}

	public void clickCancelDelete(){
		confirmDelete.SetActive (false);
	}

	public void clickConfirmDelete(){
		confirmDelete.SetActive (false);
		preview.SetActive (false);

		SaveLoad.deleteImage (activeIndex);

		refreshGallery ();
	}

	public void clickLomba(){
		confirmLomba.SetActive (true);
		if (PlayerPrefs.GetString ("Telepon", "") == "") {
			formDaftar.SetActive (true);
			uploadPanel.SetActive (false);
		} else {
			formDaftar.SetActive (false);
			uploadPanel.SetActive (true);
		}
	}
	public void clickCancelLomba(){
		peringatan.SetActive (false);
		uploadStatus.SetActive (false);
		confirmLomba.SetActive (false);
	}

	public void clickUploadDaftar(){
		if (nama.text.Length > 5 && telp.text.Length > 6) {
			PlayerPrefs.SetString("Telepon",telp.text);
			PlayerPrefs.SetString("Nama",nama.text);
			formDaftar.SetActive (false);
			clickUpload ();



		} else {
			peringatan.SetActive (true);
		}
	}

	public void clickUpload(){
		uploadPanel.SetActive (false);
		uploadStatus.SetActive (true);
		closeStatus.SetActive (false);

		status.text = "Sedang mempersiapkan file.";
		StartCoroutine (AnimateText());
		try {
			// Get a reference to the storage service, using the default Firebase App
			Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;

			// Create a storage reference from our storage service
			Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://buddhist-festival-ar-2018.appspot.com");

			// Create a child reference
			// images_ref now points to "images"
			Firebase.Storage.StorageReference images_ref = storage_ref.Child("Lomba Foto/"+PlayerPrefs.GetString ("Nama", "")+"_"+PlayerPrefs.GetString ("Telepon", "")+"/"+SaveLoad.imageName[activeIndex]);

			// Data in memory
			byte[] custom_bytes = SaveLoad.imageTexture[activeIndex].EncodeToPNG();

			status.text = "Sedang Meng-upload Foto";

			// Create file metadata including the content type
			//Firebase.Storage.MetadataChange new_metadata = new Firebase.Storage.MetadataChange();
			//new_metadata.ContentType = "image/png";

			// Upload the file to the path "images/rivers.jpg"
			images_ref.PutBytesAsync(custom_bytes)
				.ContinueWith ((task) => {
					if (task.IsFaulted || task.IsCanceled) {
						//status.text = "Gagal mengupload foto.\n Pesan error:\n"+ task.Exception.ToString();
						status.text = "Gagal mengupload foto.";
						isanimate = false;
						closeStatus.SetActive (true);
						// Uh-oh, an error occurred!
					} else {
						// Metadata contains file metadata such as size, content-type, and download URL.
						//Firebase.Storage.StorageMetadata metadata = task.Result;
						status.text = "Upload selesai. Terima kasih sudah berpartisipasi dalam lomba foto.";
						isanimate = false;
						closeStatus.SetActive (true);
					}
				});
			
		} catch (System.Exception ex) {
			//status.text = "Gagal mengupload foto.\n Pesan error:\n"+ ex.ToString();
			status.text = "Gagal mengupload foto.";
			isanimate = false;
			closeStatus.SetActive (true);
		}

	}


	IEnumerator AnimateText(){
		string[] textAnimate = new string[]{ "Mohon Tunggu", "Mohon Tunggu.", "Mohon Tunggu..", "Mohon Tunggu...", "Mohon Tunggu...." };
		float delay = 0.5f; //editable


		isanimate = true;
		int index = 0;
		while (isanimate) {
			waitAnimate.text = textAnimate [index];
			index++;
			if (index>=textAnimate.Length) {
				index = 0;
			}
			yield return new WaitForSeconds(delay);
		}

		waitAnimate.text = "";

		yield return null;
	}

}
