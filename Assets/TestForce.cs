using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForce : MonoBehaviour {
    Rigidbody rb;
    public float f = 15f;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if ( Input.GetKey( KeyCode.K ) ) {
            rb.AddForce( Vector3.up * f );
        }
	}
}
