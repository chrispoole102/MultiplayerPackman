using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    public GameObject prefab;//because the game piece is spawned, it must be added to the network manager's "registered spawnable prefabs" list

    [SyncVar]
    public GameObject myPiece;
    [SyncVar]
    public string myName;
    [SyncVar]
    public int score;

    private GameObject scoreDisplay;

	// Use this for initialization
	void Start ()
    {

        if (isLocalPlayer)
        {
            string temp = GameObject.Find("NetworkManager").GetComponent<MyNetworkManager>().playerName;//get our name from our local network manager
            Cmd_setName(temp);//give our name to the server
            
            scoreDisplay = GameObject.Find("ScoreDisplay");
            scoreDisplay.SetActive(false);
        }
        
	    if(isServer)
        {
            StartCoroutine(spawnPiece());
        } 
        StartCoroutine(slowUpdate());
	}
	public IEnumerator spawnPiece()
    {
        while (myName=="")//wait for us to get a name
        {
            yield return new WaitForSeconds(0.1f);
        }

        GameObject Temp = Instantiate(prefab);
                                              
        Temp.GetComponent<PackmanGamePiece>().myName = myName;
        Temp.GetComponent<PackmanGamePiece>().owner = this.gameObject;
        
        NetworkServer.Spawn(Temp);
        myPiece = Temp;

        Camera.main.GetComponent<CameraFollow>().target = myPiece.transform;
    }
    public IEnumerator slowUpdate()
    {
        while(true)
        {
            while(myPiece == null)
            {
                yield return new WaitForSeconds(.1f);
            }
            while(myPiece != null)
            {
                if (isLocalPlayer)
                {
                    //I should try/catch here.
                    float xt = Input.GetAxisRaw("Horizontal");
                    float yt = Input.GetAxisRaw("Vertical");
                    Vector3 tempV = new Vector3(xt, 0, yt) * 5.0f;
                    if(
                    (!isServer && myPiece.GetComponent<NetworkTransform>().targetSyncVelocity != tempV)||
                    (isServer && myPiece.GetComponent<Rigidbody>().velocity != tempV)
                    )
                    {
                        Cmd_Move(xt, yt);
                    }
                    if (!scoreDisplay.activeSelf)
                    {
                        if (Input.GetAxisRaw("Jump") > 0)
                        {
                            scoreDisplay.SetActive(true);
                            string temp = "";
                            foreach (GameObject npgo in GameObject.FindGameObjectsWithTag("NetworkPlayer"))
                            {
                                temp += npgo.GetComponent<NetworkPlayer>().myName + ": ";
                                temp += npgo.GetComponent<NetworkPlayer>().score;
                                temp += "\n";
                            }
                            scoreDisplay.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = temp;
                            //update text
                        }
                    }
                    else
                    {
                        if (Input.GetAxisRaw("Jump") == 0)
                            scoreDisplay.SetActive(false);
                        
                    }
                }

                yield return new WaitForSeconds(.05f);
            }

        }
    }
    
    [Command]//Commands are only run on the server
    public void Cmd_Move(float x, float y)
    {
        myPiece.GetComponent<Rigidbody>().velocity = new Vector3(x, 0, y) * 5.0f;//move the piece
    }
    [Command]
    public void Cmd_setName(string n)
    {
        myName = n;
    }

}
