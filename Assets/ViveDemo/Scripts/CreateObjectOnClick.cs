using UnityEngine;
using System.Collections;
using VRTK;

namespace UWBNetworkingPackage.ViveDemo
{
    /// <summary>
    /// Script to be added to the Camera Rig's Right and Left controllers. Adds object instantiation
    /// functionality on touchpad press. 
    /// </summary>
    public class CreateObjectOnClick : VRTK_ControllerEvents
    {
        /// <summary>
        /// Suscribe OnTouchpadPressedHanlder to be called when the touchpad is pressed
        /// </summary>
        void Start()
        {
            TouchpadPressed += OnTouchpadPressedHandler;
        }

        /// <summary>
        /// When the touchpad is pressed, instantiate a cube, located at the origin of the game
        /// space
        /// </summary>
        /// <param name="obj">Reference to the controller that pressed the touchpad</param>
        /// <param name="e">Controller event arguments</param>
        public void OnTouchpadPressedHandler(object obj, ControllerInteractionEventArgs e)
        {
            //PhotonNetwork.Instantiate("Trophy", Vector3.zero, Quaternion.identity, 0);
        }
    }
}
