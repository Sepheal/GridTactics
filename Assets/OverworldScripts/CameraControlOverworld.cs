using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlOverworld : MonoBehaviour
{
    public GameObject Target;

    public Vector3 TargetPos = new Vector3(0, 0, 0), StartTranPos, CurrentPos = new Vector3(0, 0, 0);
    public float TargetRotateValueY = 45, TargetRotateValueZ = 35;
    float StartRotateValue = 0, AmountToRotate = 0, QueuedAmountToRotate = 0;
    int RotateCounter = 25;
    public float CamHeight = 23, CamDistance = 25;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetButtonDown("Rotate"))
        {
            if (RotateCounter >= 20)
            {
                StartRotateValue = TargetRotateValue;
                RotateCounter = 0;
                if (Input.GetAxis("Rotate") > 0f)
                {
                    TargetRotateValue += 90;
                    AmountToRotate = 90;
                }
                else
                {
                    TargetRotateValue -= 90;
                    AmountToRotate = -90;
                }
            }
            else if (QueuedAmountToRotate == 0)
            {
                if (Input.GetAxis("Rotate") > 0f)
                {
                    QueuedAmountToRotate = 90;
                }
                else
                {
                    QueuedAmountToRotate = -90;
                }
            }
        }*/

        Quaternion MyRotation;
        Vector3 MyPosition;
        if (RotateCounter < 20)
        {
            MyRotation = Quaternion.Euler(0, StartRotateValue + ((AmountToRotate / 20) * RotateCounter), 0);
            RotateCounter++;
        }
        else
        {
            MyRotation = Quaternion.Euler(0, TargetRotateValueY, 0);
            if (QueuedAmountToRotate != 0)
            {
                StartRotateValue = TargetRotateValueY;
                RotateCounter = 0;
                TargetRotateValueY += QueuedAmountToRotate;
                AmountToRotate = QueuedAmountToRotate;
                QueuedAmountToRotate = 0;
            }
        }
        if ((Target.transform.position - CurrentPos).magnitude > 0.1f)
        {
            Vector3 Dir = (Target.transform.position - CurrentPos) / 20;
            MyPosition = new Vector3(CurrentPos.x + Dir.x, CurrentPos.y + Dir.y, CurrentPos.z + Dir.z);
            CurrentPos = new Vector3(CurrentPos.x + Dir.x, CurrentPos.y + Dir.y, CurrentPos.z + Dir.z);
        }
        else
        {
            MyPosition = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
            CurrentPos = Target.transform.position;
        }

        ////////SetTheBugger
        transform.localRotation = MyRotation;
        transform.localPosition = MyPosition;
        transform.Translate(0, CamHeight, -CamDistance, Space.Self);
        transform.Rotate(TargetRotateValueZ, 0, 0);

        //Occlusion
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("BackgroundOcclude"))
        {
            item.GetComponent<MeshRenderer>().material.color = new Color32(255, 255, 255, 255);
        }

        Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 35);
        foreach (RaycastHit hit in Physics.RaycastAll(ray, 35f))
        {
            if (hit.collider.CompareTag("BackgroundOcclude"))
            {
                //print(hit.collider.gameObject);
                if (hit.collider.GetComponent<MeshRenderer>())
                {
                    //hit.collider.GetComponent<MeshRenderer>().material.color = new Color32(255, 255, 255, 50);
                }
            }
        }
    }
}
