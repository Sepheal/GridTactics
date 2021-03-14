using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWorldAI : MonoBehaviour
{
    public bool Active = true;
    public GameObject[] WayPoints;
    public int AiId = 1, WayPointId = 0;

    public float WayPointCoolDown = 1.0f, Speed = 1.0f;
    float WayPointStart = 0f;

    // Start is called before the first frame update
    void Start()
    {
        WayPointStart = Time.time;
        gameObject.transform.position = WayPoints[WayPointId].transform.position;
        if (gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().SetBool("Walking", true); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            //Cycle between waypoints in order
            if (AiId == 1)
            {
                GameObject PointA = WayPoints[WayPointId], PointB;
                if (WayPointId == (WayPoints.Length - 1))
                {
                    PointB = WayPoints[0];
                }
                else PointB = WayPoints[WayPointId + 1];
                //float Factor = (Time.time - WayPointStart) / WayPointCoolDown;
                //if (Factor >= 1.0f)
                if (Vector3.Distance(gameObject.transform.position, PointB.transform.position) <= 1.0f)
                {
                    gameObject.transform.position = PointB.transform.position;

                    WayPointId++;
                    if (WayPointId == WayPoints.Length) WayPointId = 0;
                    WayPointStart = Time.time;
                }
                else
                {
                    Vector3 Direction = (PointB.transform.position - PointA.transform.position).normalized;
                    gameObject.transform.LookAt(PointB.transform);
                    gameObject.transform.position = gameObject.transform.position + (Direction * Time.deltaTime * 10); // * Speed/100
                    /*
                    Vector3 Direction = (PointB.transform.position - PointA.transform.position) * Factor;
                    gameObject.transform.LookAt(PointB.transform);
                    gameObject.transform.position = PointA.transform.position + Direction;
                    */
                }
            }
        }
    }
}
