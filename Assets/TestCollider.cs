using UnityEngine;
using System.Collections;

public class TestCollider : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        guiText.text = "HelloWorld";
        guiText.transform.position = transform.parent.transform.position;
    }

}
