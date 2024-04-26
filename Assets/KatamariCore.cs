using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariCore : MonoBehaviour
{
    [SerializeField] private Transform doMakikomi;

    private void Update()
    {
        transform.position = doMakikomi.position;
        transform.rotation = doMakikomi.rotation;
    }

}
