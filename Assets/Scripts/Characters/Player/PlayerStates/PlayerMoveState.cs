using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerController playerController) : base(playerController)
    {
    }
    
    public override void EnterState()
    {
        if (PlayerAnimator.animator.isActiveAndEnabled)
            PlayerAnimator.animator.Play(Player.playerAnimator.IdleHash);
    }

    public override void ExitState()
    {
    }

    public override void StateFixedUpdate()
    {
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Ray ray = new Ray(Player.PlayerCam.transform.position, Player.PlayerCam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Player.Settings.MaxEnemyDistance))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out ShadowEnemy enemy))
            {
                Player.AddEnemyToChaseList(enemy);
            }
        }
    }


    public override void HandleInteract()
    {
        if (Player.interactableObj != null)
            Player.ChangeState(Player.InteractState);
    }

    public override void HandleRecharge()
    {
        Player.flashlight.ZeroOutBattery();
        Player.ChangeState(Player.RechargeState);
    }
}