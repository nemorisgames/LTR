using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Prime31;

public class ControlSeleccion : MonoBehaviour {
    public static ControlSeleccion Instance;
	public ControlSeleccionBoton[] botones; 
	public ControlSeleccionBloqueo[] bloqueos;
	public int[] estrellasBloqueo;
	public TweenPosition panelDetalles;
	public TweenPosition panelPuntuar;
	int escenaActual = 0;
	public GameObject[] objetosAndroid;
	public GameObject[] objetosWebplayer;

	public UILabel errorLabel;
	public GameObject loadingLabel;

	public UIScrollView scrollView;

	public string urlAndroid;
	public string urlIOS;

	public GameObject botonHistoriaFinal1;
    private ControlSeleccionDetalles controlDetalles;

    // Use this for initialization
    void Awake(){
        if(Instance != null)
            Destroy(Instance);
        Instance = this;
    }

    void Start() {
        controlDetalles = panelDetalles.GetComponent<ControlSeleccionDetalles>();
        botonHistoriaFinal1.SetActive(PlayerPrefs.GetInt("HistoriaFinal1", 0) == 1);
        PlayerPrefs.SetInt("vecesSeleccion", PlayerPrefs.GetInt("vecesSeleccion", 0) + 1);
        print("veces " + PlayerPrefs.GetInt("vecesSeleccion"));
        if (PlayerPrefs.GetInt("vecesSeleccion") % 4 == 0 || PlayerPrefs.GetInt("vecesSeleccion") == 2 && PlayerPrefs.GetInt("noRecordar", 0) == 0) {
#if UNITY_ANDROID
            //if (urlAndroid != "") panelPuntuar.PlayForward();
#endif
#if UNITY_IOS
			//if(urlIOS != "") panelPuntuar.PlayForward();
#endif
        }
#if UNITY_ANDROID
        for (int i = 0; i < objetosAndroid.Length; i++) objetosAndroid[i].SetActive(!objetosAndroid[i].activeSelf);
#endif
#if UNITY_WEBPLAYER
		for(int i = 0; i < objetosWebplayer.Length; i++) objetosWebplayer[i].SetActive(!objetosWebplayer[i].activeSelf);
#endif
        estrellasBloqueo = new int[bloqueos.Length];

        Vector3 posProximaEscena = Vector3.zero;
        int ultimaEscenaPasada = PlayerPrefs.GetInt("escenasPasadas", 0);
        int ultimaEscenaRecibida = 21;
        int misIdolos = 0;
        int miTiempo = 0;
        loadingLabel.SetActive(false);
        if (ultimaEscenaPasada > PlayerPrefs.GetInt("escenasPasadas", 0))
            PlayerPrefs.SetInt("escenasPasadas", ultimaEscenaPasada);

        for (int j = 0; j < botones.Length; j++)
        {
            // if (botones[j].escena == numero)
            //{
            int miCalificacion = PlayerPrefs.GetInt("calificacionEscena" + botones[j].escena, 0);
            miTiempo = PlayerPrefs.GetInt("escenaRecord" + botones[j].escena);
            misIdolos = PlayerPrefs.GetInt("escenaIdolos" + botones[j].escena);
            print(miCalificacion);
            botones[j].enabled = true;
            botones[j].setInformacion(misIdolos, miTiempo, miCalificacion, 0, "Loading...", 0, "Loading...", 0, "Loading...", 0, "Loading...", 0, "Loading...", 0, "Loading...");
            //  if (miTiempo > 0)
            // {
            //     if (ultimaEscenaPasada == 0 || ultimaEscenaPasada < numero)
            //        ultimaEscenaPasada = numero;
            // }
            // break;
            // }
        }
        //desbloqueamos la escena siguiente y las anteriores
        for (int j = 0; j < botones.Length; j++)
        {
            if (botones[j].escena <= ultimaEscenaPasada + 1 && botones[j].escena <= ultimaEscenaRecibida)
            {
                print("habilita escena " + botones[j].escena);
                botones[j].gameObject.GetComponent<UIButton>().isEnabled = true;
                posProximaEscena = botones[j].transform.localPosition;
                if (botones[j].escena == ultimaEscenaPasada + 1)
                {
                    botones[j].destacar();
                }
                if (botones[j].indiceBloqueo < estrellasBloqueo.Length)
                    estrellasBloqueo[botones[j].indiceBloqueo] += botones[j].miCalificacion;
            }
        }
    
        for (int i = 0; i<bloqueos.Length; i++)
        {
            bloqueos[i].setInformacion(estrellasBloqueo[i], -1);
        }
        //
        for (int j = 0; j<botones.Length; j++)
        {
            if (botones[j].indiceBloqueo > 0 && bloqueos[botones[j].indiceBloqueo - 1].bloqueado)
            {
                botones[j].gameObject.GetComponent<UIButton>().isEnabled = false;
            }
        }

        //StartCoroutine(cargarInformacionMapa());
        scrollView.panel.transform.localPosition = -posProximaEscena;
        scrollView.panel.clipOffset = new Vector2(posProximaEscena.x, posProximaEscena.y);
        scrollView.RestrictWithinBounds(true);
    }
    /*
	IEnumerator cargarInformacionMapa(){

		#if !UNITY_WEBPLAYER
		//Handheld.StartActivityIndicator ();
		#endif
		WWWForm form = new WWWForm();
		//num escena
		form.AddField( "param0", SystemInfo.deviceUniqueIdentifier );
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
		form.AddField( "param1", "" + plataforma );
		WWW download = new WWW("http://www.nemorisgames.com/juegos/plataformasar/funciones.php?operacion=2", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
			errorLabel.text = "please activate internet in order to get data";
            //mostrarError("Error de conexion");
            yield return null;
        } else {
			string retorno = download.text;
			if(retorno == ""){
				//no existe
				//mostrarAlerta ("El usuario no existe o la clave es errónea");
				print ("no existe");
			}
            else
            {
                //éxito
                print("exito " + retorno);
                string[] ret = retorno.Split(new char[] { '|' });
                int ultimaEscenaPasada = 0;
                int ultimaEscenaRecibida = 0;
                for (int i = 0; i < Mathf.FloorToInt(ret.Length / 16); i++)
                {
                    int numero = ret[i * 16] == "" ? 0 : int.Parse(ret[i * 16]);
                    //print (ret[i * 16 + 1]);
                    int misIdolos = ret[i * 16 + 1] == "" ? 0 : int.Parse(ret[i * 16 + 1]);
                    int miTiempo = ret[i * 16 + 2] == "" ? 0 : int.Parse(ret[i * 16 + 2]);
                    int miCalificacion = ret[i * 16 + 3] == "" ? 0 : int.Parse(ret[i * 16 + 3]);
                    int idolosPrimero = ret[i * 16 + 4] == "" ? 0 : int.Parse(ret[i * 16 + 4]);
                    string idolosPrimeroNombre = ret[i * 16 + 5];
                    int idolosSegundo = ret[i * 16 + 6] == "" ? 0 : int.Parse(ret[i * 16 + 6]);
                    string idolosSegundoNombre = ret[i * 16 + 7];
                    int idolosTercero = ret[i * 16 + 8] == "" ? 0 : int.Parse(ret[i * 16 + 8]);
                    string idolosTerceroNombre = ret[i * 16 + 9];
                    int tiempoPrimero = ret[i * 16 + 10] == "" ? 0 : int.Parse(ret[i * 16 + 10]);
                    string tiempoPrimeroNombre = ret[i * 16 + 11];
                    int tiempoSegundo = ret[i * 16 + 12] == "" ? 0 : int.Parse(ret[i * 16 + 12]);
                    string tiempoSegundoNombre = ret[i * 16 + 13];
                    int tiempoTercero = ret[i * 16 + 14] == "" ? 0 : int.Parse(ret[i * 16 + 14]);
                    string tiempoTerceroNombre = ret[i * 16 + 15];

                    if (ultimaEscenaRecibida == 0 || ultimaEscenaRecibida < numero)
                        ultimaEscenaRecibida = numero;

                    for (int j = 0; j < botones.Length; j++)
                    {
                        if (botones[j].escena == numero)
                        {
                            botones[j].enabled = true;
                            botones[j].setInformacion(misIdolos, miTiempo, miCalificacion, idolosPrimero, idolosPrimeroNombre, idolosSegundo, idolosSegundoNombre, idolosTercero, idolosTerceroNombre, tiempoPrimero, tiempoPrimeroNombre, tiempoSegundo, tiempoSegundoNombre, tiempoTercero, tiempoTerceroNombre);
                            if (miTiempo > 0)
                            {
                                if (ultimaEscenaPasada == 0 || ultimaEscenaPasada < numero)
                                    ultimaEscenaPasada = numero;
                            }
                            break;
                        }
                    }
                }
                for (int j = 0; j < botones.Length; j++)
                {
                    if (botones[j].escena <= ultimaEscenaPasada + 1 && botones[j].escena <= ultimaEscenaRecibida)
                    {
                        print("habilita escena " + botones[j].escena);
                        botones[j].gameObject.GetComponent<UIButton>().isEnabled = true;
                        if (botones[j].escena == ultimaEscenaPasada + 1)
                        {
                            botones[j].destacar();
                        }
                        if (botones[j].indiceBloqueo < estrellasBloqueo.Length)
                            estrellasBloqueo[botones[j].indiceBloqueo] += botones[j].miCalificacion;
                    }
                }
                for (int i = 0; i < bloqueos.Length; i++)
                {
                    bloqueos[i].setInformacion(estrellasBloqueo[i], -1);
                }
                //
                for (int j = 0; j < botones.Length; j++)
                {
                    if (botones[j].indiceBloqueo > 0 && bloqueos[botones[j].indiceBloqueo - 1].bloqueado)
                    {
                        botones[j].gameObject.GetComponent<UIButton>().isEnabled = false;
                    }
                }
            }
        

        }
            #if !UNITY_WEBPLAYER
                        //Handheld.StopActivityIndicator();
            #endif

        
	}
    */
	public void back(){
        /*
		#if UNITY_IPHONE
		FlurryAnalytics.logEvent ("BotonBackSeleccion", false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonBackSeleccion", false);
		#endif*/
		Application.LoadLevel ("Titulo");
	}

