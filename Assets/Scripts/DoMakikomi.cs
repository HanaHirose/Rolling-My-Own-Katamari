using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoMakikomi : MonoBehaviour
{
    private SphereCollider sphereCollider;

    [SerializeField] private KatamariMovement katamariMovement;
    [SerializeField] private Transform keepMakikomi; //巻き込んだものをキープしておくためのオブジェクト。巻き込んだモノはこれの子にする
    [SerializeField] private LayerMask monoLayer; //巻き込む対象のレイヤー
    [SerializeField] private IbitsuMovement ibitsuMovement; //このフィールドに音用の空のオブジェクトをいれて、いびつなモノを巻き込んだ効果についてのスクリプトを読み込む
    [SerializeField] private SoundController soundController; //このフィールドに音用の空のオブジェクトをいれて、音を制御するスクリプトを読み込む

    public bool isIbitsu = false;
    public Vector3[] vertices;
    public Transform otherTransform;


    private float katamariSizeFactor = 0.3f; // 塊の直径を基準に何割の大きさか


    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        MonoAroundDetect(); //塊の近くにあるものが巻き込めるかどうかの判定を行う
    }


    //塊の近くにあるものが巻き込めるかどうかの判定
    public void MonoAroundDetect()
    {
        //巻き込みコントロールのオブジェクトにアタッチされたコライダーの大きさを現在の塊の大きさとする
        float katamariSize = katamariMovement.katamariCurrentDiameter; //塊の直径



        //塊の周りにあるmonoLayerレイヤーに所属するオブジェクトを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, (katamariSize / 2) * 1.1f, monoLayer);

        //塊周辺のモノが巻き込めるか判定してisTriggerを制御する
        foreach (Collider monoCollider in hitColliders)
        {

            if (katamariSize * katamariSizeFactor >= MonoSizeMeter(monoCollider))　//モノが塊より小さかったら
            {
                monoCollider.isTrigger = true; //モノのコライダーのisTriggerをオンにしておく（OnTriggerEngerで巻き込める）
            }
            else
            {
                monoCollider.isTrigger = false;　//モノのコライダーのisTriggerをオフにしておく（OnTriggerEngerで巻き込めず、ぶつかる）
            }

        }

    }

    //モノのBoxColliderの各辺の絶対値[m]でできたVector3 monoSizeを使ってモノのサイズを[m]で判定する
    private float MonoSizeMeter(Collider monoCollider)
    {
        Vector3 monoSize = monoCollider.bounds.size; //単位は[m]でBoxColliderの各辺の大きさを取得

        //float size = (monoSize.x + monoSize.y + monoSize.z) / 3f;
        float[] length = new float[3] { monoSize.x, monoSize.y, monoSize.z };
        Array.Sort(length);
        Array.Reverse(length); //3辺を大きい順に並べる
        float a1 = length[0] / length[2];
        float a2 = length[1] / length[2];
        float lengthNew1 = length[0] / Mathf.Pow(a1, FuncN(a1));
        float lengthNew2 = length[1] / Mathf.Pow(a2, FuncN(a2));
        float lengthNew3 = length[2];

        float size = (lengthNew1 + lengthNew2 + lengthNew3) / 3f;

        return size;
    }

    //形状を考慮に入れて補正されたモノのサイズを表すときに使うファクターを求める関数
    private float FuncN(float a)
    {
        float nFactor = 0.1f; //pythonのフィットで得た値
        return nFactor * Mathf.Log10(a);
    }

    //巻き込みを行う
    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
        {
            if ((monoLayer.value & (1 << other.gameObject.layer)) > 0) //モノレイヤーに分類されているモノか判定
            {
                BoxCollider boxCollider = other.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    //巻き込む前にBox Colliderの８つの頂点を取得
                    vertices = ibitsuMovement.GetBoxColliderVertices(boxCollider);

                    //巻き込む！
                    IncreaseSize(other); // 塊のサイズを大きくする
                    other.transform.parent = keepMakikomi;　//巻き込む対象をKeepMakikomiの子供にする
                    boxCollider.enabled = false; //巻き込まれたモノのBox Colliderを無効にする

                    soundController.PlayRandomMakikomiSound();

                    //いびつなモノを巻き込んだことを塊の動きに反映させるフラグを立てる
                    isIbitsu = true;
                    otherTransform = other.GetComponent<Transform>();
                }

                //if (other.CompareTag("Ibitsu"))
                //{
                //    int ibitsuLayer = LayerMask.NameToLayer("Ibitsu");
                //    other.gameObject.layer = ibitsuLayer;
                //    boxCollider.enabled = true;
                //    boxCollider.isTrigger = false;
                //    ibitsuCollider = other;
                //    Debug.Log("ibitsuCollider: " + ibitsuCollider);
                //}
            }

        }


    }


    private void IncreaseSize(Collider other)
    {
        float monoSize = MonoSizeMeter(other);
        float katamariRadius = katamariMovement.katamariCurrentDiameter / 2f; //塊の半径

        //float deltaR = Mathf.Pow(monoSize, 2f) / (2f * 3.14f * katamariRadius); //子のモノを巻き込んだら塊の半径がどれだけ大きくなるか[m]
        float deltaR = Mathf.Pow(monoSize, 3f) / (4f * 3.14f * Mathf.Pow(katamariRadius, 2f));
        Debug.Log("deltaR: " + deltaR);

        katamariMovement.katamariCurrentDiameter += deltaR * 2f; //スケール１のときの塊は半径0.5mなので、それで規格化する
    }

}
