using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldMovieTexture : MonoBehaviour
{
    [SerializeField] private List<Texture> listOfTextures = new();
    [SerializeField] private float changeInterval = 0.1f; // Time between texture changes

    private RawImage rawImage;
    private int currentTextureIndex = 0;

    public void StartFilm()
    {
        rawImage = GetComponent<RawImage>();
        if (listOfTextures.Count > 0)
        {
            StartCoroutine(LoopTextures());
        }
    }

    private IEnumerator LoopTextures()
    {
        while (true)
        {
            rawImage.texture = listOfTextures[currentTextureIndex];
            currentTextureIndex = (currentTextureIndex + 1) % listOfTextures.Count;
            yield return new WaitForSeconds(changeInterval);
        }
    }

    public void StopFilm()
    {
        StopAllCoroutines();
    }

}
