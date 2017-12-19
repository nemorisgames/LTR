using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

#if UNITY_IPHONE
using FB = FacebookBinding;
#endif
#if UNITY_ANDROID
using FB = FacebookAndroid;
#endif

public class FacebookNemoris : MonoBehaviour {
	public UILabel textoDebug;
	public bool showDebugText = true;
	bool logged = false;
	GameObject socialCallbackReceiver;
	facebookCallbackHandler handler;
	public amigo[] amigos;
	
	//almacena el post en caso de no estra loggeado
	string mensajePost = "";
	//deja pendiente el invitar para el caso en que no este logueado
	bool invitando = false;
	
	public UILabel nombreTexto;

	public string[] escenasActivo;
	public GameObject[] elementosFacebook;

	public UIPopupList listaAmigos;

	string idFacebook;
	static string screenshotFilename = "someScreenshot.png";
	public string linkIos;
	public string linkAndroid;

	public UIToggle changeUser;

	//Localization loc;

	#if UNITY_IPHONE || UNITY_ANDROID

	void actionSended( string error, bool canceled)
	{
		if( error != null){
			textoDebug.text = error;
			if(handler != null) handler.postError(error);
		}
		else{
			if(canceled) textoDebug.text = "Post cancelado";
			else textoDebug.text = "Accion OK";
			if(handler != null) handler.actionSended(canceled);
		}
	}

	void logueado(string error, FacebookMeResult result)
	{
		if (error != null){
			textoDebug.text = error;
			if(handler != null) handler.errorLogin(error);
		}
		else{
			//IDictionary me = (IDictionary)result;          
			#if UNITY_IPHONE
			FlurryAnalytics.logEvent("facebookUserLog", false );
			#endif		
			#if UNITY_ANDROID
			FlurryAndroid.logEvent("facebookUserLog");
			#endif
			textoDebug.text = "Hola\n" + result.first_name;
			nombreTexto.text = "" + result.first_name;
			idFacebook = result.id;
			Debug.Log(result.name + " " + result.id);
			if(handler != null) handler.logged(result.toDictionary());
			logged = true;
			//StartCoroutine("usuarioAddBD", result.id);

			//para el caso de que quiera publicar y no se habia loggeado
			if(mensajePost != "") publicarEnMuro(mensajePost);
			//carga los amigos
			//getFriends();
			
			//StartCoroutine("actualizarAmigosBD");

			#if UNITY_IPHONE
			FlurryAnalytics.logEvent ("FacebookLogin", false);
			#endif
			#if UNITY_ANDROID
			FlurryAndroid.logEvent ("FacebookLogin", false);
			#endif

			if(invitando){
				inviteFriends();
			}
		}
	}

	IEnumerator usuarioAddBD(string id){

		WWWForm form = new WWWForm();
		form.AddField( "campo", "idFacebook" );
		form.AddField( "playerPref", id );
		form.AddField( "pk_usuario", SystemInfo.deviceUniqueIdentifier );
		Debug.Log ("playerPref " + id + ", pk_usuario = " + SystemInfo.deviceUniqueIdentifier);
		WWW download = new WWW( "http://www.nemorisgames.com/medusa/funciones.php?operacion=1", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
            //mostrarError("Error de conexion");
            yield return null;
        } else {
			string retorno = download.text;
			if(retorno == "-1"){
				//error :(
				//mostrarError("Error de conexion");
				print ("error conexion");
			}
			else{
				//exito!
				print("activado");
				//Application.LoadLevel(Application.loadedLevelName);
			}
		}	
	}

	IEnumerator actualizarAmigosBD(){
		foreach (amigo a in amigos) {
			WWWForm form = new WWWForm();
			form.AddField( "idFacebook", idFacebook );
			form.AddField( "idFacebookAmigo", a.idFacebookAmigo );
			WWW download = new WWW( "http://www.nemorisgames.com/medusa/funciones.php?operacion=3", form);
			yield return download;
			if(download.error != null) {
				print( "Error downloading: " + download.error );
                //mostrarError("Error de conexion");
                yield return null;
            } else {
				if(download.text == "1"){
					a.noActivo = false;
				}
				else{
					a.noActivo = true;
				}
			}
		}
	}
	
	void getFriends(){
		Facebook.instance.getFriends(mostrarAmigos);
	}

