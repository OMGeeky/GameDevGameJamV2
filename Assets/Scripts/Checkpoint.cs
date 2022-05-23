using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMGeeky
{
    public class Checkpoint : MonoBehaviour
    {
        private Animator animator;
        private static readonly int IsActive = Animator.StringToHash( "IsActive" );

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        public void SetInactive()
        {
            if(animator != null)
                animator.SetBool( IsActive, false );
        }

        private void OnTriggerEnter2D( Collider2D other )
        {
            if ( other.TryGetComponent( out Character character ) )
            {
                character.root.spawnPosition.SetInactive();
                character.root.spawnPosition = this;
                animator.SetBool( IsActive, true );
            }
        }
    }
}
