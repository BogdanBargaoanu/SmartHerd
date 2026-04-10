using UnityEngine;
using UnityEngine.AI;

public class ClickToMoveTarget : MonoBehaviour
{
    public Camera cam;
    public Transform target;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (cam == null || target == null)
            {
                Debug.LogError("The Camera or Target is missing");
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    target.position = navHit.position;
                }
            }
        }
    }
}
