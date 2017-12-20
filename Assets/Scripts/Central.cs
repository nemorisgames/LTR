using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Advertisements;
using Prime31;

public class Central : MonoBehaviour {
	public Transform[] bloquesPrefab;
	public Bloque[] bloques;

	public Transform personaje;
	//public Transform imagen;

	public bool traerBD = true;
	[HideInInspector]
	public bool activarAR = true;
	public int escenaActual = 1;
	int pk_escena;
	bool pausa = false;

	public AudioSource audioPrincipal;
	public AudioClip fanfarria;
	public AudioClip perdisteMusica;
	public AudioClip monedaTomada;
	
	//0: inicio
	//1: jugando
	//2: pausa
	//3: terminado ganaste
	//4: terminado perdiste
	public int estado = 0;
	Vector3 posicionInicial;

	//nbloques
	//posInicialJugador
	//tipoN posBloqueN
	string testEscena;// = "7|0|1|0|0|0|0|0|0|0|0|1|0|0|0|2|0|0|0|3|0|0|1|3|0|0|1|4|1|0|1|5";

	public TweenPosition panelPasado;
	public TweenPosition panelPausa;
	public TweenPosition panelPerdiste;

	public GameObject[] elementosActivosGUI;

	public int tiempoMinimo = 0;
	public int tiempoMaximo = 0;

	public float tiempoInicial = 0f;
	public float tiempoActual = 0f;

	public UILabel tiempoLabel;

	public UILabel oroLabel;

	public int calificacion = 0;
	public UISprite estrella;

	public TweenPosition objetoNuevoRecord;

	float ciclo = 0f;
	Transform escenaRoot;
	Transform bloquesRoot;

	public GameObject[] objetosAndroid;
	public GameObject[] objetosWebplayer;
	public GameObject[] mensajeWebplayer;
	bool escenaCargada = false;

	public GameObject mensajeLoading;
	public GameObject mensajeSlide;
	public GameObject mensajeZoom;

	public UILabel tiempoEstrella1;
	public UILabel tiempoEstrella2;
	public UILabel tiempoEstrella3;

	public UILabel temploIndicador;

	private CharacterMotorNemoris characterMotor; 

	public GameObject publicarFacebook;

	bool girando = false;

	ControlMoneda[] monedas;
	ComportamientoIdolo bloqueFinal;

	public GameObject monedaTemporal;
	public PosicionInicial[] objetosMoviles;

    public TweenPosition cinematica;
    public TweenAlpha[] cinematicaBordes;
    /*
    #if UNITY_IPHONE
        private TrackableBehaviour mTrackableBehaviour;

        public void OnTrackableStateChanged(
            TrackableBehaviour.Status previousStatus,
            TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                temploIndicador.gameObject.SetActive(true);
                oroLabel.gameObject.SetActive(false);
            }
            else
            {
                temploIndicador.gameObject.SetActive(false);
                oroLabel.gameObject.SetActive(true);
            }
        }
    #endif
    */
    // Use this for initialization
    void Start () {
		/*
		 * #if UNITY_IPHONE
		mTrackableBehaviour = imagen.gameObject.GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(imagen.gameObject.GetComponent<DefaultTrackableEventHandler>());
		}
		#endif	
		*/
		oroLabel.text = "" + PlayerPrefs.GetInt ("oro", 0);
		characterMotor = personaje.GetComponent<CharacterMotorNemoris> ();
		#if (!UNITY_ANDROID && !UNITY_WEBPLAYER) || UNITY_EDITOR 
		if(objetosAndroid != null && objetosAndroid.Length > 0) for(int i = 0; i < objetosAndroid.Length; i++) objetosAndroid[i].SetActive(false);
#endif
		//escenaRoot = imagen.FindChild ("escenaRoot");
		print (escenaActual + "=" + PlayerPrefs.GetInt ("escenaActual", 1));
		activarAR = PlayerPrefs.GetInt ("activarAR", 1)==1?true:false;
		escenaActual = PlayerPrefs.GetInt ("escenaActual", 1);
		if (escenaActual == 1 && temploIndicador != null) 
			temploIndicador.gameObject.SetActive (false);
		if (temploIndicador != null)
			temploIndicador.text = "Temple " + (escenaActual) + "/21";
#if !UNITY_WEBPLAYER
		if (escenaActual == 1 || escenaActual == 4) mensajeSlide.SetActive (true);
		if (escenaActual == 2) mensajeZoom.SetActive (true);
#endif
#if UNITY_IOS
		if (escenaActual == 2 && !activarAR) mensajeZoom.SetActive (true);
#endif
		#if UNITY_WEBPLAYER
		activarAR = false;
		if(objetosWebplayer.Length > 0) for(int i = 0; i < objetosWebplayer.Length; i++) objetosWebplayer[i].transform.position = Vector3.up * 2000f;
		if(mensajeWebplayer != null && escenaActual == 1) mensajeWebplayer[0].SetActive(true);
		if(mensajeWebplayer != null && escenaActual == 2) mensajeWebplayer[1].SetActive(true);
		#endif
		Time.timeScale = 1f;
		activarElementosGUI (false);
		//activamos el boton de pausa para salir
		elementosActivosGUI[0].SetActive(true);
        /*		if (activarAR)
                    personaje.parent = imagen;
                else {
                    if(Camera.main.gameObject.GetComponent<Vuforia.QCARBehaviour>() != null) Camera.main.gameObject.GetComponent<Vuforia.QCARBehaviour>().enabled = false;
                    if(Camera.main.gameObject.GetComponent<Vuforia.DefaultInitializationErrorHandler>() != null) Camera.main.gameObject.GetComponent<Vuforia.DefaultInitializationErrorHandler>().enabled = false;
                    if(Camera.main.gameObject.GetComponent<Vuforia.DataSetLoadBehaviour>() != null) Camera.main.gameObject.GetComponent<Vuforia.DataSetLoadBehaviour>().enabled = false;
                }*/

        //escenaRoot = imagen.FindChild ("escenaRoot");
        personaje.gameObject.SetActive(false);
		if (escenaRoot == null) escenaRoot = GameObject.Find("escenaRoot").transform;
		bloquesRoot = escenaRoot.Find ("bloquesRoot");
        if (traerBD) {
            //StartCoroutine(cargarEscena(i));
            cargarEscenaEnDispositivo(escenaActual);
        }
        else {
            guardarEscena();
            escenaCargada = true;
            //mensajeLoading.SetActive (false);
        }
	}