	public void publicarFoto(){
		//no funciona
		Application.CaptureScreenshot( screenshotFilename );
		var pathToImage = Application.persistentDataPath + "/" + screenshotFilename;//FacebookComboUI.screenshotFilename;
		if( !System.IO.File.Exists( pathToImage ) )
		{
			Debug.LogError( "there is no screenshot avaialable at path: " + pathToImage );
			return;
		}
		
		var bytes = System.IO.File.ReadAllBytes( pathToImage );
		Facebook.instance.postImage( bytes, "Check out my Barbershot at MonsterCut!", fotoPublicada );
	}

	void fotoPublicada(string error, object result){
		if (error != "") {
			Debug.Log(error);
		}
		else print ("publicacion ok");
	}

	void mostrarAmigos(string error, FacebookFriendsResult result){
		//IDictionary data = result as IDictionary;
		//IList friends = data["data"] as IList;
		textoDebug.text = "mostrando amigos...";
		amigos = null;
		amigos = new amigo[result.data.Count];
		Debug.Log("namigos " + result.data.Count);
		for( int i = 0; i < result.data.Count; i++){
			textoDebug.text += (i + " " + result.data[i].name + " " + result.data[i].id + ", ");
			Debug.Log (i + " " + result.data[i].name + " " + result.data[i].id);
			crearAmigo(i, (string)result.data[i].id, (string)result.data[i].name);
		}
		StartCoroutine("buscarAmigosActivos");

	}

	IEnumerator buscarAmigosActivos(){
		foreach (amigo a in amigos) {
			yield return StartCoroutine("obtenerAmigoActivo", a);
		}
		actualizarListaAmigos();
	}

	IEnumerator obtenerAmigoActivo(amigo a){
		WWWForm form = new WWWForm();
		form.AddField( "idFacebook", idFacebook );
		form.AddField( "idFacebookAmigo", a.idFacebookAmigo );
		WWW download = new WWW( "http://www.nemorisgames.com/medusa/funciones.php?operacion=3", form);
		yield return download;
		if(download.error != null) {
			print( "Error downloading: " + download.error );
            //mostrarError("Error de conexion");
            yield return null;
        } else {
			string retorno = download.text;
			if(retorno == "0"){
				//error :(
				//mostrarError("Error de conexion");
				a.noActivo = true;
				print ("amigo " + a.nombre + " no activo");
			}
			else{
				//exito!
				//prefAux = retorno;
				a.noActivo = false;
				print ("amigo " + a.nombre + " activo");
				//Application.LoadLevel(Application.loadedLevelName);
			}
		}
	}

	/*int contarAmigos(IList friends){
		int contador = 0;
		foreach(IDictionary f in friends) contador++;
		return contador;
	}*/
	void crearAmigo(int i, string id, string nombre){
		amigos [i] = new amigo ();
		amigos[i].nombre = nombre;
		string[] nombres = nombre.Split(' ');
		amigos[i].primerNombre = nombres[0];
		print("amigo " + amigos [i].primerNombre);
		amigos[i].idFacebookAmigo = id;
	}

	void publicarEnMuro(string mensaje){
		mensajePost = "";
		if(handler != null) handler.posting();
		if(!logged){ 
			login ();
			mensajePost = mensaje;
		}
		else{ 
			//Facebook.instance.postMessage( mensaje, actionSended);
			//var pathToImage = Application.persistentDataPath + "/Assets/" + "gameImage.jpg";
			//var bytes = System.IO.File.ReadAllBytes( pathToImage );
			//gameImage = Resources.Load("imagen", byte[]);
			//Facebook.instance.postImage( bytes , mensaje, actionSended );
			
			// parameters are optional. See Facebook's documentation for all the dialogs and paramters that they support https://developers.facebook.com/docs/reference/dialogs/feed/
			var parameters = new Dictionary<string,string>
			{
				#if UNITY_ANDROID
				{ "link", linkAndroid },
				#endif
				#if UNITY_IPHONE
				{ "link", linkIos },
				#endif
				{ "name", Localization.Get("I'm playing Little Totem Raider") },
				{ "picture", "http://nemorisgames.com/juegos/plataformasar/iconoLTRv3.png" },
				{ "caption", Localization.Get("Compete with people around the world!") },
				{ "description", mensaje }
			};
			FB.showDialog("stream.publish", parameters);
			#if UNITY_IPHONE
			FlurryAnalytics.logEvent("FacebookPublicar", false );
			#endif		
			#if UNITY_ANDROID
			FlurryAndroid.logEvent("FacebookPublicar");
			#endif
		}
	}
	
