using UnityEditor;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float radius = 5f;
    public float height = 5f;
    [Range(0, 1)]
    public float angleth = 5f;
    public Transform player;
    /*void OnDrawGizmos()
    {
        Gizmos.matrix = Handles.matrix = transform.localToWorldMatrix;
        Gizmos.color = Handles.color = Contains(player.position) ? Color.red : Color.white;


        Vector3 top = new Vector3(0, height, 0);


        Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius);
        Handles.DrawWireDisc(top, Vector3.up, radius);

        float p = angleth;
        float x = Mathf.Sqrt(1 - p * p);

        Vector3 vRight = new Vector3(x, 0, p) * radius;
        Vector3 vLeft = new Vector3(-x, 0, p) * radius;

        Gizmos.DrawRay(Vector3.zero, vLeft);
        Gizmos.DrawRay(Vector3.zero, vRight);
        Gizmos.DrawRay(top, vLeft);
        Gizmos.DrawRay(top, vRight);

        Gizmos.DrawLine(Vector3.zero, top);
        Gizmos.DrawLine(Vector3.zero + vLeft, top + vLeft);
        Gizmos.DrawLine(Vector3.zero + vRight, top + vRight);

    }*/

    public bool Contains(Vector3 position)
    {
        Vector3 vecToTargetWorld = position - transform.position;

        Vector3 vecToTarget = transform.InverseTransformVector(vecToTargetWorld);



        //height check
        if (vecToTarget.y < 0 || vecToTarget.y > height)
            return false;

        Vector3 flatDir = vecToTarget;
        flatDir.y = 0;
        float flatDistance = flatDir.magnitude;

        flatDir /= flatDistance;
        Vector3 dirTotarget = vecToTarget.normalized;
        // angle check
        if (flatDir.z < angleth)
            return false;

        //distance check
        if (flatDistance > radius)
            return false;


        return true;
    }
    private void DistanceCheck()
    {
        Vector3 center = transform.position;

        if (player == null)
            return;


        Vector3 playerPos = player.position;

        Vector3 delta = center - playerPos;
        //float distance = Vector3.Distance(center, playerPos);

        float squrDistance = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
        bool isInside = squrDistance <= radius * radius;

    }


    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(transform.forward * radius);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius))
        {
            Debug.Log(hit.transform.name);
        }
    }
}