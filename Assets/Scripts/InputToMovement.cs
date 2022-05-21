using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[RequireComponent( typeof(CharacterController2D) , typeof(Character) )]
public class InputToMovement : MonoBehaviour
{
    private CharacterController2D controller2D;
    private Character character;

    void OnEnable()
    {
        controller2D = GetComponent<CharacterController2D>();
        character = GetComponent<Character>();
        // interactTimer = 0;
    }

    // Update is called once per frame
    [SerializeField]
    private float interactTimer = 0;
    [SerializeField]
    private float interactThreshold = 3;
    [SerializeField]
    private float reloadTimer = 0;
    [SerializeField]
    private float reloadThreshold = 1;

    void Update()
    {
        if ( !character.isActivePlayer )
            return;
        // if ( controller2D == null )
            // return;

        var horizontal = Input.GetAxis( "Horizontal" );
        var jump = Input.GetButton( "Jump" );

        // var crouch = Input.GetButton( "Crouch" );
        if ( horizontal != 0 || jump )
        {
            controller2D.Move( horizontal , false , jump );
        }

        var interact = Input.GetButton( "Interact" );
        if ( interact )
        {
            interactTimer += Time.deltaTime;
        }
        else
        {
            interactTimer = 0;
        }

        if ( interactTimer >= interactThreshold )
        {
            character.KillSelf();
            interactTimer = 0;
        }
        var reload = Input.GetButton( "Reload" );
        if ( reload )
        {
            reloadTimer += Time.deltaTime;
        }
        else
        {
            reloadTimer = 0;
        }

        if ( reloadTimer >= reloadThreshold )
        {
            character.Dissolve();
            reloadTimer = 0;
        }
    }
}
