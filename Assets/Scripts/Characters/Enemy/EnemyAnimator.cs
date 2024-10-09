using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public readonly int IdleHash = Animator.StringToHash("Idle");
    public readonly int ChaseHash = Animator.StringToHash("Chase");
    public readonly int WalkHash = Animator.StringToHash("Walk");
    public readonly int AttackHash = Animator.StringToHash("Attack");
    public readonly int AnticipationHash = Animator.StringToHash("Attack");
    public readonly int StunHash = Animator.StringToHash("Stun");

    [field: SerializeField] public float animationCrossFade { get; private set; }

    public Animator animator { get; private set; }

    public void GetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }
}
