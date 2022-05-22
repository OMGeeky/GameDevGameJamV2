using System;

using UnityEngine;
// ReSharper disable RedundantCheckBeforeAssignment


[RequireComponent( typeof(CharacterController2D) , typeof(Character) )]
public class InputToMovement : MonoBehaviour
{
    private CharacterController2D controller2D;
    private Character character;

    private void OnEnable()
    {
        controller2D = GetComponent<CharacterController2D>();
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    [SerializeField] private float interactTimer;
    [SerializeField] private float interactThreshold = 1;
    [SerializeField] private float reloadTimer;
    [SerializeField] private float reloadThreshold = 1;
    [SerializeField] private float attachTimer;
    [SerializeField] private float attachThreshold = 1;

    private void Update()
    {
        if(controller2D.active != character.isActivePlayer)
            controller2D.active = character.isActivePlayer;
        
        if ( !character.isActivePlayer )
            return;

        var horizontal = Input.GetAxis( "Horizontal" );
        var jump = Input.GetButton( "Jump" );

        if ( horizontal != 0 || jump )
        {
            controller2D.Move( horizontal , jump );
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
            character.Eat();
            reloadTimer = 0;
        }

        var attach = Input.GetButton( "Attach" );
        if ( attach )
        {
            attachTimer += Time.deltaTime;
        }
        else
        {
            attachTimer = 0;
        }

        if ( attachTimer >= attachThreshold )
        {
            character.Attach();
            attachTimer = 0;
        }

        Console.WriteLine();
    }
}
