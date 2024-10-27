using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScribbleFaceAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> listOfImages = new();
    [SerializeField] private float changeInterval = 0.1f; // Time between texture changes

    private Image image;
    private int currentTextureIndex = 0;

    public void Awake()
    {
        image = GetComponent<Image>();
    }

    private IEnumerator LoopImages()
    {
        while (true)
        {
            image.sprite = listOfImages[currentTextureIndex];
            currentTextureIndex = (currentTextureIndex + 1) % listOfImages.Count;
            yield return new WaitForSeconds(changeInterval);
        }
    }

    private void OnEnable()
    {
        if (listOfImages.Count > 0)
            StartCoroutine(LoopImages());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
