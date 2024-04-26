using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraControl : MonoBehaviour
{
    [SerializeField] private KatamariMovement scriptKatamariMovement;
    [SerializeField] private Transform sphereCenter; //カメラが追跡するターゲット
    [SerializeField] private Transform katamari;
    [SerializeField] private float damping = 0.1f;

    private float rotationSpeed = 5f;
    private float anglePerSecond = 10f;

    private Vector3 offset; //カメラとターゲットの距離
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 currentVelocity2 = Vector3.zero;

    private void Start()
    {
        //カメラとターゲットの距離の差分を計算
        offset = transform.position - sphereCenter.position;
    }

    private void LateUpdate()
    {
        //ターゲットの移動方向を取得
        Vector3 moveDirection = sphereCenter.forward;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        Vector3 velocity = katamari.GetComponent<Rigidbody>().velocity;

        //if(scriptKatamariMovement.inputDirection.z > 0.2f)
        //{
        //    //ターゲットの移動方向にカメラを向ける
        //    //Quaternion targetRotation = Quaternion.LookRotation(scriptKatamariMovement.moveDirection, Vector3.up);
        //    Quaternion targetRotation = Quaternion.LookRotation(sphereCenter.position + scriptKatamariMovement.moveDirection * offset.magnitude * 0.5f - transform.position, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(sphereCenter.position - transform.position, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}

        if (scriptKatamariMovement.inputDirection.z > 0.2f)
        {
            //カメラの位置を塊が始点のベクトルで表し、塊を中心に回転させる。
            Vector3 localPosition = transform.position - sphereCenter.position;
            Quaternion localRotaion = Quaternion.AngleAxis(anglePerSecond * Time.deltaTime, Vector3.up);
            Vector3 rotatedPosition = localRotaion * localPosition;


            //ローカル座標系からワールド座標系になおす
            Vector3 newPosition = sphereCenter.position + rotatedPosition;
            Quaternion newRotation = Quaternion.LookRotation(sphereCenter.position - newPosition, Vector3.up);

            //カメラの位置と向きを更新
            transform.position = newPosition;
            transform.rotation = newRotation;
            //transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref currentVelocity2, damping);
            //transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);

            Debug.Log("input forward");
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(sphereCenter.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //ターゲットの位置にカメラを移動させる
            Vector3 targetPosition = sphereCenter.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, damping);


            Debug.Log("else");
        }




        ////ターゲットの位置にカメラを移動させる
        //Vector3 targetPosition = sphereCenter.position + offset;
        //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, damping);

    }
}
