using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMakikomi : MonoBehaviour
{

    [SerializeField] private Transform doMakikomi; //これが塊の当たり判定

    private void Update()
    {
        //DoMakikomiの位置と回転に追従する。スケールは追従させない
        transform.position = doMakikomi.position;
        transform.rotation = doMakikomi.rotation;
    }
}
