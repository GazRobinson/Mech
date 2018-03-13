using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBController : MonoBehaviour {

    Rigidbody rigidbody;
    Vector3 moveDirection = Vector3.zero;
    public float speed = 6.5f;
    int frame = 0;


	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        frame++;
      //  Debug.Log(frame);
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        moveDirection *= speed;


        rigidbody.AddForce(moveDirection);

      //  Debug.Log(moveDirection);
       // rigidbody.AddForce(Vector3.right * 100f * Input.GetAxis("Horizontal"));

        if(Input.GetButton("Jump")){
            rigidbody.AddForce(Vector3.up*100f, ForceMode.Impulse);
        }

	}
}
