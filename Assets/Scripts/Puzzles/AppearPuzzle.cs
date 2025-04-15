using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AppearPuzzle : MonoBehaviour
{
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnPuzzleFailed;

    [SerializeField] private RevealableObject[] revealableObjects;
    [SerializeField] private bool[] objectsToReveal;

    private bool[] innerLightCheck;

    private void Start()
    {
        int length = revealableObjects.Length;

        innerLightCheck = new bool[length];
    }

    public void CheckPuzzle(RevealableObject obj)
    {
        int index = Array.IndexOf(revealableObjects, obj);
        if (index != -1)
        {
            innerLightCheck[index] = obj.IsRevealed;
            Debug.Log($"CheckPuzzle: {obj.name} is {innerLightCheck[index]}");
        }

        for (int i = 0; i < revealableObjects.Length; i++)
        {
            if (innerLightCheck[i] == objectsToReveal[i])
                continue;
            if (objectsToReveal[i] && !innerLightCheck[i])
                return;
            
            LosePuzzle();
            return;
        }

        WinPuzzle();
    }

    void WinPuzzle()
    {
        Debug.Log("Win Puzzle!");
        OnPuzzleSolved.Invoke();
    }

    void LosePuzzle()
    {
        Debug.Log("Lost Puzzle!");
        OnPuzzleFailed.Invoke();
    }
}