	IEnumerator cargarEscena(int escenaActual){
#if !UNITY_WEBPLAYER
		//Handheld.StartActivityIndicator ();
#endif
		WWWForm form = new WWWForm();
		//num escena
		form.AddField( "param0", escenaActual );
		form.AddField( "param1", "2000 - 06 - 12 00:00:00");
		WWW download = new WWW( "http://www.nemorisgames.com/juegos/plataformasar/funciones.php?operacion=0", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
			//mostrarError("Error de conexion");
			yield return null;
		} else {
			string retorno = download.text;
			if(retorno == ""){
				//no existe
				//mostrarAlerta ("El usuario no existe o la clave es errónea");
				print ("no existe");
			}
			else{
                //éxito
                print(escenaActual);
				print ("exito " + retorno);
				string[] ret = retorno.Split(new char[]{'$'});
				print ("id: " + ret[0]);
				pk_escena = int.Parse(ret[0]);
				PlayerPrefs.SetString("escenaActualizada" + escenaActual, ret[1]);
				tiempoMinimo = int.Parse(ret[2]);
				tiempoMaximo = int.Parse(ret[3]);

				tiempoEstrella1.text = (Mathf.FloorToInt(tiempoMaximo / 600f)<=9?"0":"") + Mathf.FloorToInt(tiempoMaximo / 600f) + ":" + (Mathf.FloorToInt((tiempoMaximo / 10f) % 60)<=9?"0":"") + Mathf.FloorToInt((tiempoMaximo / 10f) % 60) + "." + tiempoMaximo%10;
				tiempoEstrella2.text = (Mathf.FloorToInt((tiempoMinimo + (tiempoMaximo - tiempoMinimo)/2f) / 600f)<=9?"0":"") + Mathf.FloorToInt((tiempoMinimo + (tiempoMaximo - tiempoMinimo)/2f) / 600f) + ":" + (Mathf.FloorToInt(((tiempoMinimo + (tiempoMaximo - tiempoMinimo)/2f) / 10f) % 60)<=9?"0":"") + Mathf.FloorToInt(((tiempoMinimo + (tiempoMaximo - tiempoMinimo)/2f) / 10f) % 60) + "." + (tiempoMinimo + (tiempoMaximo - tiempoMinimo)/2f)%10;
				tiempoEstrella3.text = (Mathf.FloorToInt(tiempoMinimo / 600f)<=9?"0":"") + Mathf.FloorToInt(tiempoMinimo / 600f) + ":" + (Mathf.FloorToInt((tiempoMinimo / 10f) % 60)<=9?"0":"") + Mathf.FloorToInt((tiempoMinimo / 10f) % 60) + "." + tiempoMinimo%10;

				if(ret.Length > 4 && ret[4] != ""){
					print ("nueva escena recibida");
					PlayerPrefs.SetString("escena" + escenaActual, ret[4]);
				}
				testEscena = PlayerPrefs.GetString("escena" + escenaActual);
			}

		}
		string[] resp = testEscena.Split(new char[]{'|'});
		int nbloques = int.Parse(resp[0]); 
		personaje.position = new Vector3 (int.Parse (resp [1]), int.Parse (resp [2]) + 1f, int.Parse (resp [3]));

		bloques = new Bloque[nbloques];

		for (int i = 0; i < nbloques; i++) {
			int indiceBloque = int.Parse(resp[i * 4 + 4])>=bloquesPrefab.Length?0:int.Parse(resp[i * 4 + 4]);
			Transform t = (Transform)Instantiate(bloquesPrefab[indiceBloque], new Vector3(int.Parse(resp[i * 4 + 5]), int.Parse(resp[i * 4 + 6]) / 2f + 1f, int.Parse(resp[i * 4 + 7])), Quaternion.identity);
			bloques[i] = t.gameObject.GetComponent<Bloque>();
			bloques[i].aplicarRotacion();
			if(activarAR){ 

			}
			else{ 
				escenaRoot.parent = null;
				personaje.parent = null;
				//personaje.renderer.enabled = true;
				personaje.GetComponent<Collider>().enabled = true;
			}
			if(indiceBloque == 0 || indiceBloque == 2 || indiceBloque == 3 || indiceBloque == 32 || (indiceBloque >= 34 && indiceBloque <= 41))
				t.parent = bloquesRoot;
			else
				t.parent = escenaRoot;
		}
		bloquesRoot.gameObject.SendMessage("calcular");
		MeshFilter[] combinados = bloquesRoot.gameObject.GetComponentsInChildren<MeshFilter> ();
		for (int i = 0; i < combinados.Length; i++) {
			//combinados[i].gameObject.AddComponent<MeshCollider>();
		}
		for (int i = 0; i < nbloques; i++) {
			int indiceBloque = int.Parse(resp[i * 4 + 4])>=bloquesPrefab.Length?0:int.Parse(resp[i * 4 + 4]);
			//if(activarAR){ 
			if(indiceBloque == 0 || indiceBloque == 2 || indiceBloque == 3 || indiceBloque == 32 || (indiceBloque >= 34 && indiceBloque <= 41))
				Destroy(bloques[i].transform.Find("cubo_001_arreglado").gameObject); //Destroy(bloques[i].gameObject); //
			//}
			//hace que los switch carguen info
			if(indiceBloque == 6 || indiceBloque == 9 || indiceBloque == 10){
				bloques[i].gameObject.BroadcastMessage("buscarObjetivos");
			}
		}
		#if !UNITY_WEBPLAYER
		//Handheld.StopActivityIndicator();
		#endif
		if (escenaActual == 1) {
			GameObject g = GameObject.Find("BloqueFinal(Clone)");
			g.transform.Find("idolo").gameObject.SetActive(true);
			g.transform.Find("13").gameObject.SetActive(false);
		}
		posicionInicial = personaje.position;
		monedas = GameObject.FindObjectsOfType<ControlMoneda> ();
		bloqueFinal = GameObject.FindObjectOfType<ComportamientoIdolo> ();
		objetosMoviles = GameObject.FindObjectsOfType<PosicionInicial> ();
		//mensajeLoading.SetActive (false);
		escenaCargada = true;
	}

