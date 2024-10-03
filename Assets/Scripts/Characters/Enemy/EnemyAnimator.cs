using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public readonly int IdleHash = Animator.StringToHash("Idle");
    public readonly int ChaseHash = Animator.StringToHash("Chase");
    public readonly int WalkHash = Animator.StringToHash("Walk");
    public readonly int AttackHash = Animator.StringToHash("Attack");
    public readonly int GetHitHash = Animator.StringToHash("GetHit");
    public readonly int DieHash = Animator.StringToHash("Die");


    [field: SerializeField] public Animator animator { get; private set; }

    public void GetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }
}
