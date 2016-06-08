using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CanvasController : Photon.MonoBehaviour
{
    Text Status, StatusPlayers;
    public Text NOfP;
    int PlayersN = 2;
    Button BT_Connect, BT_StartRoom, BT_EnterRoom;
    private string PlayerName;
    bool NotClicked = true, actived = false;
    public GameObject Plane;
    void Awake()
    {
        Plane.gameObject.SetActive(false);


        StatusPlayers = transform.FindChild("StatusPlayers").GetComponent<Text>();
        Status = transform.FindChild("Status").GetComponent<Text>();
        BT_Connect = transform.FindChild("Connect").GetComponent<Button>();
        BT_StartRoom = transform.FindChild("StartRoom").GetComponent<Button>();
        BT_EnterRoom = transform.FindChild("EnterRoom").GetComponent<Button>();
        Status.text = "Offline";


    }
    void Update()
    {

        if (!PhotonNetwork.connected)
        {
            BT_StartRoom.interactable = false;
            BT_EnterRoom.interactable = false;
            if (!actived)
                BT_Connect.interactable = true;
        }
        else
        {
            if (PhotonNetwork.inRoom)
            {
                if (PhotonNetwork.room.playerCount >= PhotonNetwork.room.maxPlayers)
                {
                    StartCoroutine(StartingGame());
                }
                else
                {
                    StatusPlayers.text = "Waiting more " + (PhotonNetwork.room.maxPlayers - PhotonNetwork.room.playerCount) + " players...";
                }
            }



            Status.text = "Online";

            if (NotClicked)
            {
                if (PhotonNetwork.GetRoomList().Length != 0)
                {
                    BT_StartRoom.interactable = false;
                    BT_EnterRoom.interactable = true;

                }
                else
                {
                    BT_StartRoom.interactable = true;
                    BT_EnterRoom.interactable = false;

                }

            }



        }


    }


    public void ConnectPlayer()
    {
        PhotonNetwork.playerName = "Player " + Random.Range(0, 1986);
        PhotonNetwork.ConnectUsingSettings("KingOfDefiant2.0");
        actived = true;
        BT_Connect.interactable = false;

    }

    public void StartRoom()
    {
        Plane.gameObject.SetActive(true);

        NotClicked = false;


    }

    public void EnterRoom()
    {
        NotClicked = false;
        BT_EnterRoom.interactable = false;
        BT_StartRoom.interactable = false;
        PhotonNetwork.JoinRoom(StaticValues.GAMEROOM);
    }

    void OnJoinedRoom()
    {
        //        Debug.Log("Created a room " + PhotonNetwork.room.name);

        // Application.LoadLevel("GameScene");
    }

    public void Increase()
    {
        if (PlayersN < 5)
        {
            PlayersN++;
            NOfP.text = PlayersN.ToString();
        }
    }

    public void Decrease()
    {
        if (PlayersN > 2)
        {
            PlayersN--;
            NOfP.text = PlayersN.ToString();
        }

    }

    public void Continue()
    {
        Plane.gameObject.SetActive(false);
        BT_StartRoom.interactable = false;
        BT_EnterRoom.interactable = false;

        PhotonNetwork.CreateRoom(StaticValues.GAMEROOM, new RoomOptions() { maxPlayers = PlayersN }, null);
    }
    IEnumerator StartingGame()
    {

        //        Debug.Log("Going there");

        StatusPlayers.text = "Players found. Starting game...";
        yield return new WaitForSeconds(1);
        //StatusPlayers.text = "5 players find. Starting in 2";
        //yield return new WaitForSeconds(1);
        //StatusPlayers.text = "5 players find. Starting in 1";
        //yield return new WaitForSeconds(1);
        Application.LoadLevel("GameScene");
    }
}
