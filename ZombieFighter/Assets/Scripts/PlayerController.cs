using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    private Animator anim;
    private AudioSource audioSrc;
    private float attackTimer = 1.5f;
    private bool dead = false;

    public AudioClip attackClip;
    public AudioClip biteClip;
    public float attackDamage = 30f;
    public float attackSpeed = 1.5f;
    public float maxHealth = 100f;
    public float health = 100f;
	public Slider healthSlider;
	public Slider brainSlider;
	public float humans = 12f;

    public List<GameObject> hittable = new List<GameObject>();

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();	
	}
	
	// Update is called once per frame
	void Update () {
        attackTimer += Time.deltaTime;

        // move forward
        anim.SetFloat("VertSpeed", Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        // Look left and right
        float hor = Input.GetAxis("Horizontal");
        if (hor > 0 || hor < 0)
        {
            transform.Rotate(Vector3.down * Time.deltaTime * hor * -100f);
        }

        // Jump
      /*  if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Jumping", true);
            Invoke("StopJumping", 0.1f);
        }
        */

        // Basic Attack
        if (Input.GetMouseButtonDown(0) && attackTimer >= attackSpeed)
        {
            attackTimer = 0.0f;
            anim.SetLayerWeight(1, 1f);
            anim.SetInteger("CurrentAction", 3);
            
            audioSrc.PlayOneShot(attackClip);
            if (hittable.Count > 0)
            {
                hittable[0].GetComponent<PersonController>().TakeDamage(attackDamage);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetInteger("CurrentAction", 0);
        }

        // Bite Attack
        if (Input.GetMouseButtonDown(1) && hittable.Count > 0 && attackTimer >= attackSpeed )
        {
            if (hittable[0].GetComponent<PersonController>().edible && !dead)
            {
                attackTimer = 0.0f;
                anim.SetInteger("CurrentAction", 1);
                if (anim.GetInteger("CurrentAction") == 1)
                    audioSrc.PlayOneShot(biteClip);

                Vector3 lookPos = new Vector3(hittable[0].transform.position.x, transform.position.y, hittable[0].transform.position.z);
                transform.LookAt(lookPos);

                hittable[0].GetComponent<Animator>().SetBool("Bitten", true);
                hittable[0].GetComponent<PersonController>().Bitten();
                AddHealth(50f);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            anim.SetInteger("CurrentAction", 0);
        }

        // Dance!
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attackable")
        {
            hittable.Add(other.gameObject);
            Debug.Log("add");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Attackable")
        {
            hittable.Remove(other.gameObject);
            Debug.Log("remove");
        }
    }

    private void AddHealth(float amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
		healthSlider.value = health;
        Debug.Log(health);
        if (health <= 0 && !dead)
        {
            // die
            Die();
        }
    }

    private void Die()
    {
        anim.SetTrigger("Die");
        dead = true;
		SceneManager.LoadScene ("LoseScreen",LoadSceneMode.Single);
    }
}
