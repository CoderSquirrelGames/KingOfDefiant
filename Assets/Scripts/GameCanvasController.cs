using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameCanvasController : MonoBehaviour
{

    public List<Image> Dices = new List<Image>();
    public Image PlayerImg;
    public List<Sprite> DicesFaces;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetPlayerImage(Sprite img)
    {
        PlayerImg.sprite = img;
    }

    public void SetDices(int[] dices)
    {
        //        Debug.Log(dices.Length + " " + Dices.Count + " " + DicesFaces);
        for (int d = 0; d < Dices.Count; d++)
        {
            Dices[d].sprite = DicesFaces[GetIndexByName("Face" + dices[d])];
        }
    }

    int GetIndexByName(string name)
    {
        for (int s = 0; s < DicesFaces.Count; s++)
        {
            if (DicesFaces[s].name.Equals(name))
            {
                return s;
            }
        }
        return 0;
    }
}
