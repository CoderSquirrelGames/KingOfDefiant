using UnityEngine;
using System.Collections;

public class DiceValue : MonoBehaviour
{
    public StaticValues.Type FaceType;
    public int value;

    void Start()
    {
        if (FaceType.Equals(StaticValues.Type.VictoryPoints))
        {
            value = int.Parse(name);
        } 
    }
}