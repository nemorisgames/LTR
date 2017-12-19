using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class tutorialControl : MonoBehaviour {
	public panelTutorial[] paneles;
	public string siguienteNivel;

	public int indice = -1;
	//public string prefTutorial = "tutorial1";
	// Use this for initialization
	void Start () {

		//if(PlayerPrefs.GetInt(prefTutorial, 0) == 1) skip ();
		//else{
		//	paneles[0].gameObject.SetActive(true);
			Time.timeScale = paneles[0].pausar?0f:1f;
		//}
	}

	void evento(int indiceEvento){
		if(indice == indiceEvento && paneles[indice].esperaEvento && (paneles[indice].mensajeTerminado() || paneles [indice].eventoInmediato) ){
			paneles[indice].terminarMensaje();
			print ("evento " + indice + " ev " + indiceEvento );
			next ();
		}
	}

	public void skip(){
		//PlayerPrefs.SetInt(prefTutorial, 1);
		//Time.timeScale = 1f;
		//paneles[indice].mostrarBotones(true);
		//gameObject.SetActive(false);
		if(Application.loadedLevelName == "Historia12") PlayerPrefs.SetInt("Historia1", 1);
		if(Application.loadedLevelName == "Historia31") PlayerPrefs.SetInt("Historia2", 1);
		if(Application.loadedLevelName == "HistoriaFinal2") PlayerPrefs.SetInt("HistoriaFinal1", 1);
		if(siguienteNivel == "Escena"){
			if(Application.loadedLevelName == "Historia31") play (2);
			else play (1);
		}
		else 
			if(siguienteNivel == "") Application.LoadLevel("Historia22");
			else Application.LoadLevel(siguienteNivel);
	}

	public void next(){
		
		if (indice >= paneles.Length - 1 && siguienteNivel == "") return;

		if (indice < 0 || paneles [indice].mensajeTerminado ()) {
			if(indice >= 0){
				print ("botones mostrados");
				paneles[indice].gameObject.SetActive (false);
				print ("panel desactivo");
			}
			indice++;
			if (indice < paneles.Length){
				paneles[indice].gameObject.SetActive (true);
				paneles[indice].mostrarBotones(true);
				print("paneles nuevo activo");
			}
			else{
				if(Application.loadedLevelName == "Historia12") PlayerPrefs.SetInt("Historia1", 1);
				if(Application.loadedLevelName == "Historia31") PlayerPrefs.SetInt("Historia2", 1);
				if(Application.loadedLevelName == "HistoriaFinal2") PlayerPrefs.SetInt("HistoriaFinal1", 1);
				if(siguienteNivel == "Escena"){
					if(Application.loadedLevelName == "Historia31") play (2);
					else play (1);
				}
				else 
					if(siguienteNivel == "") Application.LoadLevel("Historia22");
					else Application.LoadLevel(siguienteNivel);
				return;
			}
			if (indice == paneles.Length - 1) {
				print ("final");
				//PlayerPrefs.SetInt (prefTutorial, 1);
			}
			Time.timeScale = paneles [indice].pausar ? 0f : 1f;
		} 
		else {
			paneles[indice].terminarMensaje();
		}
	}

	public void seleccion1(){
		Application.LoadLevel ("Historia22");
	}

	public void seleccion2(){
		Application.LoadLevel ("Historia23");
	}

	public void seleccion3(){
		Application.LoadLevel ("Historia24");
	}

	public void play(int escena){
		#if UNITY_ANDROID
		playNoAR(escena);
		return;
		#endif
		print ("play " + 1);
		//PlayerPrefs.SetInt ("activarAR", 1);
		PlayerPrefs.SetInt ("escenaActual", escena);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 1 );
		dict.Add( "escenaActual", "" + escena );
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("JugarEscena", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("JugarEscena",dict, false);
		#endif
		#if UNITY_WEBPLAYER
		Application.LoadLevel ("EscenaWebplayer");
		#endif
		#if UNITY_IPHONE
		Application.LoadLevel ("Escena");
		#endif
		#if UNITY_ANDROID
		Application.LoadLevel ("Escena");
		#endif
	}
	
	public void playNoAR(int escena){
		print ("play " + escena);
		PlayerPrefs.SetInt ("activarAR", 0);
		PlayerPrefs.SetInt ("escenaActual", escena);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 0 );
		dict.Add( "escenaActual", "" + escena );
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("JugarEscena", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("JugarEscena",dict, false);
		#endif
		#if UNITY_WEBPLAYER
		Application.LoadLevel ("EscenaWebplayer");
		#else
		Application.LoadLevel ("Escena");
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0) && indice < paneles.Length) {
			print ("toque");
			next ();
		}
	}
}
