using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb :MonoBehaviour {
    [HideInInspector]
    public Transform startAnchor;
    [HideInInspector]
    public Transform endAnchor;
    public bool clamp = false;
    public bool x = true, y = false, z = false;
    public float min, max;
    public Vector3 length {
        get {
            return endAnchor.position - startAnchor.position;
        }
    }

    void Awake() {
        startAnchor = transform;
        endAnchor = transform.Find( "End" );
    }
}