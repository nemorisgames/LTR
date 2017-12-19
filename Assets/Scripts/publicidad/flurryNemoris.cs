using UnityEngine;
using System.Collections;
using Prime31;

public class flurryNemoris : MonoBehaviour {
	public string flurryKey = "743CMVG3T7G2YZTYYXTZ";
	public string flurryKeyAndroid = "6R3BSB89C3GXPRXD9538";
	public bool testAds = true;
	public bool fullVersion = false;
	public bool activateAds = true;

	public bool mostrarMensajeGUI = false;
	
	public string[] escenasBanner;
	public string[] escenasInterstitial;

	string lastLoadedLevel;
#if UNITY_IOS
	//public ad bannerOrientacion;
#endif	
#if UNITY_ANDROID
	public FlurryAdPlacement bannerOrientacion;
#endif	
	string mensaje = "";
	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt("fullVersion", fullVersion?1:0);
		PlayerPrefs.SetInt("ejecuciones", PlayerPrefs.GetInt("ejecuciones", 0) + 1);
#if UNITY_ANDROID
		DontDestroyOnLoad(gameObject);
		
		FlurryAndroid.onStartSession( flurryKeyAndroid, activateAds, testAds );
		//FlurryAndroid.fetchAdsForSpace( "splash", FlurryAdPlacement.FullScreen );
		FlurryAndroid.fetchAdsForSpace( "adSpace", bannerOrientacion );
#endif	
		
#if UNITY_IOS
		DontDestroyOnLoad(gameObject);
		
		FlurryAnalytics.startSession( flurryKey );
		
		FlurryAds.enableAds( testAds );
		
		FlurryAnalytics.setSessionReportsOnCloseEnabled(true);
#endif	
		
		mensaje = "sistema inicializado";

	}
	
	void OnLevelWasLoaded(int idEscena){
		//LINEA AGREGADA PARA ESTE JUEGO
		string add = Application.loadedLevelName == "Escena" ? "" + PlayerPrefs.GetInt ("escenaActual", 1) : "";
		//registro de escena ingresada
#if UNITY_IOS
		if(lastLoadedLevel != "") FlurryAnalytics.endTimedEvent(lastLoadedLevel);
		FlurryAnalytics.logEvent( Application.loadedLevelName + add, true );
#endif		
#if UNITY_ANDROID
		if(lastLoadedLevel != "") FlurryAndroid.endTimedEvent(lastLoadedLevel);
		FlurryAndroid.logEvent(Application.loadedLevelName + add, true);
#endif
		lastLoadedLevel = Application.loadedLevelName + add;
		if(!activateAds) return;	
#if UNITY_IOS
		FlurryAds.removeAdFromSpace("adSpace");
		FlurryAds.removeAdFromSpace("splash");
		for(int i = 0; i < escenasBanner.Length; i++){
			if(escenasBanner[i] == Application.loadedLevelName){
				mostrarBanner();
			}
		}
		
		for(int i = 0; i < escenasInterstitial.Length; i++){
			if(escenasInterstitial[i] == Application.loadedLevelName){
				mostrarInterstitial();
			}
		}
#endif	
#if UNITY_ANDROID
		//FlurryAndroid.removeAd("adSpace");
		//FlurryAndroid.removeAd("splash");
		bool encontrado = false;
		for(int i = 0; i < escenasBanner.Length; i++){
			if(escenasBanner[i] == Application.loadedLevelName){
				mostrarBanner();
				encontrado = true;
			}
		}
		if(!encontrado){ 
			FlurryAndroid.removeAd("adSpace");
			FlurryAndroid.fetchAdsForSpace( "adSpace", bannerOrientacion );
		}

		encontrado = false;
		for(int i = 0; i < escenasInterstitial.Length; i++){
			if(escenasInterstitial[i] == Application.loadedLevelName){
				mostrarInterstitial();
				encontrado = true;
			}
		}
		if(!encontrado){ 
			//FlurryAndroid.removeAd("splash");
			//FlurryAndroid.fetchAdsForSpace( "splash", FlurryAdPlacement.FullScreen );
		}
#endif	
		mensaje = "level loaded";

	}
	
	void mostrarInterstitial(){
#if UNITY_IOS
		mensaje = "en splash " + FlurryAds.isAdAvailableForSpace( "splash", FlurryAdSize.Fullscreen );
		FlurryAds.fetchAndDisplayAdForSpace( "splash", FlurryAdSize.Fullscreen );
#endif	
#if UNITY_ANDROID
		/*
		mensaje = "en splash ";
		FlurryAndroid.displayAd( "splash", FlurryAdPlacement.FullScreen, 1000 );
		*/
#endif	
	}
	
	void mostrarBanner(){
#if UNITY_IOS
		//mensaje = "en banner " + FlurryAds.isAdAvailableForSpace( "adSpace", bannerOrientacion );
		//FlurryAds.fetchAndDisplayAdForSpace( "adSpace", bannerOrientacion );
#endif	
#if UNITY_ANDROID
		mensaje = "en banner ";
		FlurryAndroid.displayAd( "adSpace", bannerOrientacion, 1000 );
#endif	
	}
	
	public void OnApplicationQuit()
    {
		//registro de escena ingresada
		#if UNITY_IOS
		FlurryAnalytics.logEvent( Application.loadedLevelName + "_Exit", false );
		#endif		
		#if UNITY_ANDROID
		FlurryAndroid.logEvent(Application.loadedLevelName + "_Exit");
		#endif
		if(!activateAds) return;	
#if UNITY_IOS		
		FlurryAds.removeAdFromSpace( "adSpace" );
		FlurryAds.removeAdFromSpace( "splash" );
#endif	
#if UNITY_ANDROID		
	    FlurryAndroid.removeAd("adSpace");
		//FlurryAndroid.removeAd("splash");
#endif	
    }
	
	// Update is called once per frame
	void OnGUI () {
		if(mostrarMensajeGUI) GUI.Box(new Rect(10, 100, 400, 50), mensaje);
	}
