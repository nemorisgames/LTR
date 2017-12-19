using UnityEngine;
using System.Collections;

public class ControlSeleccionDetalles : MonoBehaviour {
	public UILabel[] recordTiempo;
	public UILabel[] recordTiempoNombre;
	public UILabel[] recordIdolo;
	public UILabel[] recordIdoloNombre;
	public UILabel miTiempo;
	public UILabel misIdolos;
	public UISprite estrellas;
	// Use this for initialization
	void Start () {

	}

	void setInformacion(ControlSeleccionBoton c){
		recordTiempo [0].text = setTiempo(c.tiempoPrimero);
		recordTiempoNombre [0].text = c.tiempoPrimeroNombre==""?"-":c.tiempoPrimeroNombre;
		recordTiempo [1].text = setTiempo(c.tiempoSegundo);
		recordTiempoNombre [1].text = c.tiempoSegundoNombre==""?"-":c.tiempoSegundoNombre;
		recordTiempo [2].text = setTiempo(c.tiempoTercero);
		recordTiempoNombre [2].text = c.tiempoTerceroNombre==""?"-":c.tiempoTerceroNombre;
		
		recordIdolo [0].text = c.idolosPrimero==0?"-":""+c.idolosPrimero;
		recordIdoloNombre [0].text = c.idolosPrimeroNombre==""?"-":c.idolosPrimeroNombre;
		recordIdolo [1].text = c.idolosSegundo==0?"-":""+c.idolosSegundo;
		recordIdoloNombre [1].text = c.idolosSegundoNombre==""?"-":c.idolosSegundoNombre;
		recordIdolo [2].text = c.idolosTercero==0?"-":""+c.idolosTercero;
		recordIdoloNombre [2].text = c.idolosTerceroNombre==""?"-":c.idolosTerceroNombre;

		miTiempo.text = setTiempo(c.miTiempo);
		misIdolos.text = "x " + (c.misIdolos == 0 ? "-" : "" + c.misIdolos);
		estrellas.width = c.miCalificacion * 112;
	}

	string setTiempo(int tiempoActual){
		if (tiempoActual == 0)
			return "-";
		return (Mathf.FloorToInt(tiempoActual / 600f)<=9?"0":"") + Mathf.FloorToInt(tiempoActual / 600f) + ":" + (Mathf.FloorToInt((tiempoActual / 10f) % 60)<=9?"0":"") + Mathf.FloorToInt((tiempoActual / 10f) % 60) + "." + tiempoActual%10;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
