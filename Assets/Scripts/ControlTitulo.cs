using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class ControlTitulo : MonoBehaviour {
	int vecesJugado = 0;
	public TweenPosition panelHow;
	public TweenPosition panelBotonHow;
	public TweenPosition panelPlay;

	public UIInput nombreInput;
	public UIInput nombreInputGrande;
	string nombreUsuario;
	public GameObject[] objetosAndroid;
	public GameObject[] objetosWebplayer;

	public TweenPosition panelSalir;
	public UILabel error;
	public TweenPosition errorPanel;

	public TweenPosition panelNombre;
	public TweenPosition panelNombreChico;

	public UIToggle AR;
	
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();
		Time.timeScale = 1f;
#if UNITY_IOS
		if (PlayerPrefs.GetInt ("Historia2", 0) == 1)
			AR.gameObject.SetActive (false);
		else
			AR.value = PlayerPrefs.GetInt ("activarAR", 0) == 1;
#endif
		//if (!PlayerPrefs.HasKey ("nombre")) {
		//	verPanelNombre ();
		//} else
			panelPlay.PlayForward ();
		nombreInput.value = PlayerPrefs.HasKey ("nombre")?PlayerPrefs.GetString ("nombre", "user"):"user";
		nombreInputGrande.value = PlayerPrefs.HasKey ("nombre")?PlayerPrefs.GetString ("nombre", "user"):"user";

		#if UNITY_ANDROID
		for(int i = 0; i < objetosAndroid.Length; i++) objetosAndroid[i].SetActive(!objetosAndroid[i].activeSelf);
		#endif
		#if UNITY_WEBPLAYER
		for(int i = 0; i < objetosWebplayer.Length; i++) objetosWebplayer[i].SetActive(!objetosWebplayer[i].activeSelf);
		#endif
		//StartCoroutine (setNombre ());
		//PlayerPrefs.DeleteAll ();


		vecesJugado = PlayerPrefs.GetInt ("vecesJugado", 0);
		vecesJugado++;
		PlayerPrefs.SetInt ("vecesJugado", vecesJugado);
		/*if (vecesJugado <= 2) {
			panelHow.PlayForward ();
			panelBotonHow.PlayReverse ();
			panelPlay.PlayReverse ();
		}*/
	}
    /*
	IEnumerator setNombre(){
		string nombre = "";
		WWWForm form = new WWWForm();
		//num escena
		form.AddField( "param0", SystemInfo.deviceUniqueIdentifier );
		WWW download = new WWW( "http://www.nemorisgames.com/juegos/plataformasar/funciones.php?operacion=4", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
            //mostrarError("Error de conexion");
            yield return null;
        } else {
			nombre = download.text;
			print ("nombre en bd: "+ nombre);
		}

		if (PlayerPrefs.HasKey ("nombre")) {
			if(nombre != PlayerPrefs.GetString ("nombre")) PlayerPrefs.SetString ("nombre", nombre);
			nombreUsuario = PlayerPrefs.GetString ("nombre");
			nombreInput.value = nombreUsuario;
			nombreInputGrande.value = nombreUsuario;
		}
		else{
			if(nombre == "") nombreUsuario = PlayerPrefs.GetString ("nombre", "User" + Random.Range (0, 99999));
			else nombreUsuario = nombre;
			PlayerPrefs.SetString ("nombre", nombreUsuario);
			nombreInput.value = nombreUsuario;
			nombreInputGrande.value = nombreUsuario;
			cambiarNombre(true);
		}
		//panelPlay.PlayForward ();
	}*/

	public void help(){
		/*#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("BotonHelp", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonHelp", false);
		#endif
        */
	}

	public void closeHelp(){
        /*
		#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("BotonHelpClose", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonHelpClose", false);
		#endif*/
	}

	public void verMarcador(){
        /*
		#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("BotonVerMarcador", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonVerMarcador", false);
		#endif*/
	}

	public void rate(){
		/*#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("BotonRate", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonRate", false);
		#endif*/
	}

	public void verPanelNombre(){
		panelNombre.PlayForward ();
		panelNombreChico.PlayReverse ();
	}

	public void esconderPanelNombre(){
		panelNombre.PlayReverse ();
		panelNombreChico.PlayForward ();
	}

	public void cambiarNombreGUI(){
		cambiarNombre ();
	}

	public void cambiarNombre(bool defaultName = false){
		if (nombreInputGrande.value == "") {
			error.text = "Your name can't be empty";
			errorPanel.PlayForward();
			nombreInputGrande.value = PlayerPrefs.GetString ("nombre");
			nombreInput.value = PlayerPrefs.GetString ("nombre");
			return;
		}
		/*#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("CambiarNombre", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("CambiarNombre", false);
		#endif
		StartCoroutine (cambiarNombreBD (defaultName));*/
	}

	IEnumerator cambiarNombreBD(bool defaultName){
		errorPanel.ResetToBeginning ();
		#if !UNITY_WEBPLAYER && !UNITY_STANDALONE
		Handheld.StartActivityIndicator ();
		#endif
		WWWForm form = new WWWForm();
		//num escena
		form.AddField( "param0", SystemInfo.deviceUniqueIdentifier );
		form.AddField( "param1", nombreInputGrande.value );
		//0: web/editor
		//1: android
		//2: ios
		int plataforma = 0;
		#if UNITY_ANDROID && !UNITY_EDITOR
		plataforma = 1;
		#endif
		#if UNITY_IPHONE && !UNITY_EDITOR
		plataforma = 2;
		#endif
		form.AddField( "param2", "" + plataforma );
		WWW download = new WWW( "http://nemorisgames.com/juegos/plataformasar/funciones.php?operacion=3", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
            //mostrarError("Error de conexion");
            yield return null;
        } else {
			string retorno = download.text;
			if(retorno == "repetido"){
				errorPanel.ResetToBeginning();
				error.text = "The name already exist";
				errorPanel.PlayForward();
				nombreInput.value = PlayerPrefs.GetString ("nombre");
				nombreInputGrande.value = PlayerPrefs.GetString ("nombre");
				print ("no existe");
			}
			else{
				//éxito
				if(retorno == "ok"){
					print ("nombre cambiado");
					if(!defaultName){
						error.text = "Name changed";
						errorPanel.PlayForward();
					}
					nombreUsuario = nombreInputGrande.value;
					PlayerPrefs.SetString ("nombre", nombreUsuario);
					nombreInput.value = PlayerPrefs.GetString ("nombre");
					if(!defaultName){
						esconderPanelNombre();
						panelPlay.PlayForward();
					}
				}
			}
			#if !UNITY_WEBPLAYER && !UNITY_STANDALONE
			Handheld.StopActivityIndicator();
			#endif
		}
	}

	public void play(){
		PlayerPrefs.SetInt ("activarAR", AR.value?1:0);
		/*#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("PlayTitulo", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("PlayTitulo", false);
		#endif*/
		if (PlayerPrefs.GetInt ("Historia1", 0) == 0) {
			Application.LoadLevel ("Historia11");
		}
		else Application.LoadLevel ("SeleccionEscena");
	}

	public void cancelSalir(){
		panelSalir.PlayReverse();
	}

	public void salirApp(){
		/*#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("SalirTitulo", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("SalirTitulo", false);
		#endif*/
		Application.Quit();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			panelSalir.Toggle();
		}
	}
}
