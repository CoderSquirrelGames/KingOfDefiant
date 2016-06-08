using UnityEngine;
using System.Collections;

public class ApplyForceInRandomDirection : MonoBehaviour
{
    public float forceAmount = 10.0f;
    public float torqueAmount = 10.0f;
    public ForceMode forceMode;
    public bool Rolled = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
//            Debug.Log("D");
            Rolled = true;
            rigidbody.AddForce(Random.onUnitSphere * forceAmount, forceMode);
            rigidbody.AddTorque(Random.onUnitSphere * torqueAmount, forceMode);
        }
    }

    
}
