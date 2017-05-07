using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Animator anim;
    private AudioSource audioSrc;

    public AudioClip attackClip;
    public AudioClip biteClip;

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();	
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetFloat("VertSpeed", Input.GetAxis("Vertical"));

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        float hor = Input.GetAxis("Horizontal");
        if (hor > 0 || hor < 0)
        {
            transform.Rotate(Vector3.down * Time.deltaTime * hor * -100f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Jumping", true);
            Invoke("StopJumping", 0.1f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetLayerWeight(1, 1f);
            anim.SetInteger("CurrentAction", 3);
            if (anim.GetInteger("CurrentAction") == 3)
                audioSrc.PlayOneShot(attackClip);

        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetInteger("CurrentAction", 0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            anim.SetInteger("CurrentAction", 1);
            if (anim.GetInteger("CurrentAction") == 1)
                audioSrc.PlayOneShot(biteClip);
        }
        if (Input.GetMouseButtonUp(1))
        {
            anim.SetInteger("CurrentAction", 0);
        }

        if (Input.GetKeyDown("1"))
        {
            if (anim.GetInteger("CurrentAction") == 0)
            {
                anim.SetInteger("CurrentAction", 2);
            } else if (anim.GetInteger("CurrentAction") == 2)
            {
                anim.SetInteger("CurrentAction", 0);
            }
        }
	}

    void StopJumping()
    {
        anim.SetBool("Jumping", false);
    }
}
