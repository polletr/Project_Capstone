using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class CursedPuzzle : MonoBehaviour
{
    [Header("Objects Per Puzzle")] [SerializeField]
    private int ObjectsCount = 2;

    [Header("Timer")] [SerializeField] private float timeToSolve = 10f;
    [SerializeField] private bool useTimer = true;

    [Header("Events")]

    public UnityEvent OnPuzzle75;
    public UnityEvent OnPuzzle50;
    public UnityEvent OnPuzzle25;
    public UnityEvent OnPuzzle10;


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

        if (useTimer && timer.IsRunning)
        {
            timer.Tick(Time.deltaTime);
            // Check if progress crosses specific thresholds

            if (timer.Progress > 0.24f && timer.Progress <= 0.25f)
            {
                OnPuzzle75.Invoke();
            }
            if (timer.Progress > 0.49f && timer.Progress <= 0.5f)
            {
                OnPuzzle50.Invoke();
            }
            if (timer.Progress > 0.74f && timer.Progress <= 0.75f)
            {
                OnPuzzle25.Invoke();
            }
            if (timer.Progress > 0.89f && timer.Progress <= 0.90f)
            {
                OnPuzzle10.Invoke();
            }

            if (timer.IsFinished && ObjectsCount > 0)
            {
                OnPuzzleFailed.Invoke();
                timer.Stop();
            }
        }
    }

    public void StartTimer()
    {
        timer.Start();
    }
}