using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject[] people;

	// Use this for initialization
	void Awake () {
        people = GameObject.FindGameObjectsWithTag("Attackable");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdatePeople()
    {
        people = GameObject.FindGameObjectsWithTag("Attackable");
        if (people.Length <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {

    }
}
