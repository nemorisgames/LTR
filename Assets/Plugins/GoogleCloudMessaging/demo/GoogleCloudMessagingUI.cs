using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Prime31;
using System.Text;
using System.Security.Cryptography;


public class GoogleCloudMessagingUI : Prime31.MonoBehaviourGUI
{
#if UNITY_ANDROID
	private string _registrationId;


	void Start()
	{
		// listen for successful registration so we can save off the registrationId in case we want to use it for Push.io registration.
		GoogleCloudMessagingManager.registrationSucceededEvent += regId =>
		{
			_registrationId = regId;
		};
	}


	void OnGUI()
	{
		beginColumn();

		if( GUILayout.Button( "Check for Notifications" ) )
		{
			GoogleCloudMessaging.checkForNotifications();
		}


		if( GUILayout.Button( "Register" ) )
		{
			// replace this with your sender ID!!!
			GoogleCloudMessaging.register( "589644589374" );
		}


		if( GUILayout.Button( "Unregister" ) )
		{
			GoogleCloudMessaging.unRegister();
		}


		if( GUILayout.Button( "Cancel All Pending Notifications" ) )
		{
			GoogleCloudMessaging.cancelAll();
		}


		if( GUILayout.Button( "Register with Push.io" ) )
		{
			if( _registrationId == null )
			{
				Debug.LogError( "registrationId is null. Please register for push before attempting to register with Push.io" );
				return;
			}

			// replace with your Push.io key
			var pushIOApiKey = "vVayHFVNUw_gvV0";
			StartCoroutine( GoogleCloudMessaging.registerDeviceWithPushIO( _registrationId, pushIOApiKey, null, ( didSucceed, error ) =>
			{
				if( didSucceed )
					Debug.Log( "Push.io registration successful" );
				else
					Debug.Log( "Push.io registration failed: " + error );
			}) );
		}

		endColumn( false );
	}
#endif
}
