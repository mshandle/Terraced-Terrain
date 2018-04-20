//-----------------------------------------------------------------  
//1，把本类作为一个组件，包含在 GameObject 中。  
//2，左手坐标系。  
//-----------------------------------------------------------------  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//-----------------------------------------------------------------  
public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 30.0f;
    public float rotateSpeed = 0.2f;
    public float SrollSpeed = 10.0f;
    public float minY = 5.0f;
    public float maxY = 20.0F;
    public float minX = 40.0f;
    public float maxX = 240;
    public float minZ = 0;
    public float maxZ = 240;
    public float HeightChangeTime = 0.3f;
    public float HeightSpeed = 0.05f;

    public static Vector3 kUpDirection = new Vector3(0.0f, 1.0f, 0.0f);


    //控制摄像机旋转的成员变量。  
    private float m_fLastMousePosX = 0.0f;
    private float m_fLastMousePosY = 0.0f;
    private bool m_bMouseRightKeyDown = false;
    private float curDistan = 20;
    private Ray SrceenRay;
    private RaycastHit hitInfo;
    private float targetHeight = 0.0f;
    private Vector3 SceenCheckPoint;
  
    //-----------------------------------------------------------------  
    IEnumerator  Start () {
        SceenCheckPoint = new Vector3(Screen.width / 2.0f, Screen.height / 3.0f, 0.0f);
        yield return new WaitForEndOfFrame();
        SrceenRay = Camera.main.ScreenPointToRay(SceenCheckPoint);
        if(Physics.Raycast(SrceenRay, out hitInfo, 1000.0f))
        {
            targetHeight = transform.position.y;
            curDistan = transform.position.y - hitInfo.point.y;
        }
   	}
    private void Update()
    {

        //判断旋转  
        if (Input.GetMouseButtonDown(1)) //鼠标右键刚刚按下了  
        {
            if (m_bMouseRightKeyDown == false)
            {
                m_bMouseRightKeyDown = true;
                Vector3 kMousePos = Input.mousePosition;
                m_fLastMousePosX = kMousePos.x;
                m_fLastMousePosY = kMousePos.y;
            }
        }
        else if (Input.GetMouseButtonUp(1)) //鼠标右键刚刚抬起了  
        {
            if (m_bMouseRightKeyDown == true)
            {
                m_bMouseRightKeyDown = false;
                m_fLastMousePosX = 0;
                m_fLastMousePosY = 0;
            }
        }
        else if (Input.GetMouseButton(1)) //鼠标右键处于按下状态中  
        {
            if (m_bMouseRightKeyDown)
            {
                Vector3 kMousePos = Input.mousePosition;
                float fDeltaX = kMousePos.x - m_fLastMousePosX;
                float fDeltaY = kMousePos.y - m_fLastMousePosY;
                m_fLastMousePosX = kMousePos.x;
                m_fLastMousePosY = kMousePos.y;


                Vector3 kNewEuler = transform.eulerAngles;
                kNewEuler.x -= (fDeltaY * rotateSpeed);
                kNewEuler.y += -(fDeltaX * rotateSpeed);
                transform.eulerAngles = kNewEuler;
            }
        }



        //判断位移  
        float fMoveDeltaX = 0.0f;
        float fMoveDeltaZ = 0.0f;
        float fMoveDeltaY = 0.0f;
        float fMoveSroll = 0.0f;
        float fDeltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            fMoveDeltaX -= moveSpeed * fDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            fMoveDeltaX += moveSpeed * fDeltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            fMoveDeltaZ += moveSpeed * fDeltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            fMoveDeltaZ -= moveSpeed * fDeltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            fMoveDeltaY -= moveSpeed * fDeltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            fMoveDeltaY += moveSpeed * fDeltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            fMoveSroll -= SrollSpeed * fDeltaTime;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            fMoveSroll += SrollSpeed * fDeltaTime;
        }

        float distance = targetHeight - transform.position.y;
        if(distance != 0)
        {
            float det =  Time.unscaledDeltaTime * HeightSpeed;
            Vector3 curPos = transform.position;
            if (Mathf.Abs(distance) < det)
            {
                curPos.y = targetHeight;
                transform.position = curPos;
                return;
            }
            else if(distance < 0)
            {
                curPos.y -= det;
                transform.position = curPos;
            }
            else
            {
                curPos.y += det;
                transform.position = curPos;
            }

        }
        //if(distance >)

        if (fMoveDeltaX != 0.0f || fMoveDeltaZ != 0.0f || fMoveDeltaY != 0.0f)
        {
            //Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance);
            // gameObject.transform.forward;
            SrceenRay = Camera.main.ScreenPointToRay(SceenCheckPoint);
            if (Physics.Raycast(SrceenRay, out hitInfo,1000))
            {
                float TerrainHeight = hitInfo.point.y;
                Vector3 kNewPos = transform.position;
                kNewPos.y = hitInfo.point.y + curDistan;

                Vector3 kForward = transform.forward;
                kForward.y = 0.0F;
                Vector3 kRight = Vector3.Cross(kUpDirection, kForward);
                kNewPos += kRight * fMoveDeltaX;
                kNewPos += kForward * fMoveDeltaZ;
                kNewPos += kUpDirection * fMoveDeltaY;

                Vector3 kForwardExs = transform.forward;
                kNewPos += kForwardExs * fMoveSroll;
                if ((kNewPos.y  - hitInfo.point.y) < minY || (kNewPos.y - hitInfo.point.y ) > maxY)
                    return;

                if (kNewPos.x < minX || kNewPos.x > maxX)
                    return;
                if (kNewPos.z < minZ || kNewPos.z > maxZ)
                    return;
                curDistan = kNewPos.y - hitInfo.point.y;

                //Step close
                targetHeight = kNewPos.y;
                kNewPos.y = transform.position.y;
                HeightSpeed = Mathf.Abs(targetHeight - kNewPos.y) / HeightChangeTime;
                transform.position = kNewPos;

            }
            
        }

        if(fMoveSroll != 0.0f)
        {
            Vector3 kNewPos = transform.position;
            Vector3 kForwardExs = transform.forward;
            kNewPos += kForwardExs * fMoveSroll;

            if ((kNewPos.y - hitInfo.point.y) < minY || (kNewPos.y - hitInfo.point.y) > maxY)
                return;
            if (kNewPos.x < minX || kNewPos.x > maxX)
                return;
            if (kNewPos.z < minZ || kNewPos.z > maxZ)
                return;

            curDistan = kNewPos.y - hitInfo.point.y;
            targetHeight = kNewPos.y;
            transform.position = kNewPos;

        }
    }
    //-----------------------------------------------------------------  
}
//-----------------------------------------------------------------