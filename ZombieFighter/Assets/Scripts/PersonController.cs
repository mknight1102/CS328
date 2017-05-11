using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class PersonController : MonoBehaviour {
    private Renderer rend;
    public float health = 100;
    public bool edible = false;
    public AudioClip deathYell;
    public AudioClip biteYell;
    public AudioClip[] boneCrunching;
    public AudioClip blood;
    public AudioClip attackSFX;
    public Transform bloodLoc;
    public GameObject bloodSpray;
    public float attackSpeed = 2f;
    public float attackDamage = 15;
    public Transform trail;
    public DefenseMode attackMode = DefenseMode.FISTS;
    private float maxHealth = 100;
    private Animator anim;
    private bool dead = false;
    private AudioSource audioSource;
    private NavMeshAgent agent;
    private GameObject player;
    private float attackTimer = 2f;
    private float stateTimer = 0;
    private float newStateTime = 10f;
    private Vector3 fleeLocation;
    private float correctFleeTimer = 0f;
    private float correctFleeTimerCap = 1.5f;
    private bool correctedFlee = false;
    private bool hidden = false;
    private PersonState state = PersonState.HIDE;
    private GameObject[] hidingPlaces;
    private Vector3 hideLocation;
    private bool searching = false;
    private bool hiding = false;

    private float trailTimer = 0f;
    private float dropTrail = .5f;
    private GameObject gameController;

    // Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        hidingPlaces = GameObject.FindGameObjectsWithTag("HidingPlace");
        gameController = GameObject.FindGameObjectWithTag("Controller");
        if (attackMode == DefenseMode.WEAPON)
        {
            anim.SetBool("HasWeapon", true);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!dead)
        {
            switch (state)
            {
                case PersonState.FLEE:
                    FleeMode();
                    break;
                case PersonState.HIDE:
                    HideMode();
                    break;
                case PersonState.FIGHT:
                    FightMode();
                    break;
            }

            trailTimer += Time.deltaTime;
            if (trailTimer >= dropTrail)
            {
                trailTimer = 0;
                Paint(transform.position);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        anim.SetLayerWeight(1, 1f);
        anim.SetTrigger("Hit");
        health -= amount;
        audioSource.PlayOneShot(blood);
        
        if (health <= 0)
        {
            Die();
        }
        
        else if (health / maxHealth <= .4)
        {
            SetLayerRecursively(transform);
            edible = true;
        }

        FightOrFlight();
    }

    public void Bitten()
    {
        dead = true;
        anim.SetTrigger("Bitten");
        Destroy(agent);
        StartCoroutine(playBoneCracking());
        GameObject spawnedBlood = Instantiate(bloodSpray, bloodLoc.position, Quaternion.identity);
        spawnedBlood.transform.parent = gameObject.transform;
        spawnedBlood.transform.localScale = new Vector3(1, 1, 1);
        Destroy(spawnedBlood, 15);
        anim.SetBool("Dead", true);
        Destroy(gameObject, 15.0f);
        gameObject.tag = "Untagged";
        gameController.GetComponent<GameController>().UpdatePeople();
        player.GetComponent<PlayerController>().hittable.Remove(gameObject);
    }

    private void Die()
    {
        Destroy(agent);
        audioSource.PlayOneShot(deathYell);
        anim.SetTrigger("Die");
        anim.SetBool("Dead", true);
        dead = true;
        Destroy(gameObject, 15.0f);
        gameObject.tag = "Untagged";
        player.GetComponent<PlayerController>().hittable.Remove(gameObject);
        gameController.GetComponent<GameController>().UpdatePeople();
    }

    private void SetLayerRecursively(Transform root)
    {
        root.gameObject.layer = LayerMask.NameToLayer("Outline");
        foreach (Transform child in root)
            SetLayerRecursively(child);
    }

    IEnumerator playBoneCracking()
    {
        audioSource.PlayOneShot(biteYell);
        yield return new WaitForSeconds(biteYell.length);
        foreach (AudioClip clip in boneCrunching)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }


    private void FightMode()
    {
        anim.SetBool("Hiding", false);
        attackTimer += Time.deltaTime;
        agent.SetDestination(player.transform.position);
        float dist = Vector3.Distance(player.transform.position,  transform.position);
        
        if (dist < 5f && dist > -5f)
        {
            anim.SetBool("Running", false);
            agent.SetDestination(transform.position);
            AttackPlayer();
        } else
        {
            anim.SetBool("Running", true);
        }
    }

    private void FleeMode()
    {
        if (CanSeePlayer())
        {
            anim.SetBool("Hiding", false);
            correctFleeTimer += Time.deltaTime;
            // count down since last checked

            if (!correctedFlee || (agent.velocity.x < 1 && agent.velocity.z < 1))
            {
                // run to some random location away from player
                transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
                fleeLocation = transform.position + transform.forward * Random.Range(15, 20);
                agent.SetDestination(fleeLocation);
                correctedFlee = true;
                anim.SetBool("Running", true);
                Debug.Log("Fleeing from player");
            }
            if (correctFleeTimer >= correctFleeTimerCap)
            {
                correctedFlee = false;
                correctFleeTimer = 0;
                correctFleeTimerCap = Random.Range(2, 5);
                Debug.Log("Correcting flee");
            }
        } 
        else
        {
            state = PersonState.HIDE;
        }
    }

    private void HideMode()
    {
        if (CanSeePlayer())
        {
            hidden = false;
            FightOrFlight();
        } 

        else if (!hidden || !hiding)
        {
            if (!hiding)
            {
                int rand = Random.Range(0, hidingPlaces.Length);
                hideLocation = hidingPlaces[rand].transform.position;
                agent.SetDestination(hideLocation);
                anim.SetBool("Running", true);
                Debug.Log("Hiding Run");
                hiding = true;
            }
            else if (hiding)
            {
                float dist = Vector3.Distance(hideLocation, transform.position);
                if (dist < 10f && dist > -10f)
                {
                    hidden = true;
                    anim.SetBool("Hiding", true);
                    Debug.Log("Hiding 1");
                }
                
            }
            else if (agent.velocity.x < 3 && agent.velocity.z < 3)
            {
                hiding = true;
                hidden = true;
                anim.SetBool("Hiding", true);
                Debug.Log("Hiding 2");
            }
        }
        else
        {
            if (stateTimer >= newStateTime)
            {
                state = PersonState.FLEE;
                hidden = false;
                hiding = false;
                searching = false;
                Debug.Log("Hiding Finding new");
            }
        }
    }

    private void AttackPlayer()
    {
        if (attackTimer >= attackSpeed)
        {
            audioSource.PlayOneShot(attackSFX);
            attackTimer = 0;
            // hit player
            Vector3 lookPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(lookPos);
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
            anim.SetLayerWeight(2, 1f);
            anim.SetTrigger("Punch");
        }
    }

    private void FightOrFlight()
    {
        int rand = Random.Range(0,2);
        if (rand == 0)
        {
            state = PersonState.FIGHT;
        }
        else
        {
            state = PersonState.FLEE;
        }
    }

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        float visibleRange = 50f;
        Vector3 guardEyes = transform.position;
        guardEyes.y = 3f;

        if (Vector3.Distance(guardEyes, player.transform.position) < visibleRange)
        {
            if (Physics.Raycast(guardEyes, (player.transform.position - guardEyes), out hit, visibleRange))
            {
                return hit.transform == player.transform;
            }
        }
        return false;
    }

    public void Paint(Vector3 location)
    {

        int n = -1;

        int drops = 1;
        RaycastHit hit;

        // Generate multiple decals in once
        while (n <= drops)
        {
            n++;

            // Get a random direction (beween -n and n for each vector component)
            var fwd = transform.TransformDirection(Vector3.down);

            // Raycast around the position to splash everwhere we can
            if (Physics.Raycast(location, fwd, out hit, 2))
            {
                // Create a splash if we found a surface
                var paintSplatter = GameObject.Instantiate(trail,
                                                           location,

                                                           // Rotation from the original sprite to the normal
                                                           // Prefab are currently oriented to z+ so we use the opposite
                                                           Quaternion.identity) as Transform;

                // Random scale
                var scaler = Random.Range(.5f, .5f);

                paintSplatter.localScale = new Vector3(
                    paintSplatter.localScale.x * scaler,
                    paintSplatter.localScale.y ,
                    paintSplatter.localScale.z * scaler
                );

                // Random rotation effect
                Vector3 paintRot = paintSplatter.transform.rotation.eulerAngles;
                paintRot = new Vector3(paintRot.x, transform.rotation.eulerAngles.y + 180, paintRot.z);
                paintSplatter.transform.rotation = Quaternion.Euler(paintRot);


                // TODO: What do we do here? We kill them after some sec?
                Destroy(paintSplatter.gameObject, 180);
            }
        }
    }
}
