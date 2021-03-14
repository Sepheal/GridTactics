using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Vector3 TargetPos = new Vector3(0, 0, 0), StartTranPos, CurrentPos = new Vector3(0, 0, 0);
    public bool IsPlayerTurn = true;
    public float TargetRotateValue = 45;
    float StartRotateValue = 45, AmountToRotate = 0, QueuedAmountToRotate = 0;
    int RotateCounter = 25;
    public GameObject Diamond, FollowObject;
    public float CamHeight = 30, CamDistance = 30;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ResetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Rotate"))
        {
            if (RotateCounter >= 20)
            {
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
            else if (QueuedAmountToRotate == 0)
            {
                if (Input.GetAxis("Rotate") > 0f)
                {
                    QueuedAmountToRotate = 90;
                    Diamond.transform.Rotate(0, 90, 0, Space.Self);
                }
                else
                {
                    QueuedAmountToRotate = -90;
                    Diamond.transform.Rotate(0, -90, 0, Space.Self);
                }
            }
        }

        Quaternion MyRotation;
        Vector3 MyPosition;
        if (RotateCounter < 20)
        {
            MyRotation = Quaternion.Euler(0, StartRotateValue + ((AmountToRotate / 20) * RotateCounter), 0);
            RotateCounter++;
        }
        else
        {
            MyRotation = Quaternion.Euler(0, TargetRotateValue, 0);
            if (QueuedAmountToRotate != 0)
            {
                StartRotateValue = TargetRotateValue;
                RotateCounter = 0;
                TargetRotateValue += QueuedAmountToRotate;
                AmountToRotate = QueuedAmountToRotate;
                QueuedAmountToRotate = 0;
            }
        }
        if ((FollowObject.transform.position - CurrentPos).magnitude > 1)
        {
            Vector3 Dir = (FollowObject.transform.position - CurrentPos) / 25;
            MyPosition = new Vector3(CurrentPos.x + Dir.x, CamHeight, CurrentPos.z + Dir.z);
            CurrentPos = new Vector3(CurrentPos.x + Dir.x, CamHeight, CurrentPos.z + Dir.z);
        }
        else
        {
            MyPosition = new Vector3(FollowObject.transform.position.x, CamHeight, FollowObject.transform.position.z);
            CurrentPos = FollowObject.transform.position;
        }

        ////////SetTheBugger
        transform.localRotation = MyRotation;
        transform.localPosition = MyPosition;
        transform.Translate(0, 0, -CamDistance, Space.Self);
        transform.Rotate(35, 0, 0);
    }

    void SetTransform(GameObject FocusObj)
    {
        FollowObject = FocusObj;
    }

    public void SetTarget(GameObject NewObj)
    {
        SetTransform(NewObj);
    }

    public void ResetTarget()
    {
        FollowObject = Diamond;
    }
}
