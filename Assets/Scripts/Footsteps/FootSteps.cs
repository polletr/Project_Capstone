using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private EventReference stepsSound;

    private enum CURRENT_TERRAIN { GRASS, GRAVEL, WOOD_FLOOR, WATER };

    private CURRENT_TERRAIN currentTerrain;

    private EventInstance footsteps;

    private void DetermineTerrain()
    {
        RaycastHit[] hit;

        hit = Physics.RaycastAll(transform.position, Vector3.down, 5.0f);

        foreach (RaycastHit rayhit in hit)
        {
            if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Gravel"))
            {
                currentTerrain = CURRENT_TERRAIN.GRAVEL;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Wood"))
            {
                currentTerrain = CURRENT_TERRAIN.WOOD_FLOOR;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                currentTerrain = CURRENT_TERRAIN.WATER;
            }
        }


    }

    private void PlayFootstep(int terrain)
    {
        footsteps = RuntimeManager.CreateInstance(stepsSound);
        footsteps.setParameterByName("Terrain", terrain);
        footsteps.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        footsteps.start();
        footsteps.release();
    }


    public void Step()
    {
        DetermineTerrain();
        switch (currentTerrain)
        {
            case CURRENT_TERRAIN.GRAVEL:
                PlayFootstep(1);
                break;

            case CURRENT_TERRAIN.WOOD_FLOOR:
                PlayFootstep(2);
                break;

            case CURRENT_TERRAIN.WATER:
                PlayFootstep(3);
                break;

            default:
                PlayFootstep(0);
                break;
        }
    }
}