using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicHandler : MonoBehaviour
{

    [SerializeField] private List<TimelineAsset> deathCinematics = new();

    private PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }
    public void PlayDeathCinematic()
    {
        if (deathCinematics.Count == 0) return;
        Debug.Log("Playing Death Cinematic");
        // Get a random index from the list
        int randomIndex = Random.Range(0, deathCinematics.Count);

        director.playableAsset = deathCinematics[randomIndex];
        director.Play();
    }

}
