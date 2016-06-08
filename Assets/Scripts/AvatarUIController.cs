using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AvatarUIController : MonoBehaviour
{
    public Text Life, Energy, Points;
    public GameObject Arrow;
    // Use this for initialization
    void Start()
    {

    }

    public void SetArrow(bool state)
    {
        Arrow.SetActive(state);
    }


}
