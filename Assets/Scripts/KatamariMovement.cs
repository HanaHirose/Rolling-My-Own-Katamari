using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KatamariMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerInputSystem playerInputSystem;
    private SphereCollider sphereCollider;

    [SerializeField] private LayerMask groundlayer;
    [SerializeField] private LayerMask monoLayer; //巻き込む対象のレイヤー
    [SerializeField] private Transform cam;
    [SerializeField] private Transform sphereCenter;
    [SerializeField] private SoundController soundController; //このフィールドに音用の空のオブジェクトをいれて、音を制御するスクリプトを読み込む
    [SerializeField] private SoundControllerBasic soundControllerBasic; //ダッシュの音
    [SerializeField] private SoundControllerDashTame soundControllerDashTame; //ダッシュタメの音
    [SerializeField] private IbitsuMovement ibitsuMovement; //このフィールドに音用の空のオブジェクトをいれて、いびつなモノを巻き込んだ効果についてのスクリプトを読み込む
    [SerializeField] private DoMakikomi doMakikomi; //これは巻き込みを実行するスクリプト。

    [SerializeField] private float accelation = 40f;
    private float maxspeedForward = 5f;
    private float maxspeedRatioBack = 0.6f;
    private float maxspeedRatioSide = 0.8f;
    private float deceleration= 50f;
    private float dashCountTime = 0.4f; //ガチャガチャをどれくらいの時間ごとに入力するか
    private float dashTime = 1f; //ダッシュしている時間
    private float dashSpeed = 20f; //ダッシュ速度
    private float rotationDashSpeed = 360f *2; //ダッシュ前の準備での回転速度
    private int dashInputCountMax = 7; //何回ガチャガチャしたらダッシュするか
    private float rotationSpeed = 50f;
    [SerializeField] private float normalDrag = 0.8f;
    [SerializeField] private float normalAngularDrag = 0.8f;
    private float dashDrag = 0f;
    private float dashAngularDrag = 0f;
    [SerializeField] private float moveForce = 10f;
    private float ratioBack = 0.6f;
    private float ratioSide = 0.8f;
    [SerializeField] private float torqueMagnitude = 10f;
    [SerializeField] private float brakeDuration = 0.3f;



    private float inputVertical;
    private float inputHorizontal;
    public Vector3 inputDirection;
    private float targetAngle;
    public Vector3 moveDirection;
    private Vector3 moveDirectionOnlyX;
    private float decelerationFactor;
    private float timer = 0f;
    private int dashInputCount = 0;
    private float inputRotateStoreOld;
    private float inputRotateStore = 0;
    private bool isDashing = false;
    private bool isPreparingDash = false;
    private bool previousIsPreparingDash = false;
    public Collider ibitsuCollider;
    private bool isBraking = false;
    public float katamariCurrentDiameter; //これが現在の塊の大きさを表す変数







    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        sphereCollider = GetComponent<SphereCollider>();

        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();

    }

    private void Start()
    {
        katamariCurrentDiameter = 1f; //ここで塊の大きさの初期値を入れる [m]
    }


    private void Update()
    {
        ////塊の大きさの変数を代入してKatamariRigidbodyの大きさを更新する
        transform.localScale = katamariCurrentDiameter * Vector3.one;
        Debug.Log("katamariCurrentDiameter: " + katamariCurrentDiameter);

        //いびつなモノを巻き込んだことを塊の動きに反映させる
        if (doMakikomi.isIbitsu)
        {
            ibitsuMovement.IbitsuMove(doMakikomi.vertices, doMakikomi.otherTransform);
        }


        //ダッシュ中のフラグをfalseにする
        if(isDashing && rb.velocity.magnitude < maxspeedForward)
        {
            isDashing = false;
        }
    }


    private void FixedUpdate()
    {

        float inputForward = playerInputSystem.Player.MoveForward.ReadValue<float>();
        float inputBack = playerInputSystem.Player.MoveBack.ReadValue<float>();
        float inputLeft = playerInputSystem.Player.MoveLeft.ReadValue<float>();
        float inputRight = playerInputSystem.Player.MoveRight.ReadValue<float>();

        inputVertical = (inputForward >= inputBack) ? inputForward : -inputBack;
        inputHorizontal = (inputRight >= inputLeft) ? inputRight : -inputLeft;

        //スティック入力の方向
        inputDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;

        //スティック入力をうけてワールド座標系でのmoveDirectionの決定
        if(inputDirection.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            //移動方向（ワールド座標で）
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;


            //横方向の入力のみの記録（ダッシュ時の横方向制御用）
            float targetAngleOnlyX = Mathf.Atan2(inputDirection.x, 0f) * Mathf.Rad2Deg + cam.eulerAngles.y;
            moveDirectionOnlyX = Quaternion.Euler(0f, targetAngleOnlyX, 0f) * Vector3.forward;
        }
        else
        {
            moveDirection = Vector3.zero;
        }



        //基本的な動き
        MoveImproved();


        Dash();


        
    }


    //衝突時に効果音を鳴らす
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionNormal = collision.contacts[0].normal.normalized;

        //垂直方向の衝突なら
        if (Mathf.Abs(collisionNormal.y) > 0.3f)
        {
            //垂直衝突の音を再生
            soundControllerDashTame.PlayBounceSound();
        }
        else
        {
            //水平衝突の音を再生
            soundControllerDashTame.PlayButsukaruA();
        }
    }




    //基本的な動き（新）
    private void MoveImproved()
    {
        float moveForceWithFactor = moveForce;
        if (inputDirection.normalized.z < - 0.1f) //入力が後進なら
        {
            moveForceWithFactor = moveForce * ratioBack;
        }
        else if(Mathf.Abs(inputDirection.normalized.z) <= 0.1f)
        {
            moveForceWithFactor = moveForce * ratioSide;
        }




        float angle = Vector3.Angle(rb.velocity, moveDirection);

        //進行方向と逆の入力があったらブレーキをかける
        if (angle >= 170f && !isBraking)
        {
            StartCoroutine(BrakeCorountine());
        }

        //ブレーキ中でなければ普通に転がす
        if(!isBraking)
        {
            rb.AddForce(moveDirection * moveForceWithFactor, ForceMode.Force);
            //Debug.Log("velocity: "+ rb.velocity.magnitude);
        }


    }


    //ブレーキをかけるコルーチン
    private IEnumerator BrakeCorountine()
    {
        isBraking = true;
        soundControllerDashTame.PlayBrakeSoundShort(); //ブレーキの効果音を鳴らす

        float originalVelocity = rb.velocity.magnitude; //初速度を保存
        
        while(rb.velocity.magnitude > 0f)
        {
            float brakeSpeedDelta = originalVelocity * Time.deltaTime / brakeDuration;
            rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, brakeSpeedDelta);
            yield return null; //次のフレームまで待機
        }

        isBraking = false;

    }


    private void Dash()
    {
        //スティック上下あべこべ入力。値は0か1を返す
        float inputRotateLeft = playerInputSystem.Player.RotateLeft.ReadValue<float>();
        float inputRotateRight = playerInputSystem.Player.RotateRight.ReadValue<float>();

        float inputRotate = (inputRotateRight > inputRotateLeft) ? inputRotateRight : -inputRotateLeft;

        //１フレーム前のinputRotateStoreの値をinputRotateStoreOldに格納しておく
        inputRotateStoreOld = inputRotateStore;

        if(inputRotateRight == 1)
        {
            inputRotateStore = 1;
        }

        if(inputRotateLeft == 1)
        {
            inputRotateStore = -1;
        }


        //タイマーが切れていたときにinputRotateStoreが0から1（または-1）になったとき実行。
        //または、タイマーは切れてなくて前と逆方向の入力があったときに実行。
        //タイマーをつけなおし、カウントの数を１増やす
        if ( (timer <= 0f  && inputRotateStore != 0 && inputRotateStoreOld == 0) || (timer > 0f && inputRotateStore * inputRotateStoreOld == -1) )
        {
            if (!isDashing)
            {
                timer = dashCountTime;
                dashInputCount += 1;
                //Debug.Log(dashInputCount);
            }
        }

        //タイマーが１フレームごとに減っていく
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }

   
        //ダッシュ準備のフラグを立てる
        if(dashInputCount > 3)
        {
            isPreparingDash = true;
        }



        //ダッシュ開始のフラグを立てる
        //カウントが十分たまって、かつタイマーが切れたときに実行
        if(dashInputCount >= dashInputCountMax) //十分ガチャガチャして、（「ガチャガチャするのをやめたとたんに」。これはやめた）
        {
            // ダッシュする
            //Debug.Log("Dash!");
            dashInputCount = 0;
            isPreparingDash = false;
            rb.freezeRotation = false;
            StartCoroutine(DashCoroutine());
            soundControllerBasic.PlayDashSound(); //ダッシュの効果音を鳴らす

        }

        //上の実行をした後で、タイマーが切れていた場合はカウントをリセットし、inputRotateStoreも0にする。
        //つまりタイマーが切れるまではinputRotateStoreは0に戻されない
        if (timer <= 0f)
        {
            dashInputCount = 0;
            inputRotateStore = 0;
            isPreparingDash = false;
        }



        //塊をダッシュの準備で回転させている
        if (isPreparingDash)
        {
            rb.freezeRotation = true;
            transform.Rotate(-sphereCenter.right * rotationDashSpeed * Time.deltaTime, Space.World);
            //rb.freezeRotation = true;
            //rb.AddTorque(-sphereCenter.right * rotationDashSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
        else
        {
            rb.freezeRotation = false;
        }

        //ダッシュタメの効果音を鳴らす
        if(previousIsPreparingDash == false && isPreparingDash == true)
        {
            soundControllerDashTame.PlayDashTameSound();
        }
        if(previousIsPreparingDash == true && isPreparingDash == false)
        {
            soundControllerDashTame.StopDashTameSound();
        }

        previousIsPreparingDash = isPreparingDash;

    }


    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        rb.drag = dashDrag;
        rb.angularDrag = dashAngularDrag;

        //一回だけ初速度を与えてダッシュさせる
        rb.AddForce(sphereCenter.forward * dashSpeed, ForceMode.VelocityChange);

        //一回だけ回転のトルクを与える
        Vector3 torqueDirection = - Vector3.Cross(sphereCenter.forward, Vector3.up * sphereCollider.radius);
        Vector3 torque = torqueMagnitude * torqueDirection;
        rb.AddTorque(torque, ForceMode.Force);


        yield return new WaitForSeconds(dashTime);



        float recoveryTime = 1f; // この値は、DragとAngularDragが元の値に戻るまでの時間
        float elapsedTime = 0f;

        //徐々にDragとAngularの値を元に戻す
        while (elapsedTime < recoveryTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / recoveryTime;
            rb.drag = Mathf.Lerp(dashDrag, normalDrag, t);
            rb.angularDrag = Mathf.Lerp(dashAngularDrag, normalAngularDrag, t);

            //ここに yield return nullをいれるべきかも？！
        }

        //最後に完全にDragとAngularの値を元に戻す
        rb.drag = normalDrag;
        rb.angularDrag = normalAngularDrag;
    }





    //基本的な動き（旧）
    private void Move()
    {
        if (inputDirection.z > 0.2f) //入力が前進なら
        {
            //速度が上限に達していなければ、塊の移動方向に加速度を加える
            if (rb.velocity.magnitude < maxspeedForward)
            {
                rb.AddForce(moveDirection * accelation, ForceMode.Force);
            }
        }
        else if (inputDirection.z < -0.1f) //入力が後進なら
        {
            //速度が上限に達していなければ、塊の移動方向に加速度を加える
            if (rb.velocity.magnitude < maxspeedForward * maxspeedRatioBack)
            {
                rb.AddForce(moveDirection * accelation, ForceMode.Force);
            }
        }
        else //入力が真横方向なら
        {
            //速度が上限に達していなければ、塊の移動方向に加速度を加える
            if (rb.velocity.magnitude < maxspeedForward * maxspeedRatioSide)
            {
                rb.AddForce(moveDirection * accelation, ForceMode.Force);
            }
        }
    }


}
