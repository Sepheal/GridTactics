using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Vector3 TargetPos = new Vector3(0, 0, 0), StartTranPos, CurrentPos = new Vector3(0, 0, 0);
    bool Transition = true;
    public bool IsPlayerTurn = true;
    float TargetRotateValue = 45, StartRotateValue = 45, AmountToRotate = 0;
    int RotateCounter = 25, TransitionCounter = 250;
    public GameObject Diamond, FollowObject;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        FollowObject = Diamond;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Rotate"))
        {
            Quaternion TestQ = Quaternion.Euler(35, TargetRotateValue, 0);
            transform.localRotation = TestQ;
            StartRotateValue = TargetRotateValue;
            RotateCounter = 0;
            if (Input.GetAxis("Rotate") > 0f)
            {
                TargetRotateValue += 90;
                AmountToRotate = 90;
                Diamond.transform.Rotate(0, 90, 0, Space.Self);
            }
            else
            {
                TargetRotateValue -= 90;
                AmountToRotate = -90;
                Diamond.transform.Rotate(0, -90, 0, Space.Self);
            }
        }

        Quaternion MyRotation = new Quaternion();
        Vector3 MyPosition; // = new Vector3(TargetPos.x, 25, TargetPos.z);
        if (RotateCounter < 25)
        {
            MyRotation = Quaternion.Euler(0, StartRotateValue + ((AmountToRotate / 25) * RotateCounter), 0);

            RotateCounter++;
        }
        else
        {
            MyRotation = Quaternion.Euler(0, TargetRotateValue, 0);
        }
        if ((FollowObject.transform.position - CurrentPos).magnitude > 1)
        {
            Vector3 Dir = (FollowObject.transform.position - CurrentPos) / 25;
            MyPosition = new Vector3(CurrentPos.x + Dir.x, 25, CurrentPos.z + Dir.z);
            CurrentPos = new Vector3(CurrentPos.x + Dir.x, 25, CurrentPos.z + Dir.z);
        }
        else
        {
            MyPosition = new Vector3(FollowObject.transform.position.x, 25, FollowObject.transform.position.z);
            CurrentPos = FollowObject.transform.position;
        }

        ////////SetTheBugger
        transform.localRotation = MyRotation;
        transform.localPosition = MyPosition;
        transform.Translate(0, 0, -25, Space.Self);
        transform.Rotate(35, 0, 0);
    }

    void SetTransform(GameObject FocusObj)
    {
        //TransitionCounter = 0;
        //Transition = true;
        //StartTranPos = TargetPos;
        //TargetPos = FocusPos;
        FollowObject = FocusObj;
    }

    public void SetTarget(GameObject NewObj)
    {
        //TargetPos = T + new Vector3(-25, 35, -25);
        //TargetPos = T;
        Transition = true;
        SetTransform(NewObj);
    }

    public void ResetTarget()
    {
        FollowObject = Diamond;
    }
}