	void inviteFriends()
	{
		invitando = false;
		if(!logged){
			invitando = true;
			login ();
		}
		else{ 
			Dictionary<string, string> lParam = new Dictionary<string, string>();
			lParam["message"] = "Let's compete in this 3d platformer with people around the world and beat their records!";
			lParam["title"] = "Join me and let's play Little Totem Raider";
			#if UNITY_ANDROID
			FacebookAndroid.showDialog("apprequests", lParam);
			#endif
			#if UNITY_IPHONE
			FacebookBinding.showDialog("apprequests", lParam);
			#endif
			#if UNITY_IPHONE
			FlurryAnalytics.logEvent("FacebookInvitarAmigo", false );
			#endif		
			#if UNITY_ANDROID
			FlurryAndroid.logEvent("FacebookInvitarAmigo");
			#endif
		}
	}
	
	void actualizarListaAmigos(){
		print ("actualizando");
		listaAmigos.items.Clear();
		
		listaAmigos.items.Add(Localization.Get("- Visit your friends -"));
		int nAmigosActivos = 0;
		foreach (amigo a in amigos)
		{
			print ("revisando " + a.nombre + " " + a.noActivo);
			if(!a.noActivo){ 
				listaAmigos.items.Add(a.nombre);
				nAmigosActivos++;
			}
		}
		if (nAmigosActivos <= 0) {
			listaAmigos.items[0] += " " + Localization.Get("(no friends)");
		}
	}
	
	void visitarAmigo(){
		//getFriends ();
		foreach (amigo a in amigos)
		{
			if(a.nombre == listaAmigos.selection){
				Debug.Log( "visitando " + a.idFacebookAmigo + " " + a.nombre);
				#if UNITY_IPHONE
				FlurryAnalytics.logEvent("facebookVisitarAmigo", false );
				#endif		
				#if UNITY_ANDROID
				FlurryAndroid.logEvent("facebookVisitarAmigo");
				#endif
				PlayerPrefs.SetString("idFacebookAmigo", a.idFacebookAmigo);
				PlayerPrefs.SetString("nombreAmigo", a.primerNombre);
				Application.LoadLevel("Peluqueria");
			}
		}
	}

	void login(){
		/*
		#if UNITY_ANDROID
		FacebookAndroid.loginWithReadPermissions( new string[] { "email", "user_birthday" } );
		#endif
		#if UNITY_IPHONE
		FacebookBinding.loginWithReadPermissions( new string[] { "email", "user_birthday" } );
		#endif
		*/
		#if UNITY_ANDROID
		//if(changeUser.isChecked)
		//	FB.setSessionLoginBehavior (FacebookSessionLoginBehavior.SUPPRESS_SSO);
		//else
		//FB.setSessionLoginBehavior (FacebookSessionLoginBehavior.SSO_ONLY);
		#endif
		#if UNITY_IPHONE
		if(changeUser.isChecked)
			FB.setSessionLoginBehavior (FacebookSessionLoginBehavior.WithNoFallbackToWebView);
		else
			FB.setSessionLoginBehavior (FacebookSessionLoginBehavior.ForcingWebView);
		#endif
		//FacebookCombo.loginWithReadPermissions( new string[] { "publish_actions", "user_friends" });
		var permissions = new string[] { "user_friends" };
		FacebookCombo.loginWithReadPermissions( permissions );
		if(handler != null) handler.logueando();
		textoDebug.text = "Logging..." + FacebookCombo.isSessionValid();
	}
	
	void logout(){
		#if UNITY_ANDROID
		FacebookAndroid.logout();
		#endif
		#if UNITY_IPHONE
		FacebookBinding.logout();
		#endif
		logged = false;
		if(handler != null) handler.logout();
		textoDebug.text = "Logged out";
		nombreTexto.text = Localization.Get("Not Connected");
	}


