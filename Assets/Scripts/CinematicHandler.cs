using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicHandler : Singleton<CinematicHandler>
{
    [SerializeField] private List<TimelineAsset> deathCinematics = new();
    [field: SerializeField] public TimelineAsset oneSecTransition;
    [field: SerializeField] public TimelineAsset threeSecTransition;

    private PlayableDirector director;

    public float OneSecDuration 
    {
        get
        {
        if (!oneSecTransition || director.state != PlayState.Playing )
        {
            return 0f;
        }
        
        var time = (float)oneSecTransition.duration;
        return time;
        }
    }

    public float ThreeSecDuration
    {
        get
        {
            if (!oneSecTransition || director.state != PlayState.Playing)
            {
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

    public void PlayThreeSecTransition()
    {
        director.playableAsset = threeSecTransition;
        director.Play();
    }


}
