using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakikomiControl : MonoBehaviour
{

    [SerializeField] private LayerMask monoLayer; //巻き込む対象のレイヤー
    [SerializeField] SphereCollider katamariSphereCollider;

    private float katamariSizeFactor = 0.2f;

    private float katamariSize;







    private void Update()
    {
        MonoAroundDetect();
    }

    public void MonoAroundDetect()
    {
        //巻き込みコントロールのオブジェクトにアタッチされたコライダーの大きさを現在の塊の大きさとする
        katamariSize = katamariSphereCollider.radius * 2;



        //塊の周りにあるmonoLayerレイヤーに所属するオブジェクトを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, katamariSphereCollider.radius * 1.1f, monoLayer);

        //塊周辺のモノが巻き込めるか判定してisTriggerを制御する
        foreach (Collider monoCollider in hitColliders)
        {

            Vector3 monoSize = Vector3.Scale(monoCollider.bounds.size, monoCollider.gameObject.transform.localScale);//巻き込まれる対象の絶対的な大きさを取得
            float aspectRatio = Mathf.Max(monoSize.x, monoSize.y, monoSize.z) / Mathf.Min(monoSize.x, monoSize.y, monoSize.z); //どれくらい細長いかの指標
            float ratioFactor = (aspectRatio > 3f) ? 1.5f : 1f; //細長いものは巻き込みづらくなる

            if (katamariSize * katamariSizeFactor > monoSize.magnitude * ratioFactor)　//モノが塊より小さかったら
            {
                monoCollider.isTrigger = true; //モノのコライダーのisTriggerをオンにしておく（OnTriggerEngerで巻き込める）
            }
            else
            {
                monoCollider.isTrigger = false;　//モノのコライダーのisTriggerをオフにしておく（OnTriggerEngerで巻き込めず、ぶつかる）
            }

        }
 
    }






}
