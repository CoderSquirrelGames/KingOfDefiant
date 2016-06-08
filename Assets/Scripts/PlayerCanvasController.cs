using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class PlayerCanvasController : MonoBehaviour
{


    //   public ApplyForceInRandomDirection APFIRD;
    public Client C;
    public Text PlayerTurn, PlayerOnTokyo, RollRemainingText;
    //EnergyText, LifeText, PointsText;
    int Life = 10, VictoryPoints, Energy, RerollsRemaining = 3;
    public int DicesReady = 0;

    public Image PlayerImage;
    public bool OnTokyo = false, GameOver = false, ResponseTokyo = false;
    // public Slider LifeSlider, PointSlider, EnergySlider;
    // public Button BT_Roll, BT_Reroll, BT_Leave, BT_Keep;
    /// <summary>
    /// 0:BT_Roll
    /// 1:BT_Reroll
    /// 2:BT_Leave
    /// 3:BT_Keep
    /// </summary>
    public List<Button> Buttons;
    /// <summary>
    /// 0:LifeSlider
    /// 1:PointSlider
    /// 2:EnergySlider
    /// </summary>
    public List<Slider> Sliders;

    /// <summary>
    /// 0:Life
    /// 1:Point
    /// 2:Energy
    /// </summary>
    public List<Text> SlidersText;
    public List<Sprite> PlayersImages;
    //   public List<Image> CanvasDices;DiceFaces,
    public List<GameObject> Dices, Panels;
    bool Ready = false, Able;
    public bool MyTurn = false;

    void Start()
    {
        PlayerImage.sprite = PlayersImages[C.MyId - 1];
    }


    public void GameOverCanvas()
    {

        C.GameOver();
        Panels[1].gameObject.SetActive(true);
    }
    public void DiceReady()
    {
        DicesReady++;
        Ready = false;
        if (DicesReady > 5)
        {

            DicesReady = 0;
            Ready = true;

        }



    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && MyTurn)
        { Buttons[3].interactable = true; }
        if (Input.GetKeyDown(KeyCode.K) && MyTurn && Able)
        { Keep(); }
        if (Input.GetKeyDown(KeyCode.L))
        { LeaveTokyo(); }

        if (Ready)
        {
            Able = true;
            Ready = false;
            C.NotifyDices(Dices);
            RerollsRemaining--;
            RollRemainingText.text = RerollsRemaining.ToString();

        }



        if (RerollsRemaining < 1)
        {
            NotAllowingRoll();
        }

    }

    public void RollDices()
    {
        //  Debug.Log("PLAYER CANVAS "+C.MyId+" : RollDices");
        //foreach (Image i in CanvasDices)
        //{
        //    i.sprite = DiceFaces[Random.Range(0, DiceFaces.Count)];
        //}
        //BT_Roll.interactable = false;
        //BT_Reroll.interactable = true;
        //BT_Leave.interactable = true;
        //BT_Keep.interactable = true;
        Buttons[0].interactable = false;
        for (int i = 1; i < Buttons.Count; i++)
        {
            Buttons[i].interactable = true;
        }
    }

    public void Reroll()
    {
        //   Debug.Log("PLAYER CANVAS " + C.MyId + ": Reroll");
        RerollsRemaining--;
        RollDices();
        //Text txt = BT_Reroll.transform.GetChild(0).GetComponent<Text>();
        Text txt = Buttons[1].transform.GetChild(0).GetComponent<Text>();
        txt.text = "Reroll " + RerollsRemaining;
        if (RerollsRemaining < 1)
        {
            //BT_Reroll.interactable = false;
            Buttons[0].interactable = true;
        }
    }

    public void LeaveTokyo()
    {

        if (OnTokyo)
        {
            C.LeaveTokyo();
            OnTokyo = false;
            PlayerOnTokyo.text = "OUT OF TOKYO";
        }
    }


    public void Keep()
    {
        SetCanvasOff();
        StartCoroutine(KeepDices());
        //ResolvingDices();
    }

    public void SetCanvasOn()
    {
        Able = false;
        if (OnTokyo)
        {
            SetPoints(0, 0, 2);
        }
        RerollsRemaining = 3;
        RollRemainingText.text = RerollsRemaining.ToString(); ;
        PlayerTurn.gameObject.SetActive(true);
        AllowingRoll();
    }

    public void SetCanvasOff()
    {
        //      Debug.Log("PLAYER CANVAS " + C.MyId + ": SetCanvasOff");
        PlayerTurn.gameObject.SetActive(false);
        //BT_Keep.interactable = false;
        //BT_Reroll.interactable = false;
        //BT_Roll.interactable = false;
        Buttons[3].interactable = false;

        NotAllowingRoll();
    }

    void SetPoints(int life, int energy, int victoryPoint)
    {

        if (!OnTokyo && Life < 10)
            Life += life;

        if (life > 0)
        {
            if (Life > 9)
            {
                Life = 10;
            }
        }
        Energy += energy;
        VictoryPoints += victoryPoint;
        C.UpdateAvatar(Life, Energy, VictoryPoints);
        UpdateCanvas();
    }

    public void SetDamage(int amount)
    {
        Life -= amount;
        UpdateCanvas();
        if (Life < 1)
        {
            GameOverCanvas();

        }
    }


    void UpdateCanvas()
    {
        //   Debug.Log("PLAYER CANVAS " + C.MyId + ": UpdateCanvas");
        //EnergyText.text = Energy.ToString();
        //LifeText.text = Life.ToString();
        //PointsText.text = VictoryPoints.ToString();
        //LifeSlider.value = Life;
        //EnergySlider.value = Energy;
        //PointSlider.value = VictoryPoints;
        C.UpdateAvatar(Life, Energy, VictoryPoints);
        Sliders[0].value = Life;
        Sliders[1].value = VictoryPoints;
        Sliders[2].value = Energy;
        SlidersText[0].text = Life.ToString();
        SlidersText[1].text = VictoryPoints.ToString();
        SlidersText[2].text = Energy.ToString();
    }

    public void GoingToTokyo(string which)
    {
        //    Debug.Log("PLAYER CANVAS " + C.MyId + ": GoingToTokyo");
        PlayerOnTokyo.text = "IN TOKYO " + which.ToUpper();
        VictoryPoints++;
        OnTokyo = true;
        UpdateCanvas();
    }

    int GetValue(StaticValues.Type type)
    {

        int number = 0;

        DisplayCurrentDieValue dcdv;
        if (!type.Equals(StaticValues.Type.VictoryPoints))
        {
            foreach (GameObject dice in Dices)
            {
                dcdv = dice.GetComponent<DisplayCurrentDieValue>();
                if (dcdv.type.Equals(type))
                {
                    number++;
                }

            }
            return number;
        }
        else
        {
            for (int j = 1; j < 4; j++)
            {
                number = 0;
                foreach (GameObject dice in Dices)
                {
                    dcdv = dice.GetComponent<DisplayCurrentDieValue>();
                    if (dcdv.amount == j)
                    {
                        number++;
                    }

                }
                if (number >= 3)
                {
                    if (number > 3)
                    {
                        int aux = number - j;
                        number = j + aux;
                        return number;
                    }
                    else
                    {
                        number = j;
                        return number;
                    }


                }
            }

        }

        return 0;
    }


    void ResolvingDices()
    {
        //Debug.Log("1 -- " + GetValue(Type.Life));
        //Debug.Log("2 -- " + GetValue(Type.Energy));
        //Debug.Log("3 -- " + GetValue(Type.VictoryPoints));
        //Debug.Log("4 -- " + GetValue(Type.Damage));

        //foreach (GameObject dice in Dices)
        //{
        //    DisplayCurrentDieValue dcdv = dice.GetComponent<DisplayCurrentDieValue>();

        //    Debug.Log(dcdv.type.ToString() + " " + dcdv.amount);
        //}

        //        Debug.Log(lifeV + " " + energyV + " " + pointsV);
        SetPoints(GetValue(StaticValues.Type.Life), GetValue(StaticValues.Type.Energy), GetValue(StaticValues.Type.VictoryPoints));


        int damage = GetValue(StaticValues.Type.Damage);
        if (damage > 0)
        {
            C.DoDamage(damage);
            if (!OnTokyo)
            {
                C.RequestTokyoInformation();
            }
            else
            {
                SetCanvasOff();
                C.NotifyServer();
            }

        }
        else
        {
            SetCanvasOff();
            C.NotifyServer();
        }


    }

    public int GetVictoryPoints()
    {
        return VictoryPoints;
    }

    public void NotAllowingRoll()
    {
        foreach (GameObject dice in Dices)
        {
            dice.GetComponent<ApplyForceInRandomDirection>().enabled = false;
        }
    }

    public void AllowingRoll()
    {
        foreach (GameObject dice in Dices)
        {
            dice.GetComponent<ApplyForceInRandomDirection>().enabled = true;
        }
    }



    IEnumerator KeepDices()
    {
        C.NotifyDices(Dices);
        yield return new WaitForSeconds(2);
        ResolvingDices();
    }
}
