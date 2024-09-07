using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    public Vector3 StartPos { get; private set; }

    [HideInInspector]
    public EnemyBaseState currentState;

    [HideInInspector]
    public NavMeshAgent agent;

    public float PatrolRange;
    public float MaxIdleTime;
    public float MinIdleTime;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        StartPos = transform.position;
        ChangeState(new EnemyPatrolState());
    }

    private void FixedUpdate() => currentState?.StateFixedUpdate();
    private void Update() => currentState?.StateUpdate();

    #region ChangeState
    public void ChangeState(EnemyBaseState newState)
    {
        StartCoroutine(WaitFixedFrame(newState));
    }

    private IEnumerator WaitFixedFrame(EnemyBaseState newState)
    {

        yield return new WaitForFixedUpdate();
        currentState?.ExitState();
        currentState = newState;
        currentState.enemy = this;
        currentState.EnterState();

    }
    #endregion

}
