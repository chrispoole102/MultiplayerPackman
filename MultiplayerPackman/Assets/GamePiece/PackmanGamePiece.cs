﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PackmanGamePiece : NetworkBehaviour {

    [SyncVar]
    public string myName;

    [SyncVar]
    public GameObject owner;//this won't cause network problems because its not updated

    public NetworkPlayer myPlayer;

    public GameObject bulletPrefab;

	// Use this for initialization
	void Start () {
        myPlayer = owner.GetComponent<NetworkPlayer>();
        StartCoroutine(waitForName());
        //StartCoroutine(slowUpdate());
	}

    public IEnumerator waitForName()
    {
        while (myName=="")
        {
            yield return new WaitForSeconds(0.01f);

        }
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = myName;
    }
    public IEnumerator slowUpdate()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isClient)//we don't need to use myPlayer here
        {
            if (other.gameObject.tag == "OnePointPickup")
            {
                Destroy(other.gameObject);
            }
            //Visual Effect
        }
        if (isServer)
        {
            if (other.gameObject.tag == "OnePointPickup")
            {
                myPlayer.score++;
                Destroy(other.gameObject);
            }
            //Game State Change
        }
    }
}
