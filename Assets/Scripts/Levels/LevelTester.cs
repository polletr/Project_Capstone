using System.Collections.Generic;
using UnityEngine;

public class LevelTester : Singleton<LevelTester>
{
    [SerializeField] private LevelManager levelManager;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            levelManager.AddNextHub(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            levelManager.AddNextHub(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            levelManager.AddNextHub(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            levelManager.AddNextHub(4);
    }

}
