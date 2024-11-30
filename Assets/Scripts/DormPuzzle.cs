using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class DormPuzzle : Singleton<DormPuzzle>
{
    [SerializeField] private GameObject[] patients;
    [SerializeField] private ClipBoard[] clipBoards;
    [SerializeField] private int minPatients = 2;

    private int maxPatients;
    private readonly List<int> activePatients = new();
    private readonly List<int> playerChoice = new();

    [Header("Events")] public UnityEvent OnPuzzleSolved;

    public UnityEvent OnPuzzleFailed;

    private void Start()
    {
        maxPatients = Random.Range(minPatients, patients.Length); 
        PickRandomPattern();
    }

    public void CheckPuzzle(int patientIndex)
    {
        if (activePatients.Contains(patientIndex))
        {
            playerChoice.Add(patientIndex);
            Debug.Log("Correct Choice");

            if (playerChoice.Count != activePatients.Count) return;

            OnPuzzleSolved.Invoke();
            Debug.Log("Puzzle Solved");
        }
        else
        {
            Debug.Log("Wrong Choice");
            OnPuzzleFailed.Invoke();
            clipBoards[patientIndex].ShowDeceasedText();
        }
    }

    private void PickRandomPattern()
    {
        foreach (var patient in patients)
        {
            patient.SetActive(false);
        }

        playerChoice.Clear();
        activePatients.Clear();

        while (activePatients.Count < maxPatients)
        {
            var randomPatient = Random.Range(0, patients.Length);

            if (activePatients.Contains(randomPatient)) continue;

            activePatients.Add(randomPatient);
            patients[randomPatient].SetActive(true);
        }
    }
}