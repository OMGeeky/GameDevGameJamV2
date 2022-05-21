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
    

    private InputToMovement inputToMovement;
    private CharacterController2D characterController2D;
    [SerializeField]
    private float dissolveSpeed = 0.05f;
    [SerializeField]
    private Transform spawnPosition;
    [SerializeField] private Transform parentForNew;

    public List<Character> spawnedObjects = new();
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
        inputToMovement = GetComponent<InputToMovement>();
        characterController2D = GetComponent<CharacterController2D>();
    }

    public void Init( Character prefab )
    {
        // inputToMovement.enabled = true;
        // characterController2D.enabled = true;
        this.root = prefab;
        isActivePlayer = true;
    }

    public void KillSelf()
    {
        if ( math.distance( transform.position , Vector3.zero ) < 2 )
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
        var newPlayer = Instantiate( root , spawnPosition.position  , Quaternion.identity, parentForNew );
        newPlayer.Init( root );
        root.spawnedObjects.Add( newPlayer );
    }

    public void Dissolve()
    {
        var target = FindNearestSpawned();
        if ( target == null )
            return;

        target.DissolveSelf();
    }

    public void DissolveSelf() { StartCoroutine( DissolveRoutine() ); }

    private Character FindNearestSpawned()
    {
        Character closest = null;
        float closestDistance = float.MaxValue;
        foreach ( Character spawnedObject in root.spawnedObjects )
        {
            if ( spawnedObject == this )
                continue;

            var distance = math.distance( spawnedObject.transform.position , transform.position );
            if ( distance > 2.5f )
                continue;

            if ( closest != null
              && distance < closestDistance )
                continue;

            closest = spawnedObject;
            closestDistance = distance;
        }

        return closest;
    }

    private IEnumerator DissolveRoutine()
    {
        var mat = GetComponent<Renderer>().material;
        float value = 0;
        while ( value < 1 )
        {
            value += dissolveSpeed;
            mat.SetFloat( "_DissolveAmount" , value );
            yield return null;
        }

        root.spawnedObjects.Remove( this );
        Destroy( gameObject );
    }
}
