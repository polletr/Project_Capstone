using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPickupUIHandler : Singleton<ObjectPickupUIHandler>
{
    public GameEvent Event;
    [SerializeField] private float waitTime = 2f;

    private UIAnimator uiAnimator;
    private Image itemDisplayImage;
    private TextMeshProUGUI itemDisplayText;

    private bool isAnimating;
    private bool isProcessing;
    private readonly Queue<PickupData> pickedUpObjects = new();


    private void OnEnable()
    {
        Event.OnPlayerDeath += ClearQueue;
    }

    private void OnDisable()
    {
        Event.OnPlayerDeath -= ClearQueue;
    }

    public void Start()
    {
        itemDisplayImage = GetComponentInChildren<Image>();
        itemDisplayText = GetComponentInChildren<TextMeshProUGUI>();
        uiAnimator = GetComponentInChildren<UIAnimator>();

        if (uiAnimator == null) Debug.Log("UIAnimator is null on ObjectPickupUIHandler");
        if (itemDisplayImage == null) Debug.Log("Image is null on ObjectPickupUIHandler");
        if (itemDisplayText == null) Debug.Log("Text is null on ObjectPickupUIHandler");
    }

    public void PickedUpObject(PickupData pickup, float time = 0)
    {
        pickedUpObjects.Enqueue(pickup);

        if (!isProcessing)
        {
            StartCoroutine(HandlePickedUpObject(time));
        }
    }

    private IEnumerator HandlePickedUpObject(float time)
    {
        var delay = time == 0 ? waitTime : time;
        isProcessing = true;

        while (pickedUpObjects.Count > 0)
        {
            var pickup = pickedUpObjects.Dequeue();

            yield return new WaitForSeconds(CinematicHandler.Instance ? CinematicHandler.Instance.OneSecDuration : 1);

            MoveIn(pickup);

            // Wait for the animation to finish moving in
            yield return new WaitUntil(() => !isAnimating);

            // Wait for waitTime after the item has moved in
            yield return new WaitForSeconds(delay);

            MoveBack();

            // Wait for the animation to finish moving back
            yield return new WaitUntil(() => !isAnimating);
        }

        isProcessing = false;
    }

    private void MoveIn(PickupData pickup)
    {
        isAnimating = true;
        itemDisplayImage.sprite = pickup.PickupSprite;
        itemDisplayText.text = pickup.PickupName;

        uiAnimator.OnAnimateFinished.AddListener(OnMoveFinished);
        uiAnimator.MoveInAnimate(true); // Start move in animation
    }

    private void MoveBack()
    {
        isAnimating = true;
        uiAnimator.OnAnimateFinished.AddListener(OnMoveFinished);
        uiAnimator.MoveInAnimate(false); // Start move back animation
    }

    private void OnMoveFinished()
    {
        uiAnimator.OnAnimateFinished.RemoveListener(OnMoveFinished);
        isAnimating = false;
    }

    private void ClearQueue()
    {
        if (!isProcessing) return;

        uiAnimator.MoveInAnimate(false);
        pickedUpObjects.Clear();
        isAnimating = false;
        isProcessing = false;
    }
}