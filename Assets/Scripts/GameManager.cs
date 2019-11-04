using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Hattimatim.BWMG
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat($"OnPlayerEnteredRoom() {newPlayer.NickName}");

            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat($"OnPlayerEnteredRoom IsMasterClient {PhotonNetwork.IsMasterClient}");
                LoadLevel();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat($"OnPlayerLeftRoom IsMasterClient {PhotonNetwork.IsMasterClient}");
                
                //Needs change. Must show result scene
                //SceneManager.LoadScene(2);
                //PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
            //PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        #endregion

        #region Public Methods

        public void OnClickLeaveRoom()
        {
            Debug.Log("Leaving Room");
            //SceneManager.LoadScene(0);
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        void LoadLevel()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to load a level but we are not master client");
            }
            Debug.LogFormat($"PhotonNetwork : Loading Level ");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                //PhotonNetwork.LoadLevel("testMode");
                PhotonNetwork.LoadLevel("testNetwork");
        }

        #endregion


    }
}
