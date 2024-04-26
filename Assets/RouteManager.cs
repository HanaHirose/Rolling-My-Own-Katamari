using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [SerializeField] private Transform[] waypoits; //これは他（IkimonoMovement.cs）から直接参照したりいじったりされないようにする
    public bool isStopped { get; private set; } = false;


    public Transform GetCurrentWaypint(int currentIndex)
    {
        return waypoits[currentIndex];
    }
    public Transform GetCNextWaypoint(int currentIndex)
    {
        return waypoits[(currentIndex + 1) % waypoits.Length];
    }

    public int IncreaseWaypointIndex(int currentIndex)
    {
        return (currentIndex + 1) % waypoits.Length;
    }
    
    public void StopAll()
    {
        isStopped = true;
        Invoke("ResumeMoving", 4.0f);
    }

    private void ResumeMoving()
    {
        isStopped = false;
    }

}
