using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class panelTutorial : MonoBehaviour {
	public GameObject[] botonesEscondidos;
	UILabel textoLabel;
	string mensaje = "";
	string mensajeActual = "";
	int indice = 0;
	public bool pausar = false;
	bool activado = false;
	float ciclo = 0f;
	float intervaloTiempo;
	public bool esperaEvento;
	public bool eventoInmediato = false;
	public AudioClip sonidoLetra;
	
	// Use this for initialization
	IEnumerator Start () {
		intervaloTiempo = Time.fixedDeltaTime;
		//mostrarBotones(false);
		Transform t = transform.Find("Label");
		textoLabel = t.gameObject.GetComponent<UILabel>();
		yield return new WaitForSeconds (0.01f);
		mensaje = textoLabel.text;
		textoLabel.text = "";
	}

	public void posicionado(){
		print ("posicionado");
		activado = true;
	}

	public void terminarMensaje(){
		indice = mensaje.Length;
		mensajeActual = mensaje;
		textoLabel.text = mensajeActual;
	}

	public bool mensajeTerminado(){
		return mensaje == mensajeActual;
	}

	public void esconderBotones(){
		foreach(GameObject g in botonesEscondidos) g.SetActive(false);
	}
	public void verBotones(){
		foreach(GameObject g in botonesEscondidos) g.SetActive(false);
	}

	public void mostrarBotones(bool b){
		foreach(GameObject g in botonesEscondidos) g.SetActive(!g.activeSelf);
	}

	void Update(){
		if(indice >= mensaje.Length || !activado) return;
		ciclo += (Time.timeScale==1f?Time.deltaTime:intervaloTiempo) * 0.6f;
		if(ciclo > 0.01f){
			ciclo = 0f;
			indice += 1;
			if(indice < mensaje.Length - 1 && mensaje.Substring((int)indice - 1, 1) == "["){
				if(mensaje.Substring((int)indice, 1) == "-"){
					indice += 3;
				}
				else{
					indice += 8;
				}
			}
			if(indice > mensaje.Length) indice = mensaje.Length;
			mensajeActual = mensaje.Substring(0, (int)indice);
			textoLabel.text = mensajeActual;
			GetComponent<AudioSource>().PlayOneShot(sonidoLetra);
		}
	}
	
	// Update is called once per frame
	/*void FixedUpdate () {
		if(indice > mensaje.Length || !activado) return;
		ciclo += Time.timeScale==1f?Time.fixedDeltaTime:intervaloTiempo;
		if(ciclo > 0.02f){
			ciclo = 0f;
			indice += 1;
			if(indice < mensaje.Length - 1 && mensaje.Substring((int)indice - 1, 1) == "["){
				if(mensaje.Substring((int)indice, 1) == "-"){
					indice += 3;
				}
				else{
					indice += 8;
				}
			}
			if(indice > mensaje.Length) indice = mensaje.Length;
			mensajeActual = mensaje.Substring(0, (int)indice);
			textoLabel.text = mensajeActual;
		}	
	}*/
}
