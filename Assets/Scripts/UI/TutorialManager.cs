using TMPro;
using UnityEngine;
using Utilities;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private float _tutorialTime;
    [SerializeField] private TextMeshProUGUI _tutorialtext;

    public GameEvent Event;
    public GlobalEventSO stunTutorial;
    public GlobalEventSO moveTutorial;
    public GlobalEventSO flashlightOnTutorial;
    public GlobalEventSO flashlightOffTutorial;
    private CountdownTimer _countdownTimer;

    private void Awake()
    {
        _countdownTimer = new CountdownTimer(_tutorialTime);
    }

    private void OnEnable()
    {
        stunTutorial.OnTriggerGlobalEvent += StunText;
        moveTutorial.OnTriggerGlobalEvent += MoveText;
        flashlightOnTutorial.OnTriggerGlobalEvent += FlashlightText;
        flashlightOffTutorial.OnTriggerGlobalEvent += FlashlightOffText;
        Event.SetTutorialText += SetText;
        Event.SetTutorialTextTimer += SetTextTimer;


    }

    private void OnDisable()
    {
        stunTutorial.OnTriggerGlobalEvent -= StunText;
        moveTutorial.OnTriggerGlobalEvent -= MoveText;
        flashlightOnTutorial.OnTriggerGlobalEvent -= FlashlightText;
        flashlightOffTutorial.OnTriggerGlobalEvent -= FlashlightOffText;
        Event.SetTutorialText -= SetText;
        Event.SetTutorialTextTimer -= SetTextTimer;
    }

    private void Update()
    {
        _countdownTimer.Tick(Time.deltaTime);
        if (_countdownTimer.IsFinished)
        {
            _tutorialtext.text = "";
            _countdownTimer.Stop();
            _countdownTimer.Reset();

        }
    }
    public void SetTextTimer(string text)
    {
        _countdownTimer.Reset();
        _countdownTimer.Start();
        _tutorialtext.text = text;
    }

    public void SetText(string text)
    {
        _tutorialtext.text = text;
    }

    public void StunText()
    {
        SetTextTimer("Left click to stun");
    }
    public void MoveText()
    {
        SetTextTimer("Hold down left mouse button to move highlighted objects");
    }
    public void FlashlightText()
    {
        SetTextTimer("Press F to turn ON flashlight");
    }

    public void FlashlightOffText()
    {
        SetTextTimer("Press F to turn OFF flashlight");
    }

}
