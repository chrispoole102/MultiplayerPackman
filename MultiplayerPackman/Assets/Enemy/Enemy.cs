using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour {

    private NavMeshAgent nma;

	// Use this for initialization
	void Start ()
    {
        nma = GetComponent<NavMeshAgent>();

        StartCoroutine(slowUpdate());
	}
	
    public IEnumerator slowUpdate()
    {
        while (true)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length>0)
            {
                int closest = 0;
                float closestdist = Vector3.Distance(players[0].transform.position, transform.position);
                for (int i = 0; i<players.Length-1; i++)
                {
                    float dist = Vector3.Distance(players[i].transform.position, transform.position);
                    if (dist<closestdist)
                    {
                        closestdist = dist;
                        closest = i;
                    }
                }
                nma.destination = players[closest].transform.position;
            }
            else
                nma.destination = transform.position;

            yield return new WaitForSeconds(.5f);
        }
    }
}
