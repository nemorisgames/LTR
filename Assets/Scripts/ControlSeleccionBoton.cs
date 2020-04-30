using UnityEngine;
using System.Collections;

public class ControlSeleccionBoton : MonoBehaviour {
	[HideInInspector]
	public int escena;
	UISprite estrellas;
	[HideInInspector]
	public int misIdolos;
	[HideInInspector]
	public int miTiempo;
	[HideInInspector]
	public int miCalificacion;
	[HideInInspector]
	public int idolosPrimero;
	[HideInInspector]
	public string idolosPrimeroNombre;
	[HideInInspector]
	public int idolosSegundo;
	[HideInInspector]
	public string idolosSegundoNombre;
	[HideInInspector]
	public int idolosTercero;
	[HideInInspector]
	public string idolosTerceroNombre;
	[HideInInspector]
	public int tiempoPrimero;
	[HideInInspector]
	public string tiempoPrimeroNombre;
	[HideInInspector]
	public int tiempoSegundo;
	[HideInInspector]
	public string tiempoSegundoNombre;
	[HideInInspector]
	public int tiempoTercero;
	[HideInInspector]
	public string tiempoTerceroNombre;
	public int indiceBloqueo = 0;
	public GameObject particulas;
	public TweenScale tween;

	// Use this for initialization
	void Start () {
		GetComponent<UIButton> ().isEnabled = false;
		escena = int.Parse(transform.Find ("Label").gameObject.GetComponent<UILabel> ().text);
	}

	public void setInformacion(int misIdolosInfo, int miTiempoInfo, int miCalificacionInfo, int idolosPrimeroInfo, string idolosPrimeroNombreInfo, int idolosSegundoInfo, string idolosSegundoNombreInfo, int idolosTerceroInfo, string idolosTerceroNombreInfo, int tiempoPrimeroInfo, string tiempoPrimeroNombreInfo, int tiempoSegundoInfo, string tiempoSegundoNombreInfo, int tiempoTerceroInfo, string tiempoTerceroNombreInfo){
		
		//GetComponent<UIButton> ().isEnabled = false;

		estrellas = transform.Find ("estrellas").gameObject.GetComponent<UISprite> ();

		misIdolos = misIdolosInfo;
		miTiempo = miTiempoInfo;
		miCalificacion = miCalificacionInfo;
		idolosPrimero = idolosPrimeroInfo;
		idolosPrimeroNombre = idolosPrimeroNombreInfo;
		idolosSegundo = idolosSegundoInfo;
		idolosSegundoNombre = idolosSegundoNombreInfo;
		idolosTercero = idolosTerceroInfo;
		idolosTerceroNombre = idolosTerceroNombreInfo;
		tiempoPrimero = tiempoPrimeroInfo;
		tiempoPrimeroNombre = tiempoPrimeroNombreInfo;
		tiempoSegundo = tiempoSegundoInfo;
		tiempoSegundoNombre = tiempoSegundoNombreInfo;
		tiempoTercero = tiempoTerceroInfo;
		tiempoTerceroNombre = tiempoTerceroNombreInfo;
		estrellas.width = 40 * miCalificacionInfo;
	}

	public void seleccionada(){
		ControlSeleccion.Instance.escenaSeleccionada(this);
	}

	public void destacar(){
		particulas.SetActive (true);
		tween.PlayForward();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
