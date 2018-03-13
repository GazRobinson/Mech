using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboRig : MonoBehaviour {

    public Transform LeftAnchor;
    public Transform RightAnchor;

    public LimbTest LeftLeg;
    public LimbTest RightLeg;

    public Vector3 DesiredPosition;
    public float DesiredHeight = 2.2f;
    private Rigidbody rb;
    public bool doMove = false;

    public float damp = 1f;
    public float k = 8f;

    public SpringJoint L_Spring;
    public SpringJoint R_Spring;

    public Rigidbody LFoot, RFoot, Head;
    public float compressionMag = 10f;
    public float compressionDamp = 0.5f;
    public ConfigurableJoint lLeg, rLeg;

    private Vector3 CoM;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
    float reactive = 0f;
    void Update() {


        //Debug.Log( "Total: " + totalF + ", React: " + reactive );
        doForceL = Input.GetKey( KeyCode.Q );
        doForceR = Input.GetKey( KeyCode.E );

        if ( Input.GetKeyDown( KeyCode.W ) ) {
            LFoot.AddForce( new Vector3( 0f, 2f, 1f ) * 2f, ForceMode.Impulse );
        }
        if ( Input.GetKeyDown( KeyCode.UpArrow ) ) {
            RFoot.AddForce( new Vector3( 0f, 2f, 1f ) * 2f, ForceMode.Impulse );
        }

        if ( Input.GetKeyDown( KeyCode.S ) ) {
            LFoot.AddForce( new Vector3( 0f, 2f, -1f ) * 2f, ForceMode.Impulse );
        }
        if ( Input.GetKeyDown( KeyCode.DownArrow ) ) {
            RFoot.AddForce( new Vector3( 0f, 2f, -1f ) * 2f, ForceMode.Impulse );
        }

        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            rb.AddForce( new Vector3( 0f, 1f, 0f ) * 15f, ForceMode.Impulse );
        }

        CoM = rb.position * rb.mass;
        CoM += Head.position * Head.mass;
        CoM /= (Head.mass + rb.mass);
    }
    bool doForceL = false;
    bool doForceR = false;
	// Update is called once per frame
	void FixedUpdate () {
        if ( LeftLeg.Grounded || RightLeg.Grounded ) {
            DesiredPosition = ( LeftLeg.End + RightLeg.End ) / 2f;
            DesiredPosition += Vector3.up * DesiredHeight;

            MoveToXZ();
            //MoveToY();
        }
        if ( doForceL ) {

            Vector3 f = ( LeftLeg.root.position - LFoot.position ).normalized * compressionMag;
            LFoot.AddForce( f );
            totalF = f.magnitude - (LFoot.mass * Physics.gravity.magnitude);


            float d = Vector3.Distance( lLeg.transform.position, lLeg.connectedBody.position );
            d = lLeg.connectedAnchor.magnitude - d;
            reactive = lLeg.yDrive.positionSpring * d;

            lLeg.GetComponent<Rigidbody>().AddForce( -f);
        }

        if ( doForceR ) {

            Vector3 f = ( RightLeg.root.position - RFoot.position ).normalized * compressionMag;
            RFoot.AddForce( f );
            totalF = f.magnitude - ( RFoot.mass * Physics.gravity.magnitude );


            float d = Vector3.Distance( rLeg.transform.position, rLeg.connectedBody.position );
            d = rLeg.connectedAnchor.magnitude - d;
            reactive = rLeg.yDrive.positionSpring * d;

            rLeg.GetComponent<Rigidbody>().AddForce( -f );
        }
	}
    float totalF = 0f;
    public void MoveToXZ() {
        
        float critDamp = 2f * Mathf.Sqrt( k * rb.mass ) * damp;


        Vector2 des = new Vector2( DesiredPosition.x, DesiredPosition.z );
        Vector2 pos = new Vector2( rb.position.x, rb.position.z );
        Vector2 direction = des - pos;
        float dist = direction.magnitude;
        float f;
        Vector2 Velocity = new Vector2( rb.velocity.x, rb.velocity.z );
        // f =  4.905f + ( critDamp * -body.velocity.y * 0.5f ) + ( x * spring ) ;
        Vector2 dampForce = ( critDamp * -Velocity );
        f = (k * dist);
        // Debug.Log( f );
        Vector3 dir = new Vector3( direction.x, 0f, direction.y );
        Vector3 dampForce3 = new Vector3( dampForce.x, 0f, dampForce.y );
        forceVec = f * dir.normalized;

        if(doMove)
            rb.AddForceAtPosition( dampForce3 + forceVec, rb.position );
    }
    Vector3 forceVec = Vector3.zero;
    public void MoveToY() {
        LeftLeg.targetHeight = DesiredPosition.y;
        RightLeg.targetHeight = DesiredPosition.y;
    }

    private void OnDrawGizmos()
    {
        if ( Application.isPlaying ) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere( DesiredPosition, 0.2f );
            Gizmos.color = Color.blue;
            Gizmos.DrawLine( DesiredPosition, DesiredPosition + forceVec );
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(  CoM, 0.5f );

        }
    }
}
