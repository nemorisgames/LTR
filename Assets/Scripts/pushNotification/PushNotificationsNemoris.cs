using UnityEngine;
using System.Collections;

public class PushNotificationsNemoris : MonoBehaviour {

	public string ProjectID;
	public string APIkey;
	string _registrationId;
#if UNITY_ANDROID


	void Start(){
		GoogleCloudMessaging.cancelAll();
		GoogleCloudMessaging.checkForNotifications();
		GoogleCloudMessagingManager.registrationSucceededEvent += regId =>
		{
			_registrationId = regId;
			StartCoroutine (registrarUsuario());
		};
		GoogleCloudMessaging.register( ProjectID );
	}

	IEnumerator registrarUsuario(){
		WWWForm form = new WWWForm();
		form.AddField( "param0", _registrationId );
		WWW download = new WWW( "http://www.nemorisgames.com/juegos/plataformasar/funciones.php?operacion=7", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
			//mostrarError("Error de conexion");
			yield return null;
		} else {
			string retorno = download.text;
			if(retorno == ""){
				//error :(
				//mostrarError("Error de conexion");
				print("error conexion");
			}
			else{
				//exito!
				print ( retorno );
				//Application.LoadLevel(Application.loadedLevelName);
			}
		}	
	}
	#endif
}
