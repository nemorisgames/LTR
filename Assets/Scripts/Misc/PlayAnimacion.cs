using UnityEngine;
using System.Collections;

public class PlayAnimacion : MonoBehaviour {
	public Animation animacion;
	public AnimationClip clip;
	// Use this for initialization
	void Start () {
		animacion.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		animacion.Play(clip.name);
		print (GetComponent<Animation>().isPlaying);
	}
}
