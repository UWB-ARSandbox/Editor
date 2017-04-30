using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for
        public bool jumpFlag = false;
        public bool jumpWait = false;
        public bool crouchFlag = false;


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }


        private void Update()
        {
            if (target != null)
                agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                if (!jumpWait)
                {
                    character.Move(agent.desiredVelocity, crouchFlag, jumpFlag);
                    if (jumpFlag)
                    {
                        jumpWait = true;
                    }
                }
                else if(!jumpFlag)
                {
                    jumpWait = false;
                }
            }
            else
            {
                character.Move(Vector3.zero, crouchFlag, false);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        [PunRPC]
        public void SetTargetPos(Vector3 newPos)
        {
            if(this.target != null)
            {
                target.position = newPos;
            }
            else
            {
                GameObject newTarget = new GameObject();
                newTarget.transform.position = newPos;
                this.SetTarget(newTarget.transform);
            }
        }

        [PunRPC]
        public void SetCrouching(bool crouch)
        {
            crouchFlag = crouch;
        }
    }
}
