using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;


[RequireComponent( typeof(Renderer) )]
public class Character : MonoBehaviour
{
    public bool isActivePlayer;

    public Character root;

#region Components

    private new Collider2D collider;

#endregion

#region Inspector

    [SerializeField] private float dissolveSpeed = 0.05f;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform parentForNew;
    [SerializeField] private LayerMask findLayerMask;
    [SerializeField] private float eatRange = 2.5f;
    [SerializeField] private float attachRange = .5f;

#endregion

#region Events

    public event Action<Character> Killed;
    public event Action<Character> Spawned;

#endregion


    public List<Character> spawnedObjects = new();
    private static readonly int DissolveAmount = Shader.PropertyToID( "_DissolveAmount" );

    private IEnumerator Start()
    {
        yield return new WaitForSeconds( 0.5f );

        if ( root == null )
        {
            root = this;
            SpawnNew();
        }

        if ( spawnPosition == null )
            spawnPosition = transform;
    }

    private void OnEnable()
    {
        GetComponent<InputToMovement>();
        GetComponent<CharacterController2D>();
        collider = GetComponent<Collider2D>();
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
        if ( math.distance( transform.position , spawnPosition.position ) < 2 )
        {
            Debug.Log( "Cant restart here, too near to spawn" );
            return;
        }

        // characterController2D.enabled = false;
        // inputToMovement.enabled = false;
        isActivePlayer = false;
        SpawnNew();

        // newPlayer.GetComponent<InputToMovement>().enabled = true;
        // newPlayer.GetComponent<CharacterController2D>().enabled = true;
    }

    private void SpawnNew()
    {
        var newPlayer = Instantiate( root , spawnPosition.position , Quaternion.identity , parentForNew );
        newPlayer.Init( root );
        root.spawnedObjects.Add( newPlayer );
        Spawned?.Invoke( newPlayer );
    }

    public void Eat()
    {
        var target = FindNearestSpawned( eatRange );
        if ( target == null )
            return;

        var size = target.transform.transform.localScale;
        size.z = 0;
        transform.localScale += size;
        target.DissolveSelf();
    }

    public void DissolveSelf() { StartCoroutine( DissolveRoutine() ); }

    private Character FindNearestSpawned( float range )
    {
        Character closest = null;
        float closestDistance = float.MaxValue;
        var t = transform;
        var localScale = t.localScale;
        Collider2D[] colliders = new Collider2D[50];
        Physics2D.OverlapBoxNonAlloc( point: t.position
                                    , size: new Vector2( localScale.x + range , localScale.y + range )
                                    , angle: t.rotation.eulerAngles.z
                                    , results: colliders
                                    , layerMask: findLayerMask );

        foreach ( Collider2D coll in colliders )
        {
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
        RemovePlayerComponentsFromTarget( target );
    }

    private static void RemovePlayerComponentsFromTarget( Character target )
    {
        var targetRb = target.GetComponent<Rigidbody2D>();
        var targetCol = target.GetComponent<CompositeCollider2D>();
        var targetController = target.GetComponent<CharacterController2D>();
        var targetInput = target.GetComponent<InputToMovement>();
        Destroy( targetInput );
        Destroy( targetController );
        Destroy( targetCol );
        Destroy( targetRb );
    }
}
