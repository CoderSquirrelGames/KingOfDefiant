using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Server : Photon.MonoBehaviour
{
    int NumberOfPlayers, CurrentPlayer, PlayerOnCity, PlayerOnBay;
    List<int> PlayersDead;
    public GameObject Stuff;
    public PlayerCanvasController PCC;
    List<GameObject> Avatars = new List<GameObject>();
    public GameObject WorldCanvas;
    Dictionary<string, Vector3> CityPosition = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> BayPosition = new Dictionary<string, Vector3>();

    void Awake()
    {
        CityPosition.Add("P1", new Vector3(19.25f, 0f, -4.96f));
        CityPosition.Add("P2", new Vector3(13.25f, 0f, -5.96f));
        CityPosition.Add("P3", new Vector3(5.69f, 0f, -6.43f));
        CityPosition.Add("P4", new Vector3(-1.19f, 0f, -6.17f));
        CityPosition.Add("P5", new Vector3(-7.38f, 0f, -6.04f));

        BayPosition.Add("P1", new Vector3(24.1f, 0f, -9.9f));
        BayPosition.Add("P2", new Vector3(19.14f, 0f, -9.73f));
        BayPosition.Add("P3", new Vector3(10.76f, 0f, -10.22f));
        BayPosition.Add("P4", new Vector3(3.86f, 0f, -11.05f));
        BayPosition.Add("P5", new Vector3(-2.2f, 0f, -11.2f));
        if (!PhotonNetwork.isMasterClient)
        {
            this.enabled = false;
        }
        else
        {
            PlayersDead = new List<int>();
            NumberOfPlayers = PhotonNetwork.playerList.Length;
            CurrentPlayer = 1;
        }


    }
    void Start()
    {
        photonView.RPC(StaticValues.SETARROW, PhotonTargets.All, 1, 1);
        photonView.RPC(StaticValues.SETTURN, PhotonTargets.All, 1);
        //SetPlayerTurn();
    }
    void SetPlayerTurn()
    {
        int oldPlayer = CurrentPlayer;
        if (CurrentPlayer < NumberOfPlayers)
            CurrentPlayer++;
        else
            CurrentPlayer = 1;



        if (PlayersDead.Contains(CurrentPlayer))
            SetPlayerTurn();
        else
        {
            photonView.RPC(StaticValues.SETARROW, PhotonTargets.All, CurrentPlayer, oldPlayer);
            photonView.RPC(StaticValues.SETTURN, PhotonTargets.All, CurrentPlayer);
            //photonView.RPC(StaticValues.SETARROW, PhotonTargets.All, CurrentPlayer, oldPlayer);
            //if (CurrentPlayer == PlayerOnCity)
            //    photonView.RPC(StaticValues.SETTURN, PhotonTargets.All, CurrentPlayer, 1);
            //else if (CurrentPlayer == PlayerOnBay)
            //    photonView.RPC(StaticValues.SETTURN, PhotonTargets.All, CurrentPlayer, 2);
            //else
            //    photonView.RPC(StaticValues.SETTURN, PhotonTargets.All, CurrentPlayer, 0);
        }
    }

    [RPC]
    void ChangePlayer()
    {
        SetPlayerTurn();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="where">0 means out of tokyo, 1 mean in</param>
    [RPC]
    void SetPlayersDamage(int sender, int amount, int where)
    {
        photonView.RPC(StaticValues.DEALDAMAGE, PhotonTargets.All, sender, amount, where);
    }
    [RPC]
    void GetTokyoInformation(int Request)
    {
        if (NumberOfPlayers - PlayersDead.Count < 5)
        {
            PlayerOnBay = -1;
        }

        photonView.RPC(StaticValues.TOKYORESPONSE, PhotonTargets.All, Request, PlayerOnCity, PlayerOnBay);

    }
    [RPC]
    public void InformWinner(int playerId)
    {
        Debug.Log("Info Winner");
        photonView.RPC(StaticValues.ENDGAME, PhotonTargets.All, playerId);

    }
    [RPC]
    void SetPlayerOnTokyo(int where, int who)
    {

        switch (where)
        {
            case 0:
                PlayerOnCity = who;
                photonView.RPC(StaticValues.AVATARTOKYO, PhotonTargets.All, who, CityPosition["P" + who].x, CityPosition["P" + who].y, CityPosition["P" + who].z);

                break;
            case 1:
                PlayerOnBay = who;
                photonView.RPC(StaticValues.AVATARTOKYO, PhotonTargets.All, who, BayPosition["P" + who].x, BayPosition["P" + who].y, BayPosition["P" + who].z);

                break;
        }

    }
    [RPC]
    void LeaveTokyo(int playerID)
    {
        if (playerID == PlayerOnBay)
        {
            PlayerOnBay = 0;
        }
        else if (playerID == PlayerOnCity)
        {
            PlayerOnCity = 0;
        }

        photonView.RPC(StaticValues.AVATARLEAVETOKYO, PhotonTargets.All, playerID);

    }

    [RPC]
    void LeaveGame(int playerID)
    {
        Debug.Log("Player " + playerID + " leaving game");
        PlayersDead.Add(playerID);
        //  photonView.RPC(StaticValues.DELETEAVATAR, PhotonTargets.All, playerID);
        if (NumberOfPlayers - PlayersDead.Count == 1)
        {
            for (int i = 1; i < NumberOfPlayers + 1; i++)
            {
                if (!PlayersDead.Contains(i))
                {
                    InformWinner(i);
                }
            }
        }
    }
    [RPC]
    void InstatiateAvatar(int playerID)
    {
        GameObject MyAvatar = PhotonNetwork.Instantiate("P" + playerID.ToString(), Vector3.zero, Quaternion.identity, 0);
        MyAvatar.transform.position = Vector3.zero;
        MyAvatar.gameObject.transform.SetParent(WorldCanvas.transform, false);
        MyAvatar.transform.position = Vector3.zero;
        Avatars.Add(MyAvatar);
        MyAvatar.transform.position = Vector3.zero;

        photonView.RPC(StaticValues.SETPARENT, PhotonTargets.Others, playerID);

    }


}
