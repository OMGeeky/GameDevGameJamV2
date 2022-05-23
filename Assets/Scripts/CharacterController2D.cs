using System;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Events;


// ReSharper disable InvalidXmlDocComment
/// <summary>
/// The script from <see cref="https://github.com/Brackeys/2D-Character-Controller"/>
/// </summary>
public class CharacterController2D : MonoBehaviour
{
    public bool active = true;
    [SerializeField] private float jumpForce = 50f;                            // Amount of force added when the player jumps.
    // [Range( 0 , .3f )] [SerializeField] private float movementSmoothing = .05f;// How much to smooth out the movement
    [SerializeField] private bool airControl = true;                           // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask whatIsGround;                           // A mask determining what is ground to the character
    [Range( 0 , 2 )] [SerializeField] private float groundedRadius = 1.5f;     // Radius of the overlap circle to determine if grounded

    private bool m_Grounded;// Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private CompositeCollider2D compositeCollider;
    private Vector3 m_Velocity = Vector3.zero;

    [Serializable] public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();
    }

    private Collider2D[] colliders = new Collider2D[10];
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxHorizontalSpeedUp = 30f;

    private void FixedUpdate()
    {
        if ( !active )
            return;

        m_Grounded = false;

        Array.Clear( colliders , 0 , colliders.Length );
        var bounds = compositeCollider.bounds;
        var size = new Vector2( bounds.size.x + groundedRadius , bounds.size.y + groundedRadius );
        Physics2D.OverlapBoxNonAlloc( point: bounds.center
                                    , size: size
                                    , angle: 0
                                    , results: colliders
                                    , layerMask: whatIsGround );

        foreach ( Collider2D c in colliders )
        {
            if ( c == null )
                continue;

            if ( c.gameObject == gameObject )
                continue;

            if ( c.transform.IsChildOf( transform ) )
                continue;

            if ( Physics2D.Distance( compositeCollider , c ).distance > groundedRadius )
                continue;

            m_Grounded = true;
        }
    }


    public void Move( float move , bool jump )
    {
        if ( !active )
            return;

        //only control the player if grounded or airControl is turned on
        if ( m_Grounded || airControl )
        {
            // Move the character by finding the target velocity
            var velocity = m_Rigidbody2D.velocity;
            Vector3 targetVelocity = new Vector2( move * speed , 0 );
            if ( velocity.x < maxHorizontalSpeedUp && velocity.x > -maxHorizontalSpeedUp )
                m_Rigidbody2D.AddForce( targetVelocity , ForceMode2D.Impulse );

            // And then smoothing it out and applying it to the character
            // m_Rigidbody2D.velocity = Vector3.SmoothDamp( velocity , targetVelocity , ref m_Velocity , movementSmoothing );
        }

        // If the player should jump...
        if ( !m_Grounded || !jump )
            return;

        // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce( new Vector2( 0f , jumpForce ) );
    }

    private void OnDrawGizmosSelected()
    {
        if ( compositeCollider == null )
            return;

        var bounds = compositeCollider.bounds;
        var size = new Vector2( bounds.size.x + groundedRadius , bounds.size.y + groundedRadius );

        Gizmos.DrawWireCube( bounds.center , size );
    }
}
