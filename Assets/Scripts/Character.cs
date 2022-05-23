using System;
using System.Collections;
using System.Collections.Generic;

using OMGeeky;

using Unity.Mathematics;

using UnityEngine;


[RequireComponent( typeof(SpriteRenderer) )]
public class Character : MonoBehaviour
{

#region Components

    private new CompositeCollider2D collider;
    private SpriteRenderer spriteRenderer;

#endregion

#region Inspector

    [SerializeField] public bool isActivePlayer;
    [SerializeField] private Sprite aliveSprite;
    [SerializeField] private Sprite deadSprite;
    [SerializeField] private float dissolveSpeed = 0.05f;
    [SerializeField] private Transform parentForNew;
    [SerializeField] private LayerMask findLayerMask;
    [SerializeField] private float eatRange = 2.5f;
    [SerializeField] private float attachRange = .5f;

#endregion

#region Other public data

    public Checkpoint spawnPosition;
    public Character root;

#endregion

#region Events

    public event Action<Character> Killed;
    public event Action<Character> Spawned;

#endregion


    public List<Character> spawnedObjects = new();
    private static readonly int DissolveAmount = Shader.PropertyToID( "_DissolveAmount" );
    private readonly Collider2D[] findColliders = new Collider2D[50];

    private void Start()
    {
        // yield return new WaitForSeconds( 0.5f );

        if ( root == null )
        {
            root = this;
            SpawnNew();
        }

        // if ( spawnPosition == null )
            // spawnPosition = transform;
    }

    private void OnEnable()
    {
        collider = GetComponent<CompositeCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = aliveSprite;
        if ( root != null )
        {
            Spawned -= OnSpawned;
            Spawned += OnSpawned;
        }
    }

    private void OnSpawned( Character c ) { root.CallSpawned( c ); }

    private void CallSpawned( Character character ) { Spawned?.Invoke( character ); }

    private void Init( Character prefab )
    {
        // inputToMovement.enabled = true;
        // characterController2D.enabled = true;
        root = prefab;
        isActivePlayer = true;
        Killed += character => root.spawnedObjects.Remove( character );
    }

    public void KillSelf()
    {
        if ( math.distance( transform.position , spawnPosition.transform.position ) < 2 )
        {
            Debug.Log( "Cant restart here, too near to spawn" );
            return;
        }

        // characterController2D.enabled = false;
        // inputToMovement.enabled = false;
        isActivePlayer = false;
        SpawnNew();
        spriteRenderer.sprite = deadSprite;

        // newPlayer.GetComponent<InputToMovement>().enabled = true;
        // newPlayer.GetComponent<CharacterController2D>().enabled = true;
    }

    private void SpawnNew()
    {
        var newPlayer = Instantiate( root , root.spawnPosition.transform.position , Quaternion.identity , parentForNew );
        newPlayer.Init( root );
        root.spawnedObjects.Add( newPlayer );
        Spawned?.Invoke( newPlayer );
    }

    public void Eat()
    {
        var target = FindNearestSpawned( eatRange );
        if ( target == null )
            return;

        //TODO: buff player after eating
        // var size = target.transform.transform.localScale;
        // size.z = 0;

        // var localScale = transform.localScale;
        // var finalSize = MathHelpers.AddBoxAreaToBox( (Vector2) size , (Vector2) localScale );

        // transform.localScale += size;
        // localScale = new Vector3( finalSize.x , finalSize.y , localScale.z );
        // transform.localScale = localScale;
        target.DissolveSelf();
    }

    public void DissolveSelf() { StartCoroutine( DissolveRoutine() ); }

    private Character FindNearestSpawned( float range )
    {
        Character closest = null;
        float closestDistance = float.MaxValue;
        var t = transform;
        var localScale = t.localScale;
        Array.Clear( findColliders , 0 , findColliders.Length );
        var bounds = collider.bounds;
        var size = new Vector2( bounds.size.x + range , bounds.size.y + range );
        Physics2D.OverlapBoxNonAlloc( point: t.position
                                    , size: size
                                    , angle: 0
                                    , results: findColliders
                                    , layerMask: findLayerMask );

        foreach ( Collider2D coll in findColliders )
        {
            if ( coll == null )
                continue;

            if ( !coll.TryGetComponent( out Character character ) )
                continue;

            if ( character == this )
                continue;

            var distance = Physics2D.Distance( collider , coll ).distance;

            if ( distance >= closestDistance )
                continue;

            closest = character;
            closestDistance = distance;
        }

        /*
        foreach ( Character spawnedObject in root.spawnedObjects )
        {
            if ( spawnedObject == this )
                continue;


            var distance = math.distance( spawnedObject.transform.position , transform.position );
            if ( distance > range )
                continue;

            if ( closest != null
              && distance < closestDistance )
                continue;

            closest = spawnedObject;
            closestDistance = distance;
        }
        */

        return closest;
    }

    private IEnumerator DissolveRoutine()
    {
        var mat = GetComponent<Renderer>().material;
        float value = 0;
        while ( value < 1 )
        {
            value += dissolveSpeed;
            mat.SetFloat( DissolveAmount , value );
            yield return null;
        }

        Killed?.Invoke( this );
        Destroy( gameObject );
    }

    public void Attach()
    {
        var target = FindNearestSpawned( attachRange );
        if ( target == null )
            return;

        target.transform.parent = transform;

        // var targetInput = target.GetComponent<InputToMovement>();
        target.isActivePlayer = false;

        RemovePlayerComponentsFromTarget( target );
    }

    private static void RemovePlayerComponentsFromTarget( Character target )
    {
        var targetRb = target.GetComponent<Rigidbody2D>();
        var targetCol = target.GetComponent<CompositeCollider2D>();

        // var targetController = target.GetComponent<CharacterController2D>();
        // var targetInput = target.GetComponent<InputToMovement>();
        // Destroy( targetInput );
        // Destroy( targetController );
        Destroy( targetCol );
        Destroy( targetRb );
    }
}
