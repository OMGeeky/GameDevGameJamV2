using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class DissolveCollidersExceptPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D( Collider2D col )
    {
        if ( !col.TryGetComponent( out Character character ) ) 
            return;
        
        if ( character.isActivePlayer )
            return;

        character.DissolveSelf();
    }
}
