using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicHandler : MonoBehaviour
{

    [SerializeField] private List<TimelineAsset> cinematics = new();

    private PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }
    public void PlayDeathCinematic()
    {
        if (cinematics.Count == 0) return;

        // Get a random index from the list
        int randomIndex = Random.Range(0, cinematics.Count);

        director.playableAsset = cinematics[randomIndex];
        director.Play();

    }

}
