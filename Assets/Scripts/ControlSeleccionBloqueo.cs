using UnityEngine;
using System.Collections;

public class ControlSeleccionBloqueo : MonoBehaviour {
	public int estrellasNecesarias;
	int estrellasActuales;
	[HideInInspector]
	public bool bloqueado = true;
	UIProgressBar barra;
	UILabel estrellasLabel;
	UILabel estrellasActualesLabel;
	// Use this for initialization
	void Start () {
		barra = transform.FindChild ("barra").gameObject.GetComponent<UIProgressBar> ();
		estrellasLabel = transform.FindChild ("Label").gameObject.GetComponent<UILabel> ();
		estrellasActualesLabel = transform.FindChild ("barra/Label").gameObject.GetComponent<UILabel> ();
	}

	public void setInformacion(int estrellas, int estrellasN){
		if ((estrellas >= estrellasN && estrellasN > 0) || (estrellas >= estrellasNecesarias && estrellasN <= 0)) {
			bloqueado = false;
			if(barra == null) barra = transform.FindChild("barra").gameObject.GetComponent<UIProgressBar>();
            if (estrellasLabel == null) estrellasLabel = transform.FindChild("Label").gameObject.GetComponent<UILabel>();
            if (estrellasActualesLabel == null) estrellasActualesLabel = transform.FindChild("barra/Label").gameObject.GetComponent<UILabel>();
            barra.gameObject.SetActive(false);
			estrellasLabel.gameObject.SetActive(false);
			gameObject.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.6f);
		} else {
			estrellasActuales = estrellas;
			if (estrellasN > 0)
					estrellasNecesarias = estrellasN;
            if (barra == null) barra = transform.FindChild("barra").gameObject.GetComponent<UIProgressBar>();
            if (estrellasLabel == null) estrellasLabel = transform.FindChild("Label").gameObject.GetComponent<UILabel>();
            if (estrellasActualesLabel == null) estrellasActualesLabel = transform.FindChild("barra/Label").gameObject.GetComponent<UILabel>();
            barra.value = ((float)estrellasActuales / (float)estrellasNecesarias);
			estrellasLabel.text = "" + estrellasNecesarias;
			estrellasActualesLabel.text = "" + estrellasActuales;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
