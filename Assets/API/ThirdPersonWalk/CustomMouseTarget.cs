using System;
using UnityEngine;


namespace UnityStandardAssets.SceneUtils
{
    public class CustomMouseTarget : MonoBehaviour
    {
        public float surfaceOffset = 0.01f;
        public GameObject setTargetOn;

        private void Start()
        {
            if(setTargetOn == null)
            {
                setTargetOn = GameObject.Find("AIThirdPersonController");

                if(setTargetOn == null)
                {
                    Debug.LogError("Mouse Target not set to a value");
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }
            transform.position = hit.point + hit.normal*surfaceOffset;
            if (setTargetOn != null)
            {
                setTargetOn.SendMessage("SetTarget", transform);
            }

            // Send the position over the network
            sendPosition();
        }

        private void sendPosition()
        {
            Vector3 newPosition = transform.position;

            PhotonView pV = setTargetOn.transform.GetComponent<PhotonView>();
            pV.RPC("SetTargetPos", PhotonTargets.All, newPosition);
        }
    }
}