	void Start () {
		DontDestroyOnLoad(gameObject);

		//loc = Localization.instance;
		
		if(!showDebugText) textoDebug.gameObject.SetActive(false);

		//FacebookCombo.init();
		FacebookCombo.init();

		FacebookManager.graphRequestCompletedEvent += result =>
		{
			Prime31.Utils.logObject( result );
		};
	}
	
	void activar(bool b){
		foreach(GameObject g in elementosFacebook){
			g.SetActive(b);	
		}
		//CASO ESPECIAL!!
		foreach(GameObject g in elementosFacebook){
			if(g.name == "ListaAmigos"){ 
				if (Application.loadedLevelName == "Mapa") g.SetActive(true);
				else g.SetActive(false);
			}
		}
		
	}
	
	void OnLevelWasLoaded(int level){
		socialCallbackReceiver = null;
		handler = null;
		socialCallbackReceiver = GameObject.FindWithTag("socialCallbackReceiver");
		if(socialCallbackReceiver != null) handler = socialCallbackReceiver.GetComponent<facebookCallbackHandler>();
		bool encontrado = false;
		foreach(string i in escenasActivo){
			if(Application.loadedLevelName == i) encontrado = true;
		}
		activar(encontrado);

		if (Application.loadedLevelName != "Mapa") {
			GameObject c = GameObject.FindWithTag ("camaraRedesSociales");
			GameObject p = GameObject.FindWithTag ("panelRedesSociales");
			Camera camaraRedesSociales = c.GetComponent<Camera>();
			Transform panelRedesSociales = p.transform;
			if (camaraRedesSociales != null)
				camaraRedesSociales.orthographicSize = 1f;
			if (panelRedesSociales != null)
				panelRedesSociales.localScale = Vector3.one * camaraRedesSociales.orthographicSize; 
		}
		//caso especial
		if(Application.loadedLevelName != "Peluqueria") PlayerPrefs.SetString("idFacebookAmigo", "-1");
	}

/*	
	public static string screenshotFilename = "someScreenshot.png";
	
	
	// common event handler used for all Facebook graph requests that logs the data to the console
	void completionHandler( string error, object result )
	{
		if( error != null )
			textoDebug.text = ( error );
		else
			textoDebug.text = ( result.ToString() );
	}
	

	void Start()
	{
		// grab a screenshot for later use
		Application.CaptureScreenshot( screenshotFilename );
		
		// optionally enable logging of all requests that go through the Facebook class
		//Facebook.instance.debugRequests = true;
	}
	

	void OnGUI()
	{

#if UNITY_ANDROID
		
		if( GUILayout.Button( "Initialize Facebook" ) )
		{
			FacebookAndroid.init();
		}
		

		if( GUILayout.Button( "Set Login Behavior to SUPPRESS_SSO" ) )
		{
			FacebookAndroid.setSessionLoginBehavior( FacebookSessionLoginBehavior.SUPPRESS_SSO );
		}
		
		
		if( GUILayout.Button( "Login" ) )
		{
			FacebookAndroid.loginWithReadPermissions( new string[] { "email", "user_birthday" } );
		}
		
		
		if( GUILayout.Button( "Reauthorize with Publish Permissions" ) )
		{
			FacebookAndroid.reauthorizeWithPublishPermissions( new string[] { "publish_actions", "manage_friendlists" }, FacebookSessionDefaultAudience.Everyone );
		}
		
		
		if( GUILayout.Button( "Logout" ) )
		{
			FacebookAndroid.logout();
		}
		
		
		if( GUILayout.Button( "Is Session Valid?" ) )
		{
			var isSessionValid = FacebookAndroid.isSessionValid();
			textoDebug.text = ( "Is session valid?: " + isSessionValid );
		}
		
		
		if( GUILayout.Button( "Get Session Token" ) )
		{
			var token = FacebookAndroid.getAccessToken();
			textoDebug.text = ( "session token: " + token );
		}
		
		
		if( GUILayout.Button( "Get Granted Permissions" ) )
		{
			var permissions = FacebookAndroid.getSessionPermissions();
			textoDebug.text = ( "granted permissions: " + permissions.Count );
			Prime31.Utils.logObject( permissions );
		}

		
		
		if( GUILayout.Button( "Post Image" ) )
		{
			var pathToImage = Application.persistentDataPath + "/" + screenshotFilename;
			var bytes = System.IO.File.ReadAllBytes( pathToImage );
			
			Facebook.instance.postImage( bytes, "im an image posted from Android", completionHandler );
		}
		
		
		if( GUILayout.Button( "Graph Request (me)" ) )
		{
			//Facebook.instance.graphRequest( "me", mostrarInfo );
			Facebook.instance.graphRequest("me?fields=id", mostrarInfo);
		}
		
		
		if( GUILayout.Button( "Post Message" ) )
		{
			Facebook.instance.postMessage( "im posting this from Unity: " + Time.deltaTime, completionHandler );
		}
		
		
		if( GUILayout.Button( "Post Message & Extras" ) )
		{
			Facebook.instance.postMessageWithLinkAndLinkToImage( "link post from Unity: " + Time.deltaTime, "http://prime31.com", "prime[31]", "http://prime31.com/assets/images/prime31logo.png", "Prime31 Logo", completionHandler );
		}
		
		
		if( GUILayout.Button( "Show Share Dialog" ) )
		{
			var parameters = new Dictionary<string,object>
			{
				{ "link", "http://prime31.com" },
				{ "name", "link name goes here" },
				{ "picture", "http://prime31.com/assets/images/prime31logo.png" },
				{ "caption", "the caption for the image is here" },
				{ "description", "description of what this share dialog is all about" }
			};
			FacebookAndroid.showFacebookShareDialog( parameters );
		}
		
		
		if( GUILayout.Button( "Show Post Dialog" ) )
		{
			// parameters are optional. See Facebook's documentation for all the dialogs and paramters that they support
			var parameters = new Dictionary<string,string>
			{
				{ "link", "http://prime31.com" },
				{ "name", "link name goes here" },
				{ "picture", "http://prime31.com/assets/images/prime31logo.png" },
				{ "caption", "the caption for the image is here" }
			};
			FacebookAndroid.showDialog( "stream.publish", parameters );
		}
		
		
		if( GUILayout.Button( "Show Apprequests Dialog" ) )
		{
			// See Facebook's documentation for all the dialogs and paramters that they support
			var parameters = new Dictionary<string,string>
			{
				{ "message", "Come play my awesome game!" }
			};
			FacebookAndroid.showDialog( "apprequests", parameters );
		}
		
		
		if( GUILayout.Button( "Get Friends" ) )
		{
			textoDebug.text = ( "get friends..." );
			Facebook.instance.getFriends( mostrarAmigos2 );
		}
#endif

	}
*/
	//#if UNITY_IPHONE || UNITY_ANDROID
	// Listens to all the events.  All event listeners MUST be removed before this object is disposed!
	void OnEnable()
	{
		FacebookManager.sessionOpenedEvent += sessionOpenedEvent;
		FacebookManager.loginFailedEvent += loginFailedEvent;

		FacebookManager.dialogCompletedWithUrlEvent += dialogCompletedEvent;
		FacebookManager.dialogFailedEvent += dialogFailedEvent;

		FacebookManager.graphRequestCompletedEvent += graphRequestCompletedEvent;
		FacebookManager.graphRequestFailedEvent += facebookCustomRequestFailed;

		FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;
		
		FacebookManager.reauthorizationFailedEvent += reauthorizationFailedEvent;
		FacebookManager.reauthorizationSucceededEvent += reauthorizationSucceededEvent;
		
		FacebookManager.shareDialogFailedEvent += shareDialogFailedEvent;
		FacebookManager.shareDialogSucceededEvent += shareDialogSucceededEvent;

		//FacebookManager.restRequestCompletedEvent += restRequestCompletedEvent;
		//FacebookManager.restRequestFailedEvent += restRequestFailedEvent;
		//FacebookManager.facebookComposerCompletedEvent += facebookComposerCompletedEvent;

		//FacebookManager.reauthorizationFailedEvent += reauthorizationFailedEvent;
		//FacebookManager.reauthorizationSucceededEvent += reauthorizationSucceededEvent;
	}