    void cargarEscenaEnDispositivo(int escenaActual)
    {
        string retorno = "";
        switch (escenaActual)
        {
            case 1: retorno = VariablesGlobales.escena1; break;
            case 2: retorno = VariablesGlobales.escena2; break;
            case 3: retorno = VariablesGlobales.escena3; break;
            case 4: retorno = VariablesGlobales.escena4; break;
            case 5: retorno = VariablesGlobales.escena5; break;
            case 6: retorno = VariablesGlobales.escena6; break;
            case 7: retorno = VariablesGlobales.escena7; break;
            case 8: retorno = VariablesGlobales.escena8; break;
            case 9: retorno = VariablesGlobales.escena9; break;
            case 10: retorno = VariablesGlobales.escena10; break;
            case 11: retorno = VariablesGlobales.escena11; break;
            case 12: retorno = VariablesGlobales.escena12; break;
            case 13: retorno = VariablesGlobales.escena13; break;
            case 14: retorno = VariablesGlobales.escena14; break;
            case 15: retorno = VariablesGlobales.escena15; break;
            case 16: retorno = VariablesGlobales.escena16; break;
            case 17: retorno = VariablesGlobales.escena17; break;
            case 18: retorno = VariablesGlobales.escena18; break;
            case 19: retorno = VariablesGlobales.escena19; break;
            case 20: retorno = VariablesGlobales.escena20; break;
            case 21: retorno = VariablesGlobales.escena21; break;
        }
        string[] ret = retorno.Split(new char[] { '$' });
        print("id: " + ret[0]);
        pk_escena = int.Parse(ret[0]);
        PlayerPrefs.SetString("escenaActualizada" + escenaActual, ret[1]);
        tiempoMinimo = int.Parse(ret[2]);
        tiempoMaximo = int.Parse(ret[3]);

        tiempoEstrella1.text = (Mathf.FloorToInt(tiempoMaximo / 600f) <= 9 ? "0" : "") + Mathf.FloorToInt(tiempoMaximo / 600f) + ":" + (Mathf.FloorToInt((tiempoMaximo / 10f) % 60) <= 9 ? "0" : "") + Mathf.FloorToInt((tiempoMaximo / 10f) % 60) + "." + tiempoMaximo % 10;
        tiempoEstrella2.text = (Mathf.FloorToInt((tiempoMinimo + (tiempoMaximo - tiempoMinimo) / 2f) / 600f) <= 9 ? "0" : "") + Mathf.FloorToInt((tiempoMinimo + (tiempoMaximo - tiempoMinimo) / 2f) / 600f) + ":" + (Mathf.FloorToInt(((tiempoMinimo + (tiempoMaximo - tiempoMinimo) / 2f) / 10f) % 60) <= 9 ? "0" : "") + Mathf.FloorToInt(((tiempoMinimo + (tiempoMaximo - tiempoMinimo) / 2f) / 10f) % 60) + "." + (tiempoMinimo + (tiempoMaximo - tiempoMinimo) / 2f) % 10;
        tiempoEstrella3.text = (Mathf.FloorToInt(tiempoMinimo / 600f) <= 9 ? "0" : "") + Mathf.FloorToInt(tiempoMinimo / 600f) + ":" + (Mathf.FloorToInt((tiempoMinimo / 10f) % 60) <= 9 ? "0" : "") + Mathf.FloorToInt((tiempoMinimo / 10f) % 60) + "." + tiempoMinimo % 10;

        if (ret.Length > 4 && ret[4] != "")
        {
            print("nueva escena recibida");
            PlayerPrefs.SetString("escena" + escenaActual, ret[4]);
        }
        testEscena = PlayerPrefs.GetString("escena" + escenaActual);
string[] resp = testEscena.Split(new char[] { '|' });
int nbloques = int.Parse(resp[0]);
personaje.position = new Vector3(int.Parse (resp[1]), int.Parse(resp[2]) + 1f, int.Parse(resp[3]));

		bloques = new Bloque[nbloques];

		for (int i = 0; i<nbloques; i++) {
			int indiceBloque = int.Parse(resp[i * 4 + 4]) >= bloquesPrefab.Length ? 0 : int.Parse(resp[i * 4 + 4]);
Transform t = (Transform)Instantiate(bloquesPrefab[indiceBloque], new Vector3(int.Parse(resp[i * 4 + 5]), int.Parse(resp[i * 4 + 6]) / 2f + 1f, int.Parse(resp[i * 4 + 7])), Quaternion.identity);
bloques[i] = t.gameObject.GetComponent<Bloque>();
			bloques[i].aplicarRotacion();
			if(activarAR){ 

			}
			else{ 
				escenaRoot.parent = null;
				personaje.parent = null;
				//personaje.renderer.enabled = true;
				personaje.GetComponent<Collider>().enabled = true;
			}
			if(indiceBloque == 0 || indiceBloque == 2 || indiceBloque == 3 || indiceBloque == 32 || (indiceBloque >= 34 && indiceBloque <= 41))
				t.parent = bloquesRoot;
			else
				t.parent = escenaRoot;
		}
		bloquesRoot.gameObject.SendMessage("calcular");
		MeshFilter[] combinados = bloquesRoot.gameObject.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i<combinados.Length; i++) {
			//combinados[i].gameObject.AddComponent<MeshCollider>();
		}
		for (int i = 0; i<nbloques; i++) {
			int indiceBloque = int.Parse(resp[i * 4 + 4]) >= bloquesPrefab.Length ? 0 : int.Parse(resp[i * 4 + 4]);
			//if(activarAR){ 
			if(indiceBloque == 0 || indiceBloque == 2 || indiceBloque == 3 || indiceBloque == 32 || (indiceBloque >= 34 && indiceBloque <= 41))
                Destroy(bloques[i].transform.Find("cubo_001_arreglado").gameObject); //Destroy(bloques[i].gameObject); //
			//}
			//hace que los switch carguen info
			if(indiceBloque == 6 || indiceBloque == 9 || indiceBloque == 10){
				bloques[i].gameObject.BroadcastMessage("buscarObjetivos");
			}
		}
		#if !UNITY_WEBPLAYER
		//Handheld.StopActivityIndicator();
		#endif
		if (escenaActual == 1) {
			GameObject g = GameObject.Find("BloqueFinal(Clone)");
g.transform.Find("idolo").gameObject.SetActive(true);
			g.transform.Find("13").gameObject.SetActive(false);
		}
		posicionInicial = personaje.position;
		monedas = GameObject.FindObjectsOfType<ControlMoneda> ();
		bloqueFinal = GameObject.FindObjectOfType<ComportamientoIdolo> ();
		objetosMoviles = GameObject.FindObjectsOfType<PosicionInicial> ();
		//mensajeLoading.SetActive (false);
		escenaCargada = true;
    }

	void guardarEscena(){
		string ret = "";
		if (bloques == null || bloques.Length <= 0) {
			GameObject[] bs = GameObject.FindGameObjectsWithTag("Bloque");
			bloques = new Bloque[bs.Length];
			for(int i = 0; i < bs.Length; i++){
				bloques[i] = bs[i].GetComponent<Bloque>();
			}
		}
		ret += bloques.Length + "|";
		ret += Mathf.RoundToInt (personaje.position.x) + "|" + Mathf.RoundToInt (personaje.position.y) + "|" + Mathf.RoundToInt (personaje.position.z) + "|";
		for (int i = 0; i < bloques.Length; i++) {
			ret += bloques[i].tipo + "|" + Mathf.RoundToInt (bloques[i].transform.position.x) + "|" + Mathf.RoundToInt (bloques[i].transform.position.y * 2) + "|"+ Mathf.RoundToInt (bloques[i].transform.position.z);
			if(i != bloques.Length - 1) ret += "|";
		}
		print (ret);
	}

	IEnumerator terminarEscena(){
		#if !UNITY_WEBPLAYER
//		Handheld.StartActivityIndicator ();
		#endif
		WWWForm form = new WWWForm();
		//pk_usuario, pk_escena, tiempo, calificacion
		form.AddField( "param0", SystemInfo.deviceUniqueIdentifier );
		form.AddField( "param1", pk_escena );
		form.AddField( "param2", Mathf.FloorToInt(tiempoActual) );
		form.AddField( "param3", calificacion );
		WWW download = new WWW( "http://www.nemorisgames.com/juegos/plataformasar/funciones.php?operacion=1", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
			//mostrarError("Error de conexion");
			yield return null;
		} else {
			int tiempoRecord = int.Parse(download.text);
			if(tiempoRecord == tiempoActual){
				objetoNuevoRecord.PlayForward();
				print ("nuevo record!");
			}
			PlayerPrefs.SetInt("escenaRecord" + escenaActual, tiempoRecord);
		}
#if !UNITY_WEBPLAYER
//		Handheld.StopActivityIndicator();
#endif
	}

	void OnGUI(){
		
		//GUI.Box (new Rect (0f, 0f, Screen.width, 90f), "" + Camera.main.fieldOfView);
		/*if (GUI.Button (new Rect(10, 10, Screen.width / 10f, Screen.height / 10f), "Reset")) {
			Application.LoadLevel(Application.loadedLevelName);
		}*/
	}

	public IEnumerator girarEscena(float ang){
		yield return new WaitForEndOfFrame ();
		if (estado == 1 && tiempoActual > 15) {
			if(characterMotor.IsJumping ()) personaje.RotateAround (escenaRoot.position, Vector3.up, ang);
			escenaRoot.RotateAround (escenaRoot.position, Vector3.up, ang);
			if (mensajeSlide.activeSelf) mensajeSlide.SetActive (false);
		}
	}

	public void girarEscena90(){
		if (girando || !(estado == 1 && tiempoActual > 0.15f)) return;
		girando = true;
		StartCoroutine (girarEscena90Wait (-90f));
	}

	public void girarEscena90count(){
		if (girando || !(estado == 1 && tiempoActual > 0.15f)) return;
		girando = true;
		StartCoroutine (girarEscena90Wait (90f));
	}

	public IEnumerator girarEscena90Wait(float angulo){
		//for (int i = 0; i < 360; i++) {
		float signo = Mathf.Sign (angulo);
		//print (Mathf.Abs (angulo)+ " " + escenaRoot.localEulerAngles.y % 90 + " " + (escenaRoot.localEulerAngles.y%90 < 0.1f && escenaRoot.localEulerAngles.y%90 > -0.1f) +"||"+ (escenaRoot.localEulerAngles.y%90 < 90.1f && escenaRoot.localEulerAngles.y%90 > 89.9f));
		//print (Mathf.Abs (angulo) +"-"+ "("+90f +"- "+ (((+escenaRoot.localEulerAngles.y % 90 < 0.1f && escenaRoot.localEulerAngles.y % 90 > -0.1f) || (escenaRoot.localEulerAngles.y % 90 < 90.1f && escenaRoot.localEulerAngles.y % 90 > 89.9f)) ? 90f : escenaRoot.localEulerAngles.y % 90));
		if(signo < 0f || ((escenaRoot.localEulerAngles.y%90 < 0.1f && escenaRoot.localEulerAngles.y%90 > -0.1f) || (escenaRoot.localEulerAngles.y%90 < 90.1f && escenaRoot.localEulerAngles.y%90 > 89.9f)))
			angulo = Mathf.Abs ( Mathf.Abs (angulo) - (90f - (((escenaRoot.localEulerAngles.y%90 < 0.1f && escenaRoot.localEulerAngles.y%90 > -0.1f) || (escenaRoot.localEulerAngles.y%90 < 90.1f && escenaRoot.localEulerAngles.y%90 > 89.9f))?90f:escenaRoot.localEulerAngles.y%90)));
		else angulo = Mathf.Abs ( Mathf.Abs (angulo) - ((((escenaRoot.localEulerAngles.y%90 < 0.1f && escenaRoot.localEulerAngles.y%90 > -0.1f) || (escenaRoot.localEulerAngles.y%90 < 90.1f && escenaRoot.localEulerAngles.y%90 > 89.9f))?90f:escenaRoot.localEulerAngles.y%90)));


		while(angulo >= 0f){
			float giro = 45f * Time.deltaTime;
			yield return new WaitForEndOfFrame();
			angulo -= giro;
			if(angulo < 0f) giro += angulo;
			StartCoroutine (girarEscena (signo * giro));

			//print (giro);
		}
		//StartCoroutine (girarEscena (signo * escenaRoot.eulerAngles.y % 90));
		girando = false;
		/*
		yield return new WaitForEndOfFrame ();
		if (estado == 1 && tiempoActual > 15) {
			float rotacionObjetivo = 0f;
			for(int i = 0; i < 4; i++){
				if(escenaRoot.localRotation.y >= 90f * i && escenaRoot.localRotation.y < 90f * (i + 1)){
					rotacionObjetivo = 90f * (i + 1);
				}
			}
			if(characterMotor.IsJumping ()) personaje.RotateAround (escenaRoot.position, Vector3.up, rotacionObjetivo);
			//personaje.transform.parent = escenaRoot;
			escenaRoot.RotateAround (escenaRoot.position, Vector3.up, rotacionObjetivo);
			if (mensajeSlide.activeSelf) mensajeSlide.SetActive (false);
		}
		*/
	}

	public void zoom(float val){
		if (estado == 1 && tiempoActual > 0.15f) {
			Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + val * 0.1f, 28f, 70f);
			if (mensajeZoom.activeSelf) mensajeZoom.SetActive (false);
		}

	}

	public void pausar(){
		if (estado == 1 || estado == 2) {
				Dictionary<string,string> dict = new Dictionary<string,string> ();
				dict.Add ("escenaActual", "" + escenaActual);
	#if UNITY_IPHONE
	FlurryAnalytics.logEventWithParameters ("BotonPausa", dict, false);
	#endif
	#if UNITY_ANDROID
				FlurryAndroid.logEvent ("BotonPausa", dict, false);
	#endif
				pausa = !pausa;
				//if(!panelPausa.enabled){ 
				panelPausa.Play (pausa);
				//}
				activarElementosGUI (!pausa);
				Time.timeScale = (pausa ? 0f : 1f);
		}
	}

	void activarElementosGUI(bool a){
		for (int i = 0; i < elementosActivosGUI.Length; i++) {
			elementosActivosGUI[i].SetActive(a);
		}
		#if !UNITY_ANDROID || UNITY_EDITOR || UNITY_WEBPLAYER
		for(int i = 0; i < objetosAndroid.Length; i++) objetosAndroid[i].SetActive(false);
		#endif
		#if UNITY_WEBPLAYER
		for(int i = 0; i < objetosWebplayer.Length; i++)  objetosWebplayer[i].transform.position = Vector3.up * 2000f;
		#endif
	}

	public void continuar(){
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "escenaActual", "" + escenaActual );
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("BotonContinuar", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonContinuar", dict, false);
		#endif
		#if UNITY_WEBPLAYER && !UNITY_EDITOR
		GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("Boton", "Continuar", "Escena"+escenaActual);
		#endif
		if (escenaActual == 1 && PlayerPrefs.GetInt ("Historia2", 0) == 0)
				Application.LoadLevel ("Historia21");
		else {
			if(escenaActual == 21){
				if(PlayerPrefs.GetInt("HistoriaFinal1", 0) == 0)
					Application.LoadLevel ("HistoriaFinal1");
				else 
					Application.LoadLevel("SeleccionEscena");
			}
			else{
				if(activarAR) play ();
				else playNoAR();
			}
		}
	}



	void play(){
		#if UNITY_ANDROID
		playNoAR();
		return;
		#endif
		escenaActual++;
		print ("play " + escenaActual);
		PlayerPrefs.SetInt ("activarAR", 1);
		PlayerPrefs.SetInt ("escenaActual", escenaActual);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 1 );
		dict.Add( "escenaActual", "" + escenaActual );
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
	
	void playNoAR(){
		escenaActual++;
        PlayerPrefs.SetInt("conteoAds", PlayerPrefs.GetInt("conteoAds", 0) + 1);
        if (PlayerPrefs.GetInt("conteoAds", 0) % 3 == 0 && PlayerPrefs.GetInt("conteoAds", 0) >= 3)
            showAd();
        print ("play " + escenaActual);
		PlayerPrefs.SetInt ("activarAR", 0);
		PlayerPrefs.SetInt ("escenaActual", escenaActual);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 0 );
		dict.Add( "escenaActual", "" + escenaActual );
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

	public void replayNoAR(){
		PlayerPrefs.SetInt ("activarAR", 0);
		PlayerPrefs.SetInt ("escenaActual", escenaActual);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "activarAR", "" + 0 );
		dict.Add( "escenaActual", "" + escenaActual );
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

	public void exit(){
        PlayerPrefs.SetInt("conteoAds", PlayerPrefs.GetInt("conteoAds", 0) + 1);
        if (PlayerPrefs.GetInt("conteoAds", 0) % 3 == 0 && PlayerPrefs.GetInt("conteoAds", 0) >= 3)
            showAd();
        Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "escenaActual", "" + escenaActual );
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("BotonExit", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonExit", dict, false);
		#endif
		#if UNITY_WEBPLAYER && !UNITY_EDITOR
		GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("Boton", "Exit", "Escena"+escenaActual);
		#endif
		Application.LoadLevel ("SeleccionEscena");
	}

	public void again(){
		mensajeLoading.SetActive (true);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "escenaActual", "" + escenaActual );
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("BotonAgain", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonAgain", dict, false);
		#endif
		#if UNITY_WEBPLAYER && !UNITY_EDITOR
		GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("Boton", "Again", "Escena"+escenaActual);
		#endif
		//Application.LoadLevel(Application.loadedLevelName);
		resetEscena ();
	}

	public void reset(){
		mensajeLoading.SetActive (true);
		Dictionary<string,string> dict = new Dictionary<string,string>();
		dict.Add( "escenaActual", "" + escenaActual );
		#if UNITY_IPHONE
		FlurryAnalytics.logEventWithParameters ("BotonReset", dict, false);
		#endif
		#if UNITY_ANDROID
		FlurryAndroid.logEvent ("BotonReset", dict, false);
		#endif
		#if UNITY_WEBPLAYER && !UNITY_EDITOR 
		GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("Boton", "Reset", "Escena"+escenaActual);
		#endif
		//Application.LoadLevel(Application.loadedLevelName);
		resetEscena ();
	}

	void resetEscena(){
        PlayerPrefs.SetInt("conteoAds", PlayerPrefs.GetInt("conteoAds", 0) + 1);
        if (PlayerPrefs.GetInt("conteoAds", 0) % 3 == 0 && PlayerPrefs.GetInt("conteoAds", 0) >= 3)
            showAd();
        estado = 0;
        //activarElementosGUI (true);
        cinematicaBordes[0].ResetToBeginning();
        cinematicaBordes[1].ResetToBeginning();
        cinematica.ResetToBeginning();
        panelPasado.ResetToBeginning();
		panelPausa.ResetToBeginning ();
		panelPerdiste.ResetToBeginning();
		escenaRoot.rotation = Quaternion.identity;
		escenaRoot.position = Vector3.zero;
		characterMotor.posicionSegura = posicionInicial;
		personaje.SendMessage ("reset");
		personaje.position = posicionInicial;
        objetoNuevoRecord.PlayReverse();
        if (monedas == null)
			monedas = GameObject.FindObjectsOfType<ControlMoneda> ();
		if(bloqueFinal == null)
			bloqueFinal = GameObject.FindObjectOfType<ComportamientoIdolo> ();

		foreach (ControlMoneda m in monedas) m.gameObject.SetActive (true);
		bloqueFinal.gameObject.SetActive (true);
		if (objetosMoviles == null || objetosMoviles.Length == 0) {
			objetosMoviles = GameObject.FindObjectsOfType<PosicionInicial> ();
			print ("elementos " + objetosMoviles.Length);
		}
		foreach (PosicionInicial p in objetosMoviles) { 
			p.reset ();
			p.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
		//if (!mensajeSlide.activeSelf) mensajeSlide.SetActive (true);
		//if (!mensajeZoom.activeSelf) mensajeZoom.SetActive (true);
		if(Camera.main.gameObject.GetComponent<CNCameraFollow>() != null && !Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled) 
			Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled = true;
		audioPrincipal.Play();
		personaje.SendMessage("activar", true);
        personaje.gameObject.SetActive(false);
        ciclo = 0f;
		//mensajeLoading.SetActive (false);
		escenaCargada = true;
	}

	void perdiste(){
		if (PlayerPrefs.GetInt ("oro", 0) <= 0)
			estado = 4;
		else {
			StartCoroutine(caidaLava());
		}
	}

	IEnumerator caidaLava(){
		yield return new WaitForSeconds (1.3f);
		for (int i = 0; i < Mathf.Clamp(PlayerPrefs.GetInt("oro", 0), 0, 20); i++) {
			Instantiate(monedaTemporal, personaje.transform.position, Quaternion.identity);
		}
		PlayerPrefs.SetInt ("oro", 0);
		oroLabel.text = "" + 0;
		yield return new WaitForSeconds (1f);
		personaje.SendMessage ("proteccion");
		personaje.SendMessage ("reset");
	}

	void tomarOro(){
		PlayerPrefs.SetInt ("oro", PlayerPrefs.GetInt ("oro", 0) + 1);
		oroLabel.text = "" + PlayerPrefs.GetInt ("oro", 0);
		audioPrincipal.PlayOneShot (monedaTomada);
		#if UNITY_WEBPLAYER && !UNITY_EDITOR
		GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("Oro", "", "Escena"+escenaActual);
		#endif
	}

    public void cinematicaTerminada()
    {
        Camera.main.transform.position = new Vector3(3.81f, 5f, -11.8f);
        Camera.main.transform.LookAt(Vector3.zero, Vector3.up);
        cinematicaBordes[0].PlayForward();
        cinematicaBordes[1].PlayForward();

        personaje.gameObject.SetActive(true);
        if (!activarAR && Camera.main.gameObject.GetComponent<TweenFOV>() != null) Camera.main.gameObject.GetComponent<TweenFOV>().PlayForward();
        if (Camera.main.gameObject.GetComponent<CNCameraFollow>() != null && !Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled)
            Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled = true;
        if (personaje.gameObject.GetComponent<CharacterController>().enabled)
        {
            activarElementosGUI(true);
            //panelPausa.PlayReverse();
            Time.timeScale = 1f;
            estado = 1;
            personaje.SendMessage("activar", true);
            tiempoInicial = Time.timeSinceLevelLoad;
        }
    }

    public void showAd()
    {
        /*if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }*/
    }
    // Update is called once per frame
    void Update () {
		switch(estado){
		case 0:
			Time.timeScale = 0f;
                Camera.main.transform.LookAt(new Vector3(0f, 4f, 0f), Vector3.up);
            if (escenaCargada && mensajeLoading.activeSelf)
            {
                Time.timeScale = 1f;
                mensajeLoading.SetActive(false);
                cinematica.PlayForward();
			}
			break;
		case 1:
			#if UNITY_WEBPLAYER
			if(Input.GetAxis("ArrowsHorizontal") != 0f){
				StartCoroutine(girarEscena (Input.GetAxis("ArrowsHorizontal") * 50f * Time.deltaTime));
			}
			//if(Input.GetKeyUp(KeyCode.RightArrow))
			//	escenaRoot.RotateAround (escenaRoot.position, Vector3.up, 90f);
			if(Input.GetAxis("ArrowsVertical") != 0f){
				zoom (Input.GetAxis("ArrowsVertical") * -80f * Time.deltaTime);
			}
			#endif
			if (Input.GetKeyUp(KeyCode.Escape)) pausar();
			if (Input.GetAxis("Mouse ScrollWheel") != 0f) zoom(Input.GetAxis("Mouse ScrollWheel") * - 20f);
			tiempoActual = Mathf.FloorToInt((Time.timeSinceLevelLoad - tiempoInicial) * 10f);
			tiempoLabel.text =  (Mathf.FloorToInt(tiempoActual / 600f)<=9?"0":"") + Mathf.FloorToInt(tiempoActual / 600f) + ":" + (Mathf.FloorToInt((tiempoActual / 10f) % 60)<=9?"0":"") + Mathf.FloorToInt((tiempoActual / 10f) % 60) + "." + tiempoActual%10;
			break;
		case 2:
			if (Input.GetKeyUp(KeyCode.Escape)) pausar();
			break;
		case 3:
			if(!panelPasado.enabled && Vector3.Distance(panelPasado.from, panelPasado.position) < 10f){
                if (mensajeSlide.activeSelf) mensajeSlide.SetActive (false);
				if (mensajeZoom.activeSelf) mensajeZoom.SetActive (false);
				if(Camera.main.gameObject.GetComponent<CNCameraFollow>() != null && Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled) 
					Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled = false;
				GameObject publicidad = GameObject.Find("leadboltPublicidad");
				if(publicidad != null) publicidad.SendMessage("verAd");
				audioPrincipal.Stop();
				audioPrincipal.PlayOneShot(fanfarria);
				activarElementosGUI (false);
				panelPasado.PlayForward();
				if(tiempoActual > tiempoMaximo) calificacion = 0;
				else{
					if(tiempoActual > tiempoMinimo + (tiempoMaximo - tiempoMinimo) / 2f){
						calificacion = 1;
					}
					else{
						if(tiempoActual > tiempoMinimo){
							calificacion = 2;
						}
						else
							calificacion = 3;
					}
				}
				estrella.width = 112 * calificacion;
				personaje.SendMessage("activar", false);
				Dictionary<string,string> dict = new Dictionary<string,string>();
				dict.Add( "calificacion", "" + calificacion );
				dict.Add( "tiempo", "" + tiempoActual );
				dict.Add( "escenaActual", "" + escenaActual );
				#if UNITY_IPHONE
				FlurryAnalytics.logEventWithParameters ("EscenaPasada", dict, false);
				#endif
				#if UNITY_ANDROID
				FlurryAndroid.logEvent ("EscenaPasada",dict, false);
#endif
#if UNITY_WEBPLAYER && !UNITY_EDITOR
				GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("EscenaPasada", "", "Escena" + escenaActual);
#endif
                PlayerPrefs.SetInt("calificacionEscena" + escenaActual, calificacion);
				if(PlayerPrefs.GetInt("escenasPasadas", 0) < escenaActual) PlayerPrefs.SetInt("escenasPasadas", escenaActual);
				StartCoroutine(terminarEscena ());
				if(publicarFacebook != null) publicarFacebook.SendMessage("setMensaje", "I've passed the Stage " + escenaActual + " in " + (tiempoActual / 10f) + " seconds. Can anybody beat me?");

			}
			if (Input.GetKeyUp(KeyCode.Escape)) exit();
			//print ("terminado");
			break;
		case 4:
			if(!panelPerdiste.enabled && Vector3.Distance(panelPerdiste.from, panelPerdiste.position) < 10f){

                if (mensajeSlide.activeSelf) mensajeSlide.SetActive (false);
				if (mensajeZoom.activeSelf) mensajeZoom.SetActive (false);
				if(Camera.main.gameObject.GetComponent<CNCameraFollow>() != null && Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled) 
					Camera.main.gameObject.GetComponent<CNCameraFollow>().enabled = false;
				GameObject publicidad = GameObject.Find("leadboltPublicidad");
				if(publicidad != null) publicidad.SendMessage("verAd");
				audioPrincipal.Stop();
				audioPrincipal.PlayOneShot(perdisteMusica);
				activarElementosGUI (false);
				personaje.SendMessage("activar", false);
				Dictionary<string,string> dict = new Dictionary<string,string>();
				dict.Add( "tiempo", "" + tiempoActual );
				dict.Add( "escenaActual", "" + escenaActual );
				#if UNITY_IPHONE
				FlurryAnalytics.logEventWithParameters ("EscenaPerdiste", dict, false);
				#endif
				#if UNITY_ANDROID
				FlurryAndroid.logEvent ("EscenaPerdiste",dict, false);
				#endif
				#if UNITY_WEBPLAYER && !UNITY_EDITOR
				GameObject.Find("TrackingWebplayer").GetComponent<TrackingNemoris>().enviarLog("EscenaPerdiste", "", "Escena" + escenaActual);
				#endif
				panelPerdiste.PlayForward();
			}
			if (Input.GetKeyUp(KeyCode.Escape)) exit();
			//print ("terminado");
			break;
		}
	}

}
