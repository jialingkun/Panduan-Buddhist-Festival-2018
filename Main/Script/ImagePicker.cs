using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//NOTE: we're using LukeWaffel.AndroidGallery, without this it won't work
using LukeWaffel.AndroidGallery;

public class ImagePicker : MonoBehaviour {

	private Texture2D loadedTexture;
	private string loadedName;
	//compress
	public float compressWidth = 1280f;
	//loading
	public Texture2D loadingImage;
	private bool loadingComplete = false;
	//form daftar
	private GameObject previewDaftar;
	private InputField nama;
	private InputField telp;
	private GameObject peringatan;
	private GameObject formDaftar;
	//tanpa daftar
	private GameObject previewNonDaftar;
	private GameObject uploadPanel;
	//upload status
	private Text status;
	private GameObject closeStatus;
	private GameObject uploadStatus;
	//animate wait status
	private Text waitAnimate;
	private bool isanimate;

	private GameObject confirmLomba;
	// Use this for initialization
	void Start () {
		//lomba
		previewDaftar = GameObject.Find("PreviewDaftar");
		nama = GameObject.Find("Nama").GetComponent<InputField>();
		telp = GameObject.Find("Telp").GetComponent<InputField>();
		peringatan = GameObject.Find("Peringatan");
		formDaftar = GameObject.Find("FormDaftar");
		previewNonDaftar = GameObject.Find("PreviewNonDaftar");
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
	}

	//This function is called by the Button
	public void OpenGalleryButton(){
		loadingComplete = false;
		//display loading texture
		previewDaftar.GetComponent<RawImage> ().texture = loadingImage;
		previewNonDaftar.GetComponent<RawImage> ().texture = loadingImage;

		//Opens the Android image picker, the parameter is a callback function the AndroidGallery script will call when the image has finished loading
		AndroidGallery.Instance.OpenGallery (ImageLoaded);
		confirmLomba.SetActive (true);
		if (PlayerPrefs.GetString ("Telepon", "") == "") {
			formDaftar.SetActive (true);
			uploadPanel.SetActive (false);
		} else {
			formDaftar.SetActive (false);
			uploadPanel.SetActive (true);
		}

	}

	//This is the callback function we created
	public void ImageLoaded(){

		//You can put anything in the callback function. You can either just grab the image, or tell your other scripts the custom image is available

		Debug.Log("The image has succesfully loaded!");
		loadedTexture = AndroidGallery.Instance.GetTexture() as Texture2D;
		loadedName = AndroidGallery.Instance.GetFileName();

		float ratio = (float)loadedTexture.width / (float)loadedTexture.height;

		//daftar
		previewDaftar.GetComponent<AspectRatioFitter> ().aspectRatio = ratio;
		previewDaftar.GetComponent<RawImage> ().texture = loadedTexture;

		//non daftar
		previewNonDaftar.GetComponent<AspectRatioFitter> ().aspectRatio = ratio;
		previewNonDaftar.GetComponent<RawImage> ().texture = loadedTexture;

		//loading
		loadingComplete = true;

	}

	public void clickCancelLomba(){
		peringatan.SetActive (false);
		uploadStatus.SetActive (false);
		confirmLomba.SetActive (false);
	}

	public void clickUploadDaftar(){
		if (loadingComplete) {
			if (nama.text.Length > 1 && telp.text.Length > 5) {
				PlayerPrefs.SetString("Telepon",telp.text);
				PlayerPrefs.SetString("Nama",nama.text);
				formDaftar.SetActive (false);
				clickUpload ();
			} else {
				peringatan.SetActive (true);
			}
		}

	}

	public void clickUpload(){
		if (loadingComplete) {
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
				Firebase.Storage.StorageReference images_ref = storage_ref.Child("Lomba Foto/"+PlayerPrefs.GetString ("Nama", "")+"_"+PlayerPrefs.GetString ("Telepon", "")+"/"+loadedName);

				//compress texture
				Texture2D compressedTexture = loadedTexture;
				if ((float)loadedTexture.width>compressWidth) {
					float ratio = (float)loadedTexture.width / compressWidth;
					int newWidth = (int)((float)loadedTexture.width/ratio);
					int newHeight = (int)((float)loadedTexture.height/ratio);
					TextureScale.Bilinear(compressedTexture,newWidth,newHeight);
				}




				// Data in memory
				byte[] custom_bytes = compressedTexture.EncodeToJPG();

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