	void OnDisable()
	{
		// Remove all the event handlers when disabled
		FacebookManager.sessionOpenedEvent -= sessionOpenedEvent;
		FacebookManager.loginFailedEvent -= loginFailedEvent;

		FacebookManager.dialogCompletedWithUrlEvent -= dialogCompletedEvent;
		FacebookManager.dialogFailedEvent -= dialogFailedEvent;

		FacebookManager.graphRequestCompletedEvent -= graphRequestCompletedEvent;
		FacebookManager.graphRequestFailedEvent -= facebookCustomRequestFailed;

		FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;
		
		FacebookManager.reauthorizationFailedEvent -= reauthorizationFailedEvent;
		FacebookManager.reauthorizationSucceededEvent -= reauthorizationSucceededEvent;
		
		FacebookManager.shareDialogFailedEvent -= shareDialogFailedEvent;
		FacebookManager.shareDialogSucceededEvent -= shareDialogSucceededEvent;

		//FacebookManager.restRequestCompletedEvent -= restRequestCompletedEvent;
		//FacebookManager.restRequestFailedEvent -= restRequestFailedEvent;
		//FacebookManager.facebookComposerCompletedEvent -= facebookComposerCompletedEvent;

		//FacebookManager.reauthorizationFailedEvent -= reauthorizationFailedEvent;
		//FacebookManager.reauthorizationSucceededEvent -= reauthorizationSucceededEvent;
	}
	//#endif
	//lanzado cuando se loguea a FB
	void sessionOpenedEvent()
	{
		textoDebug.text = ( "Successfully logged in to Facebook" );
		//Facebook.instance.graphRequest("me", logueado); 
		Facebook.instance.getMe(logueado);
	}


