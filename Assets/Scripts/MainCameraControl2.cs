using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MainCameraControl2 : MonoBehaviour
{
    private PlayerInputSystem playerInputSystem;

    [SerializeField] private Transform sphereCenter;
    [SerializeField] private Transform KatamariRigidbody;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private OujiMovement scriptOujiMovement;
    [SerializeField] private LayerMask wallLayer; //このwallLayerに対してカメラがめり込まずに上に移動する
    [SerializeField] private LayerMask obstacleLayer; //このobstacleがカメラと塊の間にあったら透明な球を発動する

    private float damping = 0.1f;
    private float cameraHeightFactor = 5f;

    private Vector3 offset; //カメラとターゲットの距離
    private Vector3 currentVelocity;
    private Vector3 targetPosition;
    private bool isFlipping = false; //反転中かのフラグ
    private float radius;
    private Vector3 newTargetPosion;
    private Renderer previousRenderer;




    private void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        //playerInputSystem.Player.Flip.performed += FlipCamera;
    }

    private void Start()
    {
        //radius = sphereCollider.radius;
        radius = KatamariRigidbody.localScale.x;

        //カメラとターゲットの距離の差分を計算
        //sphereCenterからoffset分（ただしzは-z方向に）動いたところに基本的にカメラを配置
        offset.y = radius * 1.5f;
        offset.z = radius * 8f;
        offset.x = 0f;


    }

    private void Update()
    {
        radius = KatamariRigidbody.localScale.x;

        //カメラとターゲットの距離の差分を計算
        //sphereCenterからoffset分（ただしzは-z方向に）動いたところに基本的にカメラを配置
        offset.y = radius * 1.5f;
        offset.z = radius * 8f;
    }

    private void LateUpdate()
    {
        if (!isFlipping)
        {
            //ターゲットの位置にカメラを移動させる
            targetPosition = sphereCenter.position + sphereCenter.up * offset.y - sphereCenter.forward * offset.z;
            AvoidObstacle();
            transform.position = Vector3.SmoothDamp(transform.position, newTargetPosion, ref currentVelocity, damping);

        }

        //カメラの向きをターゲットに向ける
        Vector3 lookAtPos = sphereCenter.position + sphereCenter.up * offset.y * 0.5f;
        transform.LookAt(lookAtPos);

        FlipCamera();

        ObstacleTransparent();
    }


    private void FlipCamera()
    {
        if (!isFlipping && playerInputSystem.Player.Flip.ReadValue<float>() == 1)
        {
            StartCoroutine(FlipCameraCoroutine());
        }
    }

    private IEnumerator FlipCameraCoroutine()
    {
        radius = sphereCollider.radius;
        isFlipping = true;
        float time = 0f;
        float length = offset.z;
        float height = radius * cameraHeightFactor;
        Vector3 cameraPositionStart = transform.position;
        Vector3 spherePositionStart = sphereCenter.position;


        Vector3 houbutsuLocal;
        Vector3 houbutsuWorld;
        houbutsuLocal.x = 0f;
        

        while(time < scriptOujiMovement.timeFlip)
        {
            time += Time.deltaTime;
            houbutsuLocal.z = time / scriptOujiMovement.timeFlip * 2 * length;
            houbutsuLocal.y = -height * Mathf.Pow((2 * time / scriptOujiMovement.timeFlip - 1), 2) + height;
            houbutsuWorld = sphereCenter.forward * houbutsuLocal.z + sphereCenter.right * houbutsuLocal.y * 0.2f + sphereCenter.up * houbutsuLocal.y;
            transform.position = cameraPositionStart + houbutsuWorld + sphereCenter.position - spherePositionStart;

            yield return null;
        }

        isFlipping = false;
        sphereCenter.Rotate(0f, 180f, 0f);
    }


    //カメラを壁にのめりこまないように上に流すスクリプト
  private void AvoidObstacle()
    {
        Vector3 direction = targetPosition - sphereCenter.position; //基本的なカメラがあるはずの位置から塊にむかってrayを飛ばすための方向
        float distance = Vector3.Distance(targetPosition, sphereCenter.position);

        RaycastHit hit;
        if (Physics.Raycast(sphereCenter.position, direction, out hit, distance, wallLayer))
        {
            //rayが壁にぶつかったとき
            Vector3 wallPosition = sphereCenter.position + direction.normalized * hit.distance;

            newTargetPosion =  wallPosition + Vector3.up * Vector3.Distance(targetPosition, wallPosition) * 2f;

            Debug.Log("Wall!");
        }
        else
        {
            newTargetPosion = targetPosition;
        }

    }


    //カメラと塊の間に障害物があるときに、その障害物を半透明にする
    private void ObstacleTransparent()
    {
        //以前の障害物の半透明を解除する
        if(previousRenderer != null)
        {
            Material material = previousRenderer.material;
            Color col = material.color;
            col.a = 1.0f;
            material.color = col;

            previousRenderer = null;
        }


        Vector3 direction = sphereCenter.position - transform.position; //今カメラがある位置から塊の中心に向かう方向
        float distance = Vector3.Distance(sphereCenter.position, transform.position);

        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, distance, obstacleLayer))
        {
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();

            Debug.Log("hit!");

            if(renderer != null)
            {
                Debug.Log("Got renderer");
                Material material = renderer.material;
                Color col = material.color;


                col.a = 0.5f; //半透明にする
                material.color = col;

                previousRenderer = renderer; //ヒットしたオブジェクトのレンダラーを記録
            }
        }

    }

}
