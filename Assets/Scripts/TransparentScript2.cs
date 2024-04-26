using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentScript2 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float transparency = 0f;

    private Material material;
    private Vector3 targetWorldPosition;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    private void Update()
    {



        Vector3 direction = target.position - cam.transform.position;
        float distance = Vector3.Distance(target.position, cam.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, direction, out hit, distance, obstacleLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Transparent!");
                targetWorldPosition = hit.point;
                material.SetVector("_WorldOrigin", targetWorldPosition);
                material.SetFloat("_Radius", radius);
                material.SetFloat("_Transparency", transparency);
            }
            else
            {
                material.SetFloat("_Transparency", 1.0f);
            }
        }
        else
        {
            material.SetFloat("_Transparency", 1.0f);
        }
    }
}

