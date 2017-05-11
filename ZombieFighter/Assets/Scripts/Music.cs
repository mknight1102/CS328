using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
    public AudioSource mainMusic;
    public AudioSource thriller;
    public GameObject player;
    private Animator anim;
	// Use this for initialization
	void Start () {
        anim = player.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (anim.GetInteger("CurrentAction") == 2)
        {
            if (mainMusic.isPlaying && !thriller.isPlaying)
            {
                mainMusic.Stop();
                thriller.time = 38f;
                thriller.Play();
            }
        }

        else if (!mainMusic.isPlaying)
        {
            thriller.Stop();
            mainMusic.Play();
        }
	}
}