#if UNITY_IOS
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		FlurryManager.spaceDidDismissEvent += spaceDidDismissEvent;
		FlurryManager.spaceWillLeaveApplicationEvent += spaceWillLeaveApplicationEvent;
		FlurryManager.spaceDidFailToRenderEvent += spaceDidFailToRenderEvent;
		FlurryManager.spaceDidReceiveAdEvent += spaceDidReceiveAdEvent;
		FlurryManager.spaceDidFailToReceiveAdEvent += spaceDidFailToReceiveAdEvent;
		//FlurryManager.onCurrencyValueUpdatedEvent += onCurrencyValueUpdatedEvent;
		//FlurryManager.onCurrencyValueFailedToUpdateEvent += onCurrencyValueFailedToUpdateEvent;
		FlurryManager.videoDidFinishEvent += videoDidFinishEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		FlurryManager.spaceDidDismissEvent -= spaceDidDismissEvent;
		FlurryManager.spaceWillLeaveApplicationEvent -= spaceWillLeaveApplicationEvent;
		FlurryManager.spaceDidFailToRenderEvent -= spaceDidFailToRenderEvent;
		FlurryManager.spaceDidReceiveAdEvent -= spaceDidReceiveAdEvent;
		FlurryManager.spaceDidFailToReceiveAdEvent -= spaceDidFailToReceiveAdEvent;
		//FlurryManager.onCurrencyValueUpdatedEvent -= onCurrencyValueUpdatedEvent;
		//FlurryManager.onCurrencyValueFailedToUpdateEvent -= onCurrencyValueFailedToUpdateEvent;
		FlurryManager.videoDidFinishEvent -= videoDidFinishEvent;
	}



	void spaceDidDismissEvent( string space )
	{
		mensaje = ( "spaceDidDismissEvent: " + space );
	}


	void spaceWillLeaveApplicationEvent( string space )
	{
		mensaje = ( "spaceWillLeaveApplicationEvent: " + space );
	}


	void spaceDidFailToRenderEvent( string space )
	{
		mensaje = ( "spaceDidFailToRenderEvent: " + space );
	}


	void spaceDidReceiveAdEvent( string space )
	{
		bool encontrado = false;
		for(int i = 0; i < escenasBanner.Length; i++){
			if(escenasBanner[i] == Application.loadedLevelName){
				encontrado = true;
			}
		}
		
		for(int i = 0; i < escenasInterstitial.Length; i++){
			if(escenasInterstitial[i] == Application.loadedLevelName){
				encontrado = true;
			}
		}
		if(!encontrado){

			FlurryAds.removeAdFromSpace( "adSpace" );
			FlurryAds.removeAdFromSpace( "splash" );

		}
		mensaje = ( "spaceDidReceiveAdEvent: " + space );
	}


	void spaceDidFailToReceiveAdEvent( string space )
	{
		mensaje = ( "spaceDidFailToReceiveAdEvent: " + space );
	}


	/*void onCurrencyValueFailedToUpdateEvent( P31Error error )
	{
		mensaje = ( "onCurrencyValueFailedToUpdateEvent: " + error );
	}


	void onCurrencyValueUpdatedEvent( string currency, float amount )
	{
		mensaje = ( "onCurrencyValueUpdatedEvent. currency: " + currency + ", amount: " + amount );
	}*/


	void videoDidFinishEvent( string space )
	{
		mensaje = ( "videoDidFinishEvent: " + space );

	}
	#endif	
	
	#if UNITY_ANDROID
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		FlurryAndroidManager.adAvailableForSpaceEvent += adAvailableForSpaceEvent;
		FlurryAndroidManager.adNotAvailableForSpaceEvent += adNotAvailableForSpaceEvent;
		FlurryAndroidManager.onAdClosedEvent += onAdClosedEvent;
		FlurryAndroidManager.onApplicationExitEvent += onApplicationExitEvent;
		FlurryAndroidManager.onRenderFailedEvent += onRenderFailedEvent;
		FlurryAndroidManager.spaceDidFailToReceiveAdEvent += spaceDidFailToReceiveAdEvent;
		FlurryAndroidManager.spaceDidReceiveAdEvent += spaceDidReceiveAdEvent;
		FlurryAndroidManager.onAdClickedEvent += onAdClickedEvent;
		FlurryAndroidManager.onAdOpenedEvent += onAdOpenedEvent;
		FlurryAndroidManager.onVideoCompletedEvent += onVideoCompletedEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		FlurryAndroidManager.adAvailableForSpaceEvent -= adAvailableForSpaceEvent;
		FlurryAndroidManager.adNotAvailableForSpaceEvent -= adNotAvailableForSpaceEvent;
		FlurryAndroidManager.onAdClosedEvent -= onAdClosedEvent;
		FlurryAndroidManager.onApplicationExitEvent -= onApplicationExitEvent;
		FlurryAndroidManager.onRenderFailedEvent -= onRenderFailedEvent;
		FlurryAndroidManager.spaceDidFailToReceiveAdEvent -= spaceDidFailToReceiveAdEvent;
		FlurryAndroidManager.spaceDidReceiveAdEvent -= spaceDidReceiveAdEvent;
		FlurryAndroidManager.onAdClickedEvent -= onAdClickedEvent;
		FlurryAndroidManager.onAdOpenedEvent -= onAdOpenedEvent;
		FlurryAndroidManager.onVideoCompletedEvent -= onVideoCompletedEvent;
	}



	void adAvailableForSpaceEvent( string adSpace )
	{
		mensaje = ( "spaceDidReceiveAdEvent: " + adSpace );
	}


	void adNotAvailableForSpaceEvent( string adSpace )
	{
		mensaje = ( "adNotAvailableForSpaceEvent: " + adSpace );
	}


	void onAdClosedEvent( string adSpace )
	{
		mensaje = ( "onAdClosedEvent: " + adSpace );
	}


	void onApplicationExitEvent( string adSpace )
	{
		mensaje = ( "onApplicationExitEvent: " + adSpace );
	}


	void onRenderFailedEvent( string adSpace )
	{
		mensaje = ( "onRenderFailedEvent: " + adSpace );
	}


	void spaceDidFailToReceiveAdEvent( string adSpace )
	{
		mensaje = ( "spaceDidFailToReceiveAdEvent: " + adSpace );
	}


	void spaceDidReceiveAdEvent( string adSpace )
	{
		mensaje = ( "spaceDidReceiveAdEvent: " + adSpace );
	}


	void onAdClickedEvent( string adSpace )
	{
		mensaje = ( "onAdClickedEvent: " + adSpace );
	}


	void onAdOpenedEvent( string adSpace )
	{
		mensaje = ( "onAdOpenedEvent: " + adSpace );
	}


	void onVideoCompletedEvent( string adSpace )
	{
		mensaje = ( "onVideoCompletedEvent: " + adSpace );
	}

#endif	
	
}
