using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public readonly int IdleHash = Animator.StringToHash("Idle");
    public readonly int RechargeHash = Animator.StringToHash("Recharge");

    public Animator animator { get; private set; }

    public void GetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }
}
