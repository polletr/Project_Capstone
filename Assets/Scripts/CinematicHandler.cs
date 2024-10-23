using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicHandler : Singleton<CinematicHandler>
{
    [SerializeField] private List<TimelineAsset> deathCinematics = new();
    [field: SerializeField] public TimelineAsset oneSecTransition;

    private PlayableDirector director;

    public float OneSecDuration 
    {
        get
        {
        if (!oneSecTransition || director.state != PlayState.Playing )
        {
            Debug.Log("OneSecTransition is null or not the current playable asset");
            return 0f;
        }
        
        var time = (float)oneSecTransition.duration;
        return time;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        director = GetComponent<PlayableDirector>();
    }
    public void PlayDeathCinematic()
    {
        if (deathCinematics.Count == 0) return;
        Debug.Log("Playing Death Cinematic");
        // Get a random index from the list
        var randomIndex = Random.Range(0, deathCinematics.Count);

        director.playableAsset = deathCinematics[randomIndex];
        director.Play();
    }

    public void PlayOneSecTransition()
    {
        director.playableAsset = oneSecTransition;
        director.Play();
    }

}
