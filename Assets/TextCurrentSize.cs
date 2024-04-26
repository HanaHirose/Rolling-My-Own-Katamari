using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextCurrentSize : MonoBehaviour
{
    [SerializeField] private KatamariMovement katamariMovement;
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float size = katamariMovement.katamariCurrentDiameter;

        int m = Mathf.FloorToInt(size); //[m]の部分
        size -= m;
        size *= 100;
        int cm = Mathf.FloorToInt(size); //[cm]の部分
        size -= cm;
        size *= 10;
        int mm = Mathf.FloorToInt(size); //[mm]の部分

        textMeshPro.text = $"<size=200%>{m}</size>m<size=150%>{cm}</size>cm<size=120%>{mm}</size>mm";
    }
}
