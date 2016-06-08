using UnityEngine;
using System.Collections;

public class UpdateDices : Photon.MonoBehaviour
{
    private Vector3[] correctDicePos;
    private Quaternion[] correctDiceRot;

    void Awake()
    {
        correctDicePos = new Vector3[transform.childCount];
        correctDiceRot = new Quaternion[transform.childCount];
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            for (int c = 0; c < transform.childCount; c++)
            {

                transform.GetChild(c).transform.position = correctDicePos[c] ;
                transform.GetChild(c).transform.rotation =  correctDiceRot[c] ;
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Debug.Log("isWriting");
            for (int c = 0; c < transform.childCount; c++)
            {
                stream.SendNext(transform.GetChild(c).transform.position);
                stream.SendNext(transform.GetChild(c).transform.rotation);
            }

        }
        else
        {
            Debug.Log(" not isWriting");
            if (stream.Count > 0)
            {
                for (int c = 0; c < transform.childCount; c++)
                {
                    correctDicePos[c] = (Vector3)stream.ReceiveNext();
                    correctDiceRot[c] = (Quaternion)stream.ReceiveNext();
                    //transform.GetChild(c).transform.position = (Vector3)stream.ReceiveNext();
                    //transform.GetChild(c).transform.rotation = (Quaternion)stream.ReceiveNext();
                }
            }
        }
    }


}
