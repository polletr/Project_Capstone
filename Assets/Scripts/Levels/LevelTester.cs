using System.Collections.Generic;
using UnityEngine;

public class LevelTester : Singleton<LevelTester>
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private HubSO[] hubs;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            levelManager.EnteredHub(hubs[0]);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            levelManager.EnteredHub(hubs[1]);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            levelManager.EnteredHub(hubs[2]);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            levelManager.EnteredHub(hubs[3]);
    }

}