	public void totem(){
		Application.LoadLevel ("CompletarTotem");
	}

	public void play(){
		#if UNITY_ANDROID
		playNoAR();
		return;
		#endif
		print ("play " + escenaActual);
		PlayerPrefs.SetInt ("activarAR", 1);
		PlayerPrefs.SetInt ("escenaActual", escenaActual);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 1 );
		dict.Add( "escenaActual", "" + escenaActual );
        /*
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("JugarEscena", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("JugarEscena",dict, false);
		#endif*/
		#if UNITY_WEBPLAYER
		Application.LoadLevel ("EscenaWebplayer");
		#else
		Application.LoadLevel ("Escena");
		#endif
	}

	public void playNoAR(){
		print ("play " + escenaActual);
		PlayerPrefs.SetInt ("activarAR", 0);
		PlayerPrefs.SetInt ("escenaActual", escenaActual);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 0 );
		dict.Add( "escenaActual", "" + escenaActual );
        /*
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("JugarEscena", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("JugarEscena",dict, false);
		#endif
        */
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

	public void verHistoria1(){
		Application.LoadLevel ("Historia11");
	}
	public void verHistoria2(){
		Application.LoadLevel ("Historia21");
	}
	public void verFinal(){
		Application.LoadLevel ("HistoriaFinal1");
	}

	public void cancel(){
		panelDetalles.PlayReverse ();
	}

	public void rankIt(){
		/*
         * #if UNITY_ANDROID && !UNITY_EDITOR
		Application.OpenURL(urlAndroid);
		FlurryAndroid.logEvent ("RankIt", false);
		#endif
		#if UNITY_IPHONE && !UNITY_EDITOR
		Application.OpenURL(urlIOS);
		FlurryAnalytics.logEvent ("RankIt", false);
		#endif
		PlayerPrefs.SetInt ("noRecordar", 1);
		panelPuntuar.PlayReverse ();
        */
	}

	public void notNow(){
		panelPuntuar.PlayReverse ();
	}

	public void dontRemind(){
		PlayerPrefs.SetInt ("noRecordar", 1);
		panelPuntuar.PlayReverse ();
	}

	public void escenaSeleccionada(ControlSeleccionBoton boton){
		print ("escena sel " + boton.escena);
		escenaActual = boton.escena;
		panelDetalles.PlayForward ();
        controlDetalles.setInformacion(boton);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if(Vector3.Distance(panelDetalles.to, panelDetalles.transform.localPosition) < 0.1f) cancel ();
			else back ();
		}
	}
}
