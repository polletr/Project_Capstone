using System.Collections;
using TMPro;
using UnityEngine;

namespace Tutorial
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        public TutorialData CurrentTutorial { get; set; }

        [SerializeField] private TextMeshProUGUI tutorialText;

        private Coroutine tutorialCoroutine;

        public void SetTutorial(TutorialData tutorialData)
        {
            CurrentTutorial = tutorialData;
            tutorialText.text = CurrentTutorial.Message;
            CurrentTutorial.OnDoTutorial += CurrentTutorial.End;
            CurrentTutorial.OnEndTutorial += WipeScreen;
        }

        public void SetNextTutorial(TutorialData tutorialData)
        {
            if (tutorialCoroutine != null) StopCoroutine(tutorialCoroutine);

            tutorialCoroutine = StartCoroutine(NextTutorial(tutorialData));
        }

        private IEnumerator NextTutorial(TutorialData tutorialData)
        {
            yield return new WaitForSeconds(1f);
            SetTutorial(tutorialData);
        }

        private void WipeScreen()
        {
            CurrentTutorial.OnEndTutorial -= WipeScreen;
            tutorialText.text = "Done!";
            CurrentTutorial = null;
        }
    }
}