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
    [SerializeField] private int maxPatients = 4;
    
    private int currentPatients;
    private readonly List<int> activePatients = new();
    private readonly List<int> playerChoice = new();

    [Header("Events")] public UnityEvent OnPuzzleSolved;

    public UnityEvent OnPuzzleFailed;

    private void Start()
    {
        currentPatients = Random.Range(minPatients, maxPatients); 
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

        while (activePatients.Count < currentPatients)
        {
            var randomPatient = Random.Range(0, patients.Length);

            if (activePatients.Contains(randomPatient)) continue;

            activePatients.Add(randomPatient);
            patients[randomPatient].SetActive(true);
        }
    }
}