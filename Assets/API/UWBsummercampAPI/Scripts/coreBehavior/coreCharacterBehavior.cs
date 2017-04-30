using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class coreCharacterBehavior : MonoBehaviour
{
    // Enums
	protected enum direction {forward, back, left, right};
	protected enum sizes {small, normal, big};

    // Flags
    protected bool isMovingFlag = false;
    protected bool isJumpingFlag = false;
    protected bool isCrouchingFlag = false;

    // Static Variables
    NavMeshAgent agent;
    AICharacterControl charControl;
    PhotonView pV;

    // Dynamic Variables
    protected direction movingDirection = direction.forward;
	protected sizes characterSize = sizes.normal;

    // Use this for initialization
    void Start ()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        charControl = gameObject.GetComponent<AICharacterControl>();
        pV = transform.GetComponent<PhotonView>();

        MethodInfo method = this.GetType().GetMethod("buildGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        MethodInfo method = this.GetType().GetMethod("updateGame", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (method != null)
        {
            method.Invoke(this, new object[0]);
        }
    }



	protected void moveForward( int speed = 5)
    {

		movingDirection = direction.forward;

        if (agent == null)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else
        {
            // AICharacterControl.cs PunRPC
            pV.RPC("SetTargetPos", PhotonTargets.All, transform.position + Vector3.forward);
        }

	}

    public bool makeSmaller( )
    {
        if (characterSize == sizes.small)
        {
			return false;
		}

        // This file PunRPC
        pV.RPC("makeSmallerRPC", PhotonTargets.All);

        return true;
	}

    [PunRPC]
    public void makeSmallerRPC()
    {
        CapsuleCollider m_Capsule = GetComponent<CapsuleCollider>();
        if (characterSize == sizes.normal)
        {
            transform.localScale = new Vector3(.3f, .3f, .3f);
            characterSize = sizes.small;
            m_Capsule.height = m_Capsule.height / 3f;
            m_Capsule.center = m_Capsule.center / 3f;

        }
        else if (characterSize == sizes.big)
        {
            transform.localScale = new Vector3(1, 1, 1);
            characterSize = sizes.normal;
            m_Capsule.height = m_Capsule.height / 2f;
            m_Capsule.center = m_Capsule.center / 2f;
        }
    }

    public bool makeBigger()
    {

        if (characterSize == sizes.big)
        {
            return false;
        }

        // This file PunRPC
        pV.RPC("makeBiggerRPC", PhotonTargets.All);

        return true;
    }

    [PunRPC]
    public void makeBiggerRPC()
    {
        CapsuleCollider m_Capsule = GetComponent<CapsuleCollider>();
        if (characterSize == sizes.normal)
        {
            transform.localScale = new Vector3(2, 2, 2);
            characterSize = sizes.big;

            m_Capsule.height = m_Capsule.height * 2f;
            m_Capsule.center = m_Capsule.center * 2f;

        }
        else if (characterSize == sizes.small)
        {
            transform.localScale = new Vector3(1, 1, 1);
            characterSize = sizes.normal;

            m_Capsule.height = m_Capsule.height * 3f;
            m_Capsule.center = m_Capsule.center * 3f;
        }
    }

    public bool isMoving()
    {
        return isMovingFlag;
    }

    public void jump(int forceTMP = 10)
    {
        if (agent == null)
        {
            int force = forceTMP * 50;
            GetComponent<Rigidbody>().AddForce(0, force, 0);
        }
        else
        {
            pV.RPC("jumpAgent", PhotonTargets.All, forceTMP); // This file PunRPC
        }
    }

    [PunRPC]
    private void jumpAgent(int forceTMP)
    {
        isJumpingFlag = true;
        charControl.jumpFlag = true;
        Vector3 directiton = (charControl.target.position - transform.position).normalized;
        directiton.y = 0;
        charControl.target.position = transform.position + directiton * forceTMP;
        StartCoroutine(Parabola(GetComponent<NavMeshAgent>(), forceTMP / 3f, forceTMP / 6f));
    }

    // Method for jumpping in a 'parabola' motion, using a NavMeshAgent
    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = charControl.target.position + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        charControl.jumpFlag = false;
        isJumpingFlag = false;
    }

    public bool isJumping()
    {
        return isJumpingFlag;
    }

    public void crouch()
    {
        if (agent == null)
        {
            Debug.LogWarning("No agent - Could not crouch");
        }
        else
        {
            isCrouchingFlag = !isCrouchingFlag;
            pV.RPC("SetCrouching", PhotonTargets.All, isCrouchingFlag); // AICharacterControl.cs PunRPC
        }
    }

    public bool isCrouching()
    {
        return isCrouchingFlag;
    }
}
