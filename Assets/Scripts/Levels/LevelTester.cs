using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTester : MonoBehaviour
{
    [SerializeField] private string[] levels;
    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadSceneAsync(levels[0], LoadSceneMode.Additive);
       if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadSceneAsync(levels[1], LoadSceneMode.Additive);
       if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadSceneAsync(levels[2], LoadSceneMode.Additive);
       if (Input.GetKeyDown(KeyCode.Alpha4))
            SceneManager.LoadSceneAsync(levels[3], LoadSceneMode.Additive);

      
    }

}
