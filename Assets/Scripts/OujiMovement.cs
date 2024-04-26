using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OujiMovement : MonoBehaviour
{
    private PlayerInputSystem playerInputSystem;

    [SerializeField] Transform sphereCenter;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] SoundControllerBasic soundControllerBasic;

    private Vector3 worldDirection;
    private bool isFlipping = false; //反転中かどうかのフラグ
    public float timeFlip = 1f; //反転にかかる時間
    private float distanceFactor = 1.3f; //塊と王子の距離を決める（radiusの何倍離れるか）


    private void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        playerInputSystem.Player.Flip.performed += FlipOuji;
    }

    private void Update()
    {


        Vector3 localForward = sphereCenter.forward; //sphereCenterのローカル座標で+z方向をワールド座標系で返す

        //worldDirection = sphereCenter.TransformDirection(localForward); //ワールド座標系に直す

        float radius = sphereCollider.radius;
 

        if (!isFlipping)
        {
            //塊の背後に王子を位置させる
            transform.position = sphereCenter.position - localForward * radius * distanceFactor - Vector3.up * radius;
        }

        //王子が塊の中心を向くようにする。ただし常に地面に垂直に立っている
        //transform.LookAt(sphereCenter.position);
        Vector3 lookDirection = transform.position - sphereCenter.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        
  
    }

    //スティック同時押し込みで王子を反転させる
    private void FlipOuji(InputAction.CallbackContext context)
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipOujiCoroutine());
            soundControllerBasic.PlayHantenSound(); //反転時の効果音を鳴らす
        }
    }

    private IEnumerator FlipOujiCoroutine()
    {
        isFlipping = true;
        Vector3 oujiPositionStart = transform.position;
        Vector3 spherePositionStart = sphereCenter.position;
        float radius = sphereCollider.radius;
        float distance = radius * distanceFactor;
        float height = radius *2.3f +1f;
        Vector3 houbutsuLocal;
        houbutsuLocal.x = 0f;
        Vector3 houbutsuWorld;

        float time = 0f;

        while(time < timeFlip)
        {
            time += Time.deltaTime;
            houbutsuLocal.z = time / timeFlip * 2 * distance;
            houbutsuLocal.y = -height * Mathf.Pow((2 * time / timeFlip - 1), 2) + height;
            houbutsuWorld = sphereCenter.forward * houbutsuLocal.z + sphereCenter.right * houbutsuLocal.y * 0.1f + sphereCenter.up * houbutsuLocal.y;
            transform.position = oujiPositionStart + houbutsuWorld + sphereCenter.position - spherePositionStart;

            yield return null;
        }

        isFlipping = false;
        //sphereCenter.Rotate(0f, 180f, 0f);
    }

}
