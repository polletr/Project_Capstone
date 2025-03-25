using System;
using UnityEngine;

public class WhiteRoomPuzzle : Singleton<WhiteRoomPuzzle>
{
    public GlobalEventSO openDoor;

    [SerializeField] private LightController[] rooms = new LightController[7];
    [SerializeField] private bool[] outterLightCheck = new bool[7];
    

    private bool[] innerLightCheck = new bool[7];

    private void Start()
    {
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
        openDoor.OnTriggerGlobalEvent.Invoke();
        //Unlock Exit Door
        //Make sound for the unlock
        //Make all lights flicker
    }
}
