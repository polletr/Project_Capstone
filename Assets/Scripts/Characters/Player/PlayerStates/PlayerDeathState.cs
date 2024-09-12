using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public override void EnterState()
    {
        Debug.Log("Player Dead");
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {

    }

    public override void HandleMovement(Vector2 dir)
    {

    }


    public override void HandleGetHit()
    {

    }

    public override void HandleDeath()
    {

    }

}
