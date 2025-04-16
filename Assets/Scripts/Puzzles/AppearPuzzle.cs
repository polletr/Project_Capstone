using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppearPuzzle : MonoBehaviour
{
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnPuzzleMistaken;
    public UnityEvent OnPuzzleFailed;

    [SerializeField] private RevealableObject[] revealableObjects;
    [SerializeField] private bool[] objectsToReveal;
    [SerializeField] private int maxMistakes = 2;

    [SerializeField] private GameObject darkTrigger;
    [SerializeField] private float duration = 1;
    [SerializeField] private List<Transform> darkTriggerPos = new();

    private bool[] innerLightCheck;
    private int numofMistakes;

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
            Debug.Log("Object" + obj.name + ": " + obj.IsRevealed);
            innerLightCheck[index] = obj.IsRevealed;
            obj.ApplyOutline = false;
        }

        for (int i = 0; i < revealableObjects.Length; i++)
        {
            if (objectsToReveal[i] == innerLightCheck[i])
                continue;
            if (objectsToReveal[i] && !innerLightCheck[i])
                continue;

            Debug.Log("Mistake!");

            obj.UnRevealObj();
            obj.ApplyOutline = true;
            innerLightCheck[index] = false;

            MoveDarkness();
            numofMistakes++;
            // increase darkness
            OnPuzzleMistaken.Invoke();
            if (numofMistakes >= maxMistakes)
                LosePuzzle();
            return;
        }

        for (var i = 0; i < revealableObjects.Length; i++)
        {
            if (objectsToReveal[i] != innerLightCheck[i])
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

    private void MoveDarkness()
    {
        if (numofMistakes >= darkTriggerPos.Count) return;
        if (darkTriggerPos[numofMistakes] == null) return;

        darkTrigger.transform.DOMove(darkTriggerPos[numofMistakes].position, duration).SetEase(Ease.InOutSine);
    }
}