	void loginFailedEvent( P31Error error )
	{
		textoDebug.text = ( "Facebook login failed: " + error );
	}


	void dialogCompletedEvent( string url )
	{
		actionSended(null, url=="");
		textoDebug.text = ( "dialogCompletedEvent: " + url );
	}


	void dialogFailedEvent( P31Error error )
	{
		actionSended(error.ToString(), false);
		textoDebug.text = ( "dialogFailedEvent: " + error );
	}


	void facebokDialogCompleted()
	{
		actionSended(null, false);
		textoDebug.text = ( "facebokDialogCompleted" );
	}


	void graphRequestCompletedEvent( object obj )
	{
		textoDebug.text = ( "graphRequestCompletedEvent" );
		Prime31.Utils.logObject( obj );
	}

	void facebookCustomRequestFailed( P31Error error )
	{
		textoDebug.text = ( "facebookCustomRequestFailed failed: " + error );
	}
	
	
	void facebookComposerCompletedEvent( bool didSucceed )
	{
		textoDebug.text = ( "facebookComposerCompletedEvent did succeed: " + didSucceed );
	}
	
	
	void reauthorizationSucceededEvent()
	{
		textoDebug.text = ( "reauthorizationSucceededEvent" );
	}
	
	
	void reauthorizationFailedEvent( P31Error error )
	{
		textoDebug.text = ( "reauthorizationFailedEvent: " + error );
	}
	
	
	void shareDialogFailedEvent( P31Error error )
	{
		textoDebug.text = ( "shareDialogFailedEvent: " + error );
	}
	
	
	void shareDialogSucceededEvent( Dictionary<string,object> dict )
	{
		textoDebug.text = ( "shareDialogSucceededEvent" );
		Prime31.Utils.logObject( dict );
	}

	/*void facebookCustomRequestFailed( P31Error error )
	{
		textoDebug.text = ( "facebookCustomRequestFailed failed: " + error );
	}


	void restRequestCompletedEvent( object obj )
	{
		textoDebug.text = ( "restRequestCompletedEvent" );
		Prime31.Utils.logObject( obj );
	}


	void restRequestFailedEvent( P31Error error )
	{
		textoDebug.text = ( "restRequestFailedEvent failed: " + error );
	}


	void facebookComposerCompletedEvent( bool didSucceed )
	{
		textoDebug.text = ( "facebookComposerCompletedEvent did succeed: " + didSucceed );
	}


	void reauthorizationSucceededEvent()
	{
		textoDebug.text = ( "reauthorizationSucceededEvent" );
	}


	void reauthorizationFailedEvent( string error )
	{
		textoDebug.text = ( "reauthorizationFailedEvent: " + error );
	}*/
#endif
}

[System.Serializable]
public class amigo{
	public string idFacebookAmigo;
	public string nombre;
	public string primerNombre;
	public bool noActivo;
}
