using System.Collections.Generic;
using UnityEngine;
namespace Tutorial{
    
public class TutorialTrigger : MonoBehaviour
 {
     public List<TutorialData> tutorialDataList;
     private int currentTutorialIndex = 0;
 
     private void OnEnable()
     {
         if (tutorialDataList.Count > 0)
         {
             tutorialDataList[currentTutorialIndex].OnEndTutorial += OnEndTutorial;
         }
     }
 
     private void OnDisable()
     {
         if (tutorialDataList.Count > 0 && tutorialDataList.Count > currentTutorialIndex)
         {
             tutorialDataList[currentTutorialIndex].OnEndTutorial -= OnEndTutorial;
         }
     }
 
     public void InvokeEvent()
     {

         if (tutorialDataList.Count > 0)
         {
            Debug.Log("Tutorial Triggered");
           TutorialManager.Instance.SetTutorial(tutorialDataList[currentTutorialIndex]);
         }
     }
 
     public void OnEndTutorial()
     {
         //tutorialDataList[currentTutorialIndex].OnEndTutorial -= OnEndTutorial;
         currentTutorialIndex++;

         if (currentTutorialIndex >= tutorialDataList.Count)
         {
                currentTutorialIndex = 0;
                Debug.Log("Wipe");
                TutorialManager.Instance.WipeScreen();
                return;
         }
         else
         {
            //tutorialDataList[currentTutorialIndex].OnEndTutorial += OnEndTutorial;
            TutorialManager.Instance.SetNextTutorial(tutorialDataList[currentTutorialIndex]);
         }

        }
 
     private void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag("Player"))
         {
            InvokeEvent();
            GetComponent<Collider>().enabled = false;
         }
     }
 
}
}