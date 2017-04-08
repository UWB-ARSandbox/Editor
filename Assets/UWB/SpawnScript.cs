using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public bool sendColor = false;
    bool sent = false;
    public string prefabName = "Cube";

	// Update is called once per frame
	void Update ()
    {
        // If we've sent, skip this script
        if (sent)
            return;

        // If we're "fully" connected to a room
        if ((PhotonNetwork.connectionStateDetailed == ClientState.ConnectedToGameserver || PhotonNetwork.connectionStateDetailed == ClientState.Joined) && !PhotonNetwork.connecting)
        {
            // Send this object after 2 seconds
            StartCoroutine(Send());
            sent = true;
        }
	}

    IEnumerator Send()
    {
        yield return new WaitForSeconds(2); // Wait 2 seconds to ensure room connectivity

        // Create a list of ViewIds (for SendInstantiate)
        PhotonView[] views = gameObject.GetPhotonViewsInChildren();
        int[] viewIDs = new int[views.Length];
        for (int i = 0; i < viewIDs.Length; i++)
        {
            if(views[i].viewID == 0)
            {
                views[i].viewID = PhotonNetwork.AllocateViewID();
            }
            viewIDs[i] = views[i].viewID;
        }

        views[0].ownershipTransfer = OwnershipOption.Takeover; // Allow objects to be moved by anyone
        views[0].onSerializeTransformOption = OnSerializeTransform.All; // Indicate to PUN to track Position, Rotation, AND scale (defaults to no scale)

        // Send to others, create info
        ExitGames.Client.Photon.Hashtable instantiateEvent = PhotonNetwork.networkingPeer.SendInstantiate(prefabName, transform.position, transform.rotation, 0, viewIDs, null, false);
        gameObject.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(PhotonNetwork.networkingPeer.LocalPlayer, (int)instantiateEvent[(byte)6], null), SendMessageOptions.DontRequireReceiver);

        if (sendColor)
        {
            StartCoroutine(SendColor(views[0]));
        }
    }

    IEnumerator SendColor(PhotonView pv)
    {
        yield return new WaitForSeconds(0.5f);
        Color thisColor = gameObject.GetComponent<Renderer>().material.color;
        pv.RPC("ChangeColor", PhotonTargets.All, thisColor.r, thisColor.g, thisColor.b);
    }
}
