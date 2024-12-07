using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class CursedPuzzle : MonoBehaviour
{
    [Header("Objects Per Puzzle")] [SerializeField]
    private int ObjectsCount = 2;

    [Header("Timer")] [SerializeField] private float timeToSolve = 10f;
    [SerializeField] private bool useTimer = true;

    [Header("Events")] public UnityEvent OnPuzzleMidway;
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnPuzzleFailed;


    private CountdownTimer timer;

    private void Start()
    {
        timer = new CountdownTimer(timeToSolve);
    }

    public void ObjectDisappeared()
    {
        ObjectsCount--;

        if (ObjectsCount == 0)
        {
            OnPuzzleSolved.Invoke();
        }
    }

    private void Update()
    {
        if (useTimer)
        {
            timer.Tick(Time.deltaTime);
            if (timer.Progress == 0.5f)
            {
                OnPuzzleMidway.Invoke();
            }

            if (timer.IsFinished)
            {
                OnPuzzleFailed.Invoke();
            }
        }
    }

    public void StartTimer()
    {
        timer.Start();
    }
}