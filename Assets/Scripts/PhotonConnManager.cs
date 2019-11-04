using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

namespace Com.Hattimatim.BWMG
{
    public class PhotonConnManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "0.0.33";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        #endregion

        #region Private Serializable Fields

        [SerializeField]
        private GameObject buttonContainer, gameModeSelector, waitingLabel, settingsPanel;
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            buttonContainer.SetActive(true);
            gameModeSelector.SetActive(false);
            waitingLabel.SetActive(false);
            settingsPanel.SetActive(false);
        }



        #endregion

        #region Public Functions
        public void OnClickConnectToPhoton()
        {
            isConnecting = true;
            buttonContainer.SetActive(false);
            gameModeSelector.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void OnClickChangeSettings()
        {
            buttonContainer.SetActive(false);
            settingsPanel.SetActive(true);
        }
        #endregion

        #region MonobehaviourPun Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            buttonContainer.SetActive(false);
            gameModeSelector.SetActive(true);
            Debug.LogWarningFormat($"OnDisconnected() was called by PUN with reason: {cause}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PhotonConnManager:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PhotonConnManager: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // #Critical: We only load if a 2nd player joins. We rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We wait for 2nd player");

                // #Critical
                // Show waiting for player message
                waitingLabel.SetActive(true);
            }
        }

        #endregion
    }
}
