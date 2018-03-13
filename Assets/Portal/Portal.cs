using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
    public Portal Other = null;
    private Transform camTransform;

    public Quaternion AngleBetween {
        get {
            //return Quaternion.AngleAxis( (Other.transform.localEulerAngles.y + 180f) - transform.localEulerAngles.y, transform.up);
            return Quaternion.FromToRotation( transform.forward, -Other.transform.forward );
        }
    }

	// Use this for initialization
	void Start () {
        camTransform = transform.GetChild( 0 );
	}
	
	// Update is called once per frame
	void Update () {
        UpdateCam();
	}


    void UpdateCam() {
        camTransform.localPosition = Other.transform.InverseTransformPoint( PortalGun.Instance.transform.position );
        camTransform.rotation = AngleBetween * PortalGun.Instance.transform.rotation;
    }
}
