using System;
using UnityEngine;
using UnityEngine.Events;

public class WhiteRoomPuzzle : Singleton<WhiteRoomPuzzle>
{
    public UnityEvent OnPuzzleSolved;

    [SerializeField] private LightController[] rooms;
    [SerializeField] private bool[] outterLightCheck;
    

    private bool[] innerLightCheck;

    private void Start()
    {
        int length = rooms.Length;

        innerLightCheck = new bool[length];

        for (int i = 0; i < innerLightCheck.Length; i++)
        {
            innerLightCheck[i] = true;
        }
    }

    public void CheckPuzzle(LightController light)
    {
        int index = Array.IndexOf(rooms, light);
        if (index != -1)
        {
            innerLightCheck[index] = light.lightSource.enabled;
        }

        Debug.Log(innerLightCheck[index]);

        for (int i = 0; i < rooms.Length; i++)
        {
            if (innerLightCheck[i] != outterLightCheck[i])
                return;
        }

        WinPuzzle();
    }

    void WinPuzzle()
    {
        Debug.Log("Win Puzzle!");
        OnPuzzleSolved.Invoke();
        //Make all lights flicker
    }
}
