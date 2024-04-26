using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCenter : MonoBehaviour
{
    private PlayerInputSystem playerInputSystem;
    [SerializeField] private KatamariMovement scriptKatamariMovement;
    [SerializeField] private Transform katamari;

    private float anglePerSecond = 50f;
    private float rotateSpeed = 90f;

    private float inputRotate;

    private void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        //playerInputSystem.Player.Flip.performed += Flip;
    }

    private void Update()
    {

        //塊の位置をこのsphereCenterが取得して常に同じ位置に動く
        transform.position = katamari.position;

        //斜め前に塊が動いたときに、このsphereCenterも少しそちら側を向くようにする
        if (scriptKatamariMovement.inputDirection.z > 0.2f)
        {
            transform.Rotate(0f, anglePerSecond * scriptKatamariMovement.inputDirection.x * Time.deltaTime, 0f);
        }

        RotatePlayer();

        

    }

    //スティック上下あべこべの入力で塊の周りを左右に回る。
    private void RotatePlayer()
    {
        float inputRotateLeft = playerInputSystem.Player.RotateLeft.ReadValue<float>();
        float inputRotateRight = playerInputSystem.Player.RotateRight.ReadValue<float>();

        inputRotate = (inputRotateRight > inputRotateLeft) ? inputRotateRight : -inputRotateLeft;

        if(Mathf.Abs(inputRotate) > 0.2f)
        {
            transform.Rotate(0f, rotateSpeed * inputRotate * Time.deltaTime, 0f);
        }
    }


    private void Flip(InputAction.CallbackContext context)
    {
        transform.Rotate(0f, 180f, 0f);
    }



}
