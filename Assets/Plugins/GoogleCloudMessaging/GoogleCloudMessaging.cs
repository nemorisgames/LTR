using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



#if UNITY_ANDROID
public class GoogleCloudMessaging
{
	private static AndroidJavaObject _plugin;

	static GoogleCloudMessaging()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		// find the plugin instance
		using( var pluginClass = new AndroidJavaClass( "com.prime31.GoogleCloudMessagingPlugin" ) )
			_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
	}
	
	
	// Call this at application launch so the plugin can check for any received notifications via the Intent extras
	// used to launch the application. Note that calling it more than once will result in duplication notificationReceivedEvents
	public static void checkForNotifications()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "checkForNotifications" );
	}


	// Registers the device for push notifications. gcmSenderId is the sender ID as detailed in Google's setup guide (see docs for link)
	public static void register( string gcmSenderId )
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "register", gcmSenderId );
	}


	// Unregisters a device for push notification support
	public static void unRegister()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "unRegister" );
	}


	// Cancels all notfications and removes them from the system tray
	public static void cancelAll()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;

		_plugin.Call( "cancelAll" );
	}
	
	
	// Registers with Push.io
	public static IEnumerator registerDeviceWithPushIO( string deviceId, string pushIOApiKey, List<string> pushIOCategories, Action<bool,string> completionHandler )
	{
		var url = string.Format( "https://api.push.io/r/{0}?di={1}&dt={2}", pushIOApiKey, SystemInfo.deviceUniqueIdentifier, deviceId );
		
		// add categories if we have them
		if( pushIOCategories != null && pushIOCategories.Count > 0 )
			url += "&c=" + string.Join( ",", pushIOCategories.ToArray() );
		
		using( WWW www = new WWW( url ) )
		{
			yield return www;
			
			if( completionHandler != null )
			{
				if( www.text.StartsWith( "ok" ) )
					completionHandler( true, null );
				else
					completionHandler( false, www.error );
			}
		}
	}

}
#endif
