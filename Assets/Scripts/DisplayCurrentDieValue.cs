using UnityEngine;
using System.Collections;

public class DisplayCurrentDieValue : MonoBehaviour
{
    public LayerMask dieValueColliderLayer = -1;

    public StaticValues.Type type;
    public int amount;
    private bool rollComplete = false;
    public PlayerCanvasController pcCtrl;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, dieValueColliderLayer))
        {
            type = hit.collider.GetComponent<DiceValue>().FaceType;
            amount = hit.collider.GetComponent<DiceValue>().value;

        }

        if (rigidbody.IsSleeping() && !rollComplete)
        {
            rollComplete = true;
            pcCtrl.DiceReady();
        }
        else if (!rigidbody.IsSleeping())
        {
            rollComplete = false;
        }

    }


    //	void OnGUI ()
    //
    //		GUILayout.Label (currentValue.ToString ());
    //	}
}
