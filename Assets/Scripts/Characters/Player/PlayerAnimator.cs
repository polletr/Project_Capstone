using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public readonly int IdleHash = Animator.StringToHash("Idle");
    public  readonly int RunHash = Animator.StringToHash("Run");
    public  readonly int WalkHash = Animator.StringToHash("Walk");

    public readonly int GetHitHash = Animator.StringToHash("GetHit");
    public  readonly int DieHash = Animator.StringToHash("Die");

    public Animator animator { get; private set; }

    public void GetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
        animator.Play(IdleHash);
    }
}
