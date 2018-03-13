using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour {

    public static PortalGun Instance;
    public Portal portalA;
    public Portal portalB;
	// Use this for initialization
	void Awake () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if ( Input.GetButtonDown( "Fire1" ) ) {
            FirePortal( 0 );
        }
        if ( Input.GetButtonDown( "Fire2" ) ) {
            FirePortal( 1 );
        }
	}

    void FirePortal( int pIndex ) {
        Portal p = pIndex == 0 ? portalA : portalB;
        RaycastHit hit;
        if ( Physics.Raycast( transform.position, transform.forward, out hit ) ) {
            p.transform.position = hit.point + hit.normal * 0.05f;
            p.transform.LookAt( p.transform.position - hit.normal, Vector3.up );
        }
    }
}
