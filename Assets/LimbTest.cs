using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LimbTest : MonoBehaviour {
    public Transform root, upper, lower;
    public Limb[] segments;
    public Transform end;
    public Transform foot;

    Vector3 endEffectorPosition;
    Vector3 goalPosition;
    public Vector3 Goal {
        get {
            return goalPosition;
        }
    }
    public Vector3 End {
        get {
            return end.position;
        }
    }
    float EPS = 0.25f;
    float limbLength = 2f;
    public float maxDist = 0f;
    public Transform anchor;

    public float Dist {
        get {
            return Mathf.Abs( Vector3.Distance( end.position, goalPosition ) );
        }
    }
    public bool atPoint {
        get {
            return Dist < EPS;
        }
    }

    public bool Grounded {
        get {
            return atPoint && grounded;
        }
    }


    private Rigidbody body;
	// Use this for initialization
	void Start () {
        body = transform.parent.GetComponent<Rigidbody>();
        for ( int i = 1; i < segments.Length; i++ ) {
            maxDist += segments[i].length.magnitude;
        }
        end = segments[segments.Length - 1].endAnchor;
	}
    bool grounded = false;
	// Update is called once per frame
	void Update () {


	}
    void LockHeight() {
        targetHeight = Vector3.Distance( root.position, end.position );
    }
    public float targetHeight = 2.2f;
    public float k = 2.2295f;
    public float damping = 0f;
    private void FixedUpdate()
    {
        foot.transform.position = end.position;
        RaycastHit hit;
        if ( Physics.Raycast( root.position, anchor.position - root.position, out hit, Mathf.Abs( Vector3.Distance( anchor.position, root.position ) ), ~( 1 << 8 ) ) ) {
            goalPosition = hit.point;
            grounded = true;

        }
        else {
            goalPosition = anchor.position;
            grounded = false;

        }
        endEffectorPosition = end.position;
        /**/
        FABRIK();

        if ( grounded && atPoint ) {
            foot.transform.rotation = Quaternion.LookRotation( Vector3.Cross( root.right, hit.normal ), hit.normal );
        }
        else {
            foot.transform.rotation = Quaternion.LookRotation( Vector3.Cross( root.right, -( goalPosition - segments[segments.Length - 1].startAnchor.position ) ), -( goalPosition - segments[segments.Length - 1].startAnchor.position ) );
        }
        // Debug.Log( Dist );
        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            LockHeight();
        }


        if ( physics && atPoint && grounded) {
            float mid = ( end.position.z - body.transform.position.z );
            float targetExtension = Mathf.Sqrt(( targetHeight * targetHeight ) + (mid*mid));
            float targetCompression = maxDist - targetExtension;
            float f2 = k * targetCompression;
            f2 += Physics.gravity.y;
            float dist = Vector3.Distance( root.position, end.position );
            dist = maxDist - dist;
            float critDamp = 2f * Mathf.Sqrt( k * body.mass ) * damping;

            float f = ( critDamp * -body.velocity.y * 0.5f ) + k * dist;
            // float targetF = k * targetHeight;
            // if ( f2 < 0 )
            // f2 = 0f;
            f -= f2;
            downF = f2;
            finalF = f;
            if ( f < 0 )
                f = 0f;
          //  Debug.Log( "F: " + f + ", Target: " + targetF );
            body.AddForceAtPosition( f * Vector3.up, body.position );
        }
    }
    public bool physics = true;
    public float finalF = 0f;
    public float downF=0f;
    void FABRIK() {
        //while ( Mathf.Abs( Vector3.Distance(endEffectorPosition, goalPosition) ) > EPS ) {
            FinalToRoot(); // PartOne
            RootToFinal(); // PartTwo
                           // }

    }

    void FinalToRoot() {
        Vector3 localGoal = root.InverseTransformPoint( goalPosition );
     //   localGoal.x = 0f;
        
        Vector3 currentGoal = root.TransformPoint(localGoal);

        for ( int i = segments.Length - 1; i > 0;i--) {

            Limb currentLimb = segments[i];
            Vector3 forward = currentGoal - currentLimb.startAnchor.position;
            Vector3 up = -Vector3.Cross( root.right, currentGoal - currentLimb.startAnchor.position );
            Quaternion rot = Quaternion.LookRotation( forward,  up);
            if ( currentLimb.clamp ) {
                float a = Vector3.SignedAngle( segments[i-1].startAnchor.forward, currentLimb.startAnchor.forward, root.right);


                Quaternion adj;

                if ( a > currentLimb.max ) {
                    adj = segments[i - 1].startAnchor.rotation * Quaternion.AngleAxis( currentLimb.max, segments[i - 1].startAnchor.right );
                    rot = adj;
                }
                else if ( a < currentLimb.min ) {
                    adj = segments[i - 1].startAnchor.rotation * Quaternion.AngleAxis( currentLimb.min, segments[i - 1].startAnchor.right );
                    rot = adj;
                }
            }

            //TODO: Lock aligned to root axis

            currentLimb.startAnchor.rotation = rot;
            currentLimb.startAnchor.position = currentGoal - currentLimb.length;
            currentGoal = currentLimb.startAnchor.position;

        }
    }

    void RootToFinal() {
        Vector3 currentInboardPosition = root.position;
        for ( int i = 1; i < segments.Length; i++ ){
            Limb currentLimb = segments[i];
            currentLimb.startAnchor.position = currentInboardPosition;
            currentInboardPosition = currentLimb.startAnchor.position + currentLimb.length;

        }
    }

    private void OnDrawGizmos()
    {
        if ( Application.isPlaying ) {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere( goalPosition, 0.2f );
        }
    }
}
