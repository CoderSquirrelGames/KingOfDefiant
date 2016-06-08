using UnityEngine;
using System.Collections;

public class Lobby : Photon.MonoBehaviour
{

    private string PlayerName, Aux;

    // Use this for initialization
    void Start()
    {
        PlayerName = "Player " + Random.Range(1, 10);
        Aux = "OFFLINE";
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 30), Aux);
        if (!PhotonNetwork.connected)
        {

            if (GUI.Button(new Rect(200, 10, 160, 30), "CONNECT"))
            {
                PhotonNetwork.playerName = PlayerName;
                PhotonNetwork.ConnectUsingSettings("Teste");
            }
        }
        else
        {
            Aux = "ONLINE";

            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                if (GUI.Button(new Rect(200, 10, 160, 30), "START ROOM"))
                {
                    GUI.enabled = false;
                    PhotonNetwork.CreateRoom(StaticValues.GAMEROOM);
                }
            }
            else
            {
                if (GUI.Button(new Rect(200, 10, 160, 30), "ENTER ROOM"))
                {
                    GUI.enabled = false;
                    PhotonNetwork.JoinRoom(StaticValues.GAMEROOM);
                }
            }
            if (PhotonNetwork.inRoom)
            {
                if (PhotonNetwork.room.playerCount >= 5)
                {
                    Application.LoadLevel("GameScene");
                }
                else
                {
                    GUI.Box(new Rect(250, 10, 100, 30), "Players in the room " + PhotonNetwork.room.playerCount + ", players necessary 5");
                }
            }
        }


    }


    void OnJoinedRoom()
    {
        Debug.Log("Created a room " + PhotonNetwork.room.name);

        // Application.LoadLevel("GameScene");
    }
}
