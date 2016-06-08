using UnityEngine;
using System.Collections;

public class EndGameCanvas : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Menu()
    {
        PhotonNetwork.Disconnect();
        Application.LoadLevel("Lobby");
    }
}
