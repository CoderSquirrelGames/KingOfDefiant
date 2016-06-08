using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Client : Photon.MonoBehaviour
{
    public PlayerCanvasController pcCtrl;
    public GameCanvasController GameController;
    //private string Msg = "", CurrentMsg = "";
    public int MyId;
    bool MyTurn;
    public int[] TempPlayersOnTokyo = new int[2];
    public GameObject WorldCanvas;
    Dictionary<string, Vector3> ArrowPosition = new Dictionary<string, Vector3>();
    public GameObject Arrow;
    bool Winner
    {
        get
        {
            return pcCtrl.GetVictoryPoints() >= 12;
        }
    }
    // Use this for initialization
    void Awake()
    {
        ArrowPosition.Add("P1", new Vector3(-13.25f, 16.075f, 3.53f));
        ArrowPosition.Add("P2", new Vector3(-7.48f, 16.075f, 3.53f));
        ArrowPosition.Add("P3", new Vector3(-0.66f, 16.075f, 3.53f));
        ArrowPosition.Add("P4", new Vector3(5.92f, 16.075f, 3.53f));
        ArrowPosition.Add("P5", new Vector3(13.07f, 16.075f, 3.53f));
        ArrowPosition.Add("CITY", new Vector3(3.45f, 17.4f, -9.78f));
        ArrowPosition.Add("BAY", new Vector3(6.99f, 17.18f, -12.77f));
    }
    void Start()
    {
        MyId = PhotonNetwork.player.ID;
        pcCtrl.SetCanvasOff();
        photonView.RPC(StaticValues.INSTANTIATEPLAYER, PhotonTargets.MasterClient, MyId);

    }
    void Update()
    {
        if (Winner)
        {
            photonView.RPC(StaticValues.WINNER, PhotonTargets.MasterClient, MyId);
        }

    }
    public void NotifyServer()
    {
        photonView.RPC(StaticValues.CHANGEPLAYER, PhotonTargets.MasterClient);
    }
    public void DoDamage(int amount)
    {
        if (pcCtrl.OnTokyo)
        {
            photonView.RPC(StaticValues.DEALDAMAGE, PhotonTargets.Others, MyId, amount, 0);
        }
        else
        {
            photonView.RPC(StaticValues.DEALDAMAGE, PhotonTargets.Others, MyId, amount, 1);
        }

    }
    public void RequestTokyoInformation()
    {
        photonView.RPC(StaticValues.TOKYOREQUEST, PhotonTargets.MasterClient, MyId);
    }
    void CheckGoingToTokyo()
    {
        if (TempPlayersOnTokyo[0] == 0)
        {
            pcCtrl.GoingToTokyo("CITY");
            photonView.RPC(StaticValues.SETTOKYO, PhotonTargets.MasterClient, 0, MyId);
        }
        else if (TempPlayersOnTokyo[1] == 0)
        {
            pcCtrl.GoingToTokyo("BAY");
            photonView.RPC(StaticValues.SETTOKYO, PhotonTargets.MasterClient, 1, MyId);
        }


        pcCtrl.SetCanvasOff();
        NotifyServer();
    }
    public void LeaveTokyo()
    {
        photonView.RPC(StaticValues.LEAVETOKYO, PhotonTargets.MasterClient, MyId);
    }
    public void GameOver()
    {
        LeaveTokyo();
        photonView.RPC(StaticValues.GAMEOVER, PhotonTargets.MasterClient, MyId);
        photonView.RPC(StaticValues.DELETEAVATAR, PhotonTargets.All, MyId);
    }
    [RPC]
    void SetParent(int playerID)
    {

        GameObject myAvatar;
        for (int i = 1; i <= playerID; i++)
        {

            myAvatar = GameObject.Find("P" + i + "(Clone)");
            if (myAvatar != null)
            {
                myAvatar.transform.position = Vector3.zero;
                myAvatar.transform.SetParent(WorldCanvas.transform);
                myAvatar.transform.position = Vector3.zero;
            }
        }

    }
    [RPC]
    void SetTokyoInformation(int Request, int PlayerOnCity, int PlayerOnBay)
    {
        if (Request == MyId)
        {
            pcCtrl.ResponseTokyo = true;
            TempPlayersOnTokyo[0] = PlayerOnCity;
            TempPlayersOnTokyo[1] = PlayerOnBay;


            CheckGoingToTokyo();
        }
    }
    [RPC]
    void EndGame(int winnerId)
    {

        Debug.Log("EndGame");
        if (winnerId == MyId)
        {

            Application.LoadLevel("EndGameScene");
        }
        else
        {
            GameOver();
            Debug.Log("not winner");
            pcCtrl.GameOverCanvas();

        }

        photonView.RPC(StaticValues.GAMEOVER, PhotonTargets.MasterClient, MyId);
    }
    //void SetTurn(int playerId, int onTokyo)
    [RPC]
    void SetTurn(int playerId)
    {
        GameController.SetPlayerImage(pcCtrl.PlayersImages[playerId - 1]);

        //if (onTokyo == 0)
        //    Arrow.transform.position = ArrowPosition["P" + playerId];
        //else if (onTokyo == 1)
        //    Arrow.transform.position = ArrowPosition["CITY"];
        //if (onTokyo == 2)
        //    Arrow.transform.position = ArrowPosition["BAY"];

        if (pcCtrl.GameOver)
        {
            NotifyServer();
        }
        else
        {
            if (playerId == MyId)
            {
                pcCtrl.MyTurn = true;
                pcCtrl.DicesReady = 0;
                pcCtrl.SetCanvasOn();
                pcCtrl.AllowingRoll();
            }
            else
            {

                pcCtrl.MyTurn = false;
                pcCtrl.SetCanvasOff();
                pcCtrl.NotAllowingRoll();

            }
        }
    }

    [RPC]
    void SetArrow(int playerID, int oldPlayer)
    {
        Transform oldplayer = WorldCanvas.transform.FindChild("P" + oldPlayer + "(Clone)");
        AvatarUIController co = oldplayer.GetComponent<AvatarUIController>();
        co.SetArrow(false);
        Transform newplayer = WorldCanvas.transform.FindChild("P" + playerID + "(Clone)");
        AvatarUIController co2 = newplayer.GetComponent<AvatarUIController>();
        co2.SetArrow(true);
    }

    [RPC]
    void GetDamage(int sender, int amount, int where)
    {
        if (sender != MyId)
        {
            switch (where)
            {

                case 0:
                    if (!pcCtrl.OnTokyo)
                    {
                        pcCtrl.SetDamage(amount);
                    }
                    break;


                case 1:
                    if (pcCtrl.OnTokyo)
                    {
                        pcCtrl.SetDamage(amount);
                    }
                    break;
            }
        }
    }
    void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }
    public void NotifyDices(List<GameObject> Dices)
    {
        int[] DiceFace = new int[6];
        DisplayCurrentDieValue dcdv;
        for (int d = 0; d < Dices.Count; d++)
        {
            dcdv = Dices[d].GetComponent<DisplayCurrentDieValue>();
            switch (dcdv.type)
            {
                case StaticValues.Type.Damage:
                    DiceFace[d] = 6;
                    break;
                case StaticValues.Type.Life:
                    DiceFace[d] = 4;
                    break;
                case StaticValues.Type.Energy:
                    DiceFace[d] = 5;
                    break;
                case StaticValues.Type.VictoryPoints:
                    DiceFace[d] = dcdv.amount;
                    break;

            }
        }
        photonView.RPC(StaticValues.UPDATEDICES, PhotonTargets.All, DiceFace);

    }

    [RPC]
    public void DicesFaces(int[] faces)
    {
        GameController.SetDices(faces);
    }
    public void UpdateAvatar(int life, int energy, int points)
    {
        photonView.RPC(StaticValues.UPDATEAVATAR, PhotonTargets.All, MyId, energy, life, points);
    }


    [RPC]
    void UpdateAvatar(int playerID, int energy, int life, int points)
    {
        AvatarUIController auic = GameObject.Find("P" + playerID + "(Clone)").GetComponent<AvatarUIController>();
        auic.Life.text = life.ToString();
        auic.Energy.text = energy.ToString();
        auic.Points.text = points.ToString();
    }


    [RPC]
    void SetAvatarOnTokyo(int playerID, float x, float y, float z)
    {

        GameObject.Find("P" + playerID + "(Clone)").transform.position = new Vector3(x, y, z);

    }

    [RPC]
    void SetAvatarOutOffTokyo(int playerID)
    {

        GameObject.Find("P" + playerID + "(Clone)").transform.position = Vector3.zero;

    }


    [RPC]
    void DeleteAvatar(int playerID)
    {
        GameObject obj = GameObject.Find("P" + playerID + "(Clone)");
        if (obj != null)
            Destroy(obj);
    }

}
