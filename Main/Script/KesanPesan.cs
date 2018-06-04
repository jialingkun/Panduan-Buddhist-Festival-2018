using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class KesanPesan : MonoBehaviour {
	private bool isready;
	private DatabaseReference reference;

	private InputField nama;
	private InputField teks;
	private GameObject peringatan;
	//upload status
	private Text status;
	//private GameObject closeStatus;
	private GameObject uploadStatus;

	// Use this for initialization
	void Start () {
		nama = GameObject.Find("Nama").GetComponent<InputField>();
		teks = GameObject.Find("KesanPesan").GetComponent<InputField>();

		peringatan = GameObject.Find("Peringatan");
		peringatan.SetActive (false);

		status = GameObject.Find("Status").GetComponent<Text>();
		//closeStatus = GameObject.Find("CloseStatus");
		uploadStatus = GameObject.Find("UploadStatus");
		//closeStatus.SetActive (false);
		uploadStatus.SetActive (false);

		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available) {
				// Set a flag here indiciating that Firebase is ready to use by your
				// application.
				isready = true;
				FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://buddhist-festival-ar-2018.firebaseio.com/");
				reference = FirebaseDatabase.DefaultInstance.RootReference;
			} else {
				UnityEngine.Debug.LogError(System.String.Format(
					"Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
				isready = false;
			}
		});
	}
	
	public void clickSubmit(){
		if (isready) {
			if (nama.text.Length > 1 && teks.text.Length > 1) {
				//submitting
				uploadStatus.SetActive (true);
				//closeStatus.SetActive (false);

				reference.Child("Kesan Pesan").Push().Child(nama.text).SetValueAsync(teks.text)
					.ContinueWith ((task) => {
						if (!task.IsCompleted) {
							status.text = "Tidak dapat mengirim pesan. Terjadi kesalahan.";
							//closeStatus.SetActive (true);
							// Uh-oh, an error occurred!
						} else {
							// Metadata contains file metadata such as size, content-type, and download URL.
							//Firebase.Storage.StorageMetadata metadata = task.Result;
							status.text = "Pesan Berhasil Terkirim. Terima kasih.";
							//closeStatus.SetActive (true);
						}
					});
			} else {
				peringatan.SetActive (true);
			}
		} else {
			peringatan.SetActive (true);
			peringatan.GetComponent<Text>().text = "Tidak dapat mengirim pesan. Terjadi kesalahan.";
		}
	}


	public void clickCloseSubmit(){
		peringatan.SetActive (false);
		uploadStatus.SetActive (false);

	}
}
