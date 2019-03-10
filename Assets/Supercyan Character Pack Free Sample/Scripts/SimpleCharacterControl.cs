using UnityEngine;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class SimpleCharacterControl : MonoBehaviour {

    private enum ControlMode
    {
        Tank,
        Direct
    }

    [SerializeField] private float m_moveSpeed = 0.0001f;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private bool m_moveBegin;
    private bool m_moveLeft;
    private bool m_moveRight;
    private float horizVel = 0;
    private int laneNum = 2;
    private string controlLocked = "n";
    private bool m_turnLeft;
    private bool m_turnRight;
    private float angle = 0;

    //private MyARAnchorManager unityARAnchorManager;
    //private ARPlaneAnchor unityARAnchor;


    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

	void Update () {
        m_animator.SetBool("Grounded", m_isGrounded);

        switch(m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
    }

    private void TankUpdate()
    {
        //float v = Input.GetAxis("Vertical");
        //float h = Input.GetAxis("Horizontal");

        //bool walk = Input.GetKey(KeyCode.LeftShift);

        //if (v < 0) {
        //    if (walk) { v *= m_backwardsWalkScale; }
        //    else { v *= m_backwardRunScale; }
        //} else if(walk)
        //{
        //    v *= m_walkScale;
        //}

        //m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        //m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        //transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        //transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        //m_animator.SetFloat("MoveSpeed", m_currentV);

        //JumpingAndLanding();

        //Debug.Log("2: " + m_moveBegin);

        //float v = Input.GetAxis("Vertical");
        //float h = Input.GetAxis("Horizontal");

        if (m_moveBegin == true)
        {

            //v *= m_walkScale;
            //v *= m_runScale;

            m_currentV = Mathf.Lerp(m_currentV, 0.1f, Time.deltaTime * m_interpolation);
            Debug.Log(m_currentV);
            //m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

            //transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
            //transform.position += transform.forward * m_moveSpeed * Time.deltaTime;
            transform.position += transform.forward * 0.2f * Time.deltaTime;

            //transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

            m_animator.SetFloat("MoveSpeed", 0.1f);

            //unityARAnchorManager = new MyARAnchorManager();
            //unityARAnchorManager.planeAnchorMap.ARPlaneAnchorGameObject
            //unityPlaneAnchor = new ARPlaneAnchor();
            //unityPlaneAnchor.center;

            //horizVel = 0;
            //GetComponent<Rigidbody>().velocity = new Vector3(horizVel, 0, 0.1f);
        }
        else
        {
            m_animator.SetFloat("MoveSpeed", 0);
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            transform.Rotate(0, 0, 0);
        }


        if ((m_moveLeft == true) && (laneNum > 1) && (controlLocked == "n"))
        {
            horizVel = -0.3f;
            GetComponent<Rigidbody>().velocity = new Vector3(horizVel, 0, 0.1f);
            //stopSlide();
            horizVel = 0;
            laneNum -= 1;
            controlLocked = "y";
        }
        if ((m_moveRight == true) && (laneNum <= 3) && (controlLocked == "n"))
        {
            horizVel = 0.3f;
            GetComponent<Rigidbody>().velocity = new Vector3(horizVel, 0, 0.1f);
            //stopSlide();
            horizVel = 0;
            laneNum += 1;
            controlLocked = "y";
        }
        controlLocked = "n";

        if (m_turnLeft == true)
        {
            //m_currentH = Mathf.Lerp(m_currentH, 0.5f, Time.deltaTime * m_interpolation);

            transform.Rotate(0, -90, 0);
            //transform.Rotate(0, 0, 0);
            m_turnLeft = false;
            //angle = m_currentH * m_turnSpeed * Time.deltaTime;  
            //if (angle <= 90)
            //{
            //    transform.Rotate(0, -90, 0);
            //}
            //else
            //{
            //    angle = 0;
            //    transform.Rotate(0, angle, 0);
            //}
        }
        if (m_turnRight == true)
        {
            //m_currentH = Mathf.Lerp(m_currentH, 0.5f, Time.deltaTime * m_interpolation);

            transform.Rotate(0, 90, 0);
            //transform.Rotate(0, 0, 0);
            m_turnRight = false;

            //angle = m_currentH * m_turnSpeed * Time.deltaTime;
            //if (angle <= 90)
            //{
            //    transform.Rotate(0, 90, 0);
            //}
            //else
            //{
            //    angle = 0;
            //    transform.Rotate(0, angle, 0);
            //}
        }
    }


    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if(direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }

        JumpingAndLanding();
    }

    public void randomMove()
    {
        JumpingAndLanding();

        m_moveBegin = true;
        Debug.Log("1: " + m_moveBegin);

        m_moveLeft = false;
        m_moveRight = false;
    }

    public void randomStop()
    {
        m_moveBegin = false;
    }

    public void randomShiftL()
    {
        m_moveLeft = true;
        m_moveRight = false;
    }

    public void randomShiftR()
    {
        m_moveRight = true;
        m_moveLeft = false;
    }

    private void stopSlide()
    {
        //yield return new WaitForSeconds(0.1f);
        horizVel = 0;
        controlLocked = "n";
    }

    public void randomTurnL()
    {
        m_turnLeft = true;
        m_turnRight = false;
    }

    public void randomTurnR()
    {
        m_turnRight = true;
        m_turnLeft = false;
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
}
