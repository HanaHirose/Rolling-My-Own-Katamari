using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkimonoMovementTest : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;

    private float speed = 5f;
    private float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;
    private bool isStopped = false;


    private void Update()
    {
        if (!isStopped) //停止のフラグが立ってなければ
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            Quaternion toRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime); //滑らかに回転
            transform.position += direction * speed * Time.deltaTime; //移動


            //次のウェイポイントをターゲットに変える
            if(Vector3.Distance(transform.position, targetWaypoint.position)< 0.1f)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }



        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //塊と衝突したら、一定時間停止する
        if (collision.gameObject.CompareTag("Katamari"))
        {
            isStopped = true;
            Invoke("ResumeMoving", 4.0f); //数秒後に動きを開始する
        }
    }

    private void ResumeMoving()
    {
        isStopped = false;
    }
}
