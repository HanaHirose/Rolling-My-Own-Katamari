using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//このコードはシェーダーのコードをいじらずに既存のシェーダーの透明度をこのスクリプト内でColor newColor をいじって変える方法
public class TransparentTest : MonoBehaviour
{
    [SerializeField] private float transparency = 0.8f;
    [SerializeField] private Transform target;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask obstacleLayer;

    private Material material;
    private Color originalColor;
    private Vector3 targetWorldPosition;


    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
        else
        {
            Debug.LogWarning("No Renderer component found on the object.");
        }

        //material = GetComponent<Material>();
        //originalColor = material.color;
    }
    private void Update()
    {
        
        Color newColor = originalColor;
        newColor.a = 1.0f;


        Vector3 direction = target.position - cam.transform.position;
        float distance = Vector3.Distance(target.position, cam.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, direction, out hit, distance, obstacleLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Transparent!");
                newColor.a = transparency;
            }
            else
            {
                newColor.a = 1.0f;
            }
        }
        else
        {
            newColor.a = 1.0f;
        }

        material.color = newColor;

    }
}
