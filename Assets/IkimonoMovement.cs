using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//生き物が複数隊列にいる場合にも対応している。
//このスクリプトを隊列にいる生き物すべてにアタッチする
//RouteManager.csを空のオブジェクトにアタッチして、この隊列を管理する
public class IkimonoMovement : MonoBehaviour
{
    private BoxCollider boxCollider;

    [SerializeField] private RouteManager routeManager;

    private float speed = 5f;
    private float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;
    private bool canMove = true; //塊に一度でも巻き込まれたらfalseにして、このオブジェクトが巡回するのを永久に停止する

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Debug.Log("boxCollider: " + boxCollider);

        if (boxCollider.enabled == false)
        {
            canMove = false;
        }

        if (!routeManager.isStopped && canMove) //全体停止のフラグが立ってなければ　& 　この個体がcamMove == trueなら
        {
            Transform targetWaypoint = routeManager.GetCurrentWaypint(currentWaypointIndex);

            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            Quaternion toRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime); //滑らかに回転
            transform.position += direction * speed * Time.deltaTime; //移動

            //次のウェイポイントをターゲットに変える
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex = routeManager.IncreaseWaypointIndex(currentWaypointIndex);
            }

        }

        

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Katamari"))
        {
            routeManager.StopAll(); //列にいる生き物全部を停止させる
        }
    }


}
