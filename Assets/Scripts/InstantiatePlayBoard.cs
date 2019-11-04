using Assets.Scripts;
using ban_u2a;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
//using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Hattimatim.BWMG
{
    public class InstantiatePlayBoard : MonoBehaviourPun
    {
        #region Event Codes

        private const byte EVENT_CHANGE_INPUT = 0;
        private const byte EVENT_SYNC_PLAYBOARD = 1;
        private const byte EVENT_UPDATE_SCORE = 2;
        private const byte EVENT_CHANGE_TURN = 3;
        public static Assets.Scripts.Player player1 = new Assets.Scripts.Player(1, 0, true);
        public static Assets.Scripts.Player player2 = new Assets.Scripts.Player(2, 0, false);
        #endregion

        #region Private Variables

        [SerializeField]
        GameObject playboardPrefab, playboardContainer, inputPanelPrefab, inputPanelContainer, inputpanelGO, playboardGO, inputButtonsContainer, scoreboard1, scoreboard2;
        //bool stateEmpty;
        Photon.Realtime.Player currentPlayer;

        int resetPB = 0;

        #endregion

        #region Public Variables

        public static List<string> oldInputs = new List<string>();
        public static GameState matchState;

        #endregion

        #region Monobehavious Callbacks

        private void Awake()
        {
            //We instantiate the playboard for both players and set it in a place so that both players can see
            playboardGO = PhotonNetwork.Instantiate(playboardPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
            playboardGO.tag = "PlayBoardPanel";
            playboardGO.transform.SetParent(playboardContainer.transform, false);
            playboardGO.name = "PlayBoardPanel";
            //stateEmpty = true; // Flag to check if the state was empty

            //Same for the inputpanels
            inputpanelGO = PhotonNetwork.Instantiate(inputPanelPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
            inputpanelGO.tag = "InputPanel";
            inputpanelGO.transform.SetParent(inputPanelContainer.transform, false);
            inputpanelGO.name = "InputPanel";

            PhotonNetwork.AutomaticallySyncScene = true;

            //We instantiate the players and match state and save them locally
            
            matchState = new GameState(player1, player2);
            matchState.stateUpdate(inputpanelGO.GetComponent<InputPanelController>().inputBoxes);
            matchState.stateUpdate(playboardGO.GetComponent<PlayboardManager>().boxes);
            SaveSystem.Save(matchState);

            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                inputButtonsContainer.SetActive(false);
                //inputpanelGO.SetActive(false);
            }

             currentPlayer = PhotonNetwork.PlayerListOthers[0];

        }

        private void Update()
        {
            if (player1.score >=25)
            {
                PhotonNetwork.LoadLevel("Victory");
            }
            else if(player2.score >= 25)
            {
                PhotonNetwork.LoadLevel("Defeat");
            }
        }

        #endregion

        #region Private Functions



        #endregion

        #region Public Functions

        public void OnClickShuffleInputs()
        {
            //We call the masterclient's RPC to generate a string for us.
            inputpanelGO.GetPhotonView().RPC("GenerateInputByMaster", RpcTarget.MasterClient);
        }

        public void OnClickCheckPlayBoardPanel()
        {
            // Call checking function
            //var playBoardStrings = 
            playboardGO.GetComponent<PlayboardManager>().Check();
            //if(playBoardStrings == null)
            //{
            //    //Do nothing

            //}
            //else
            //{
            //    //Send an RPC in all clients to update the playboard
            //    playboardGO.GetPhotonView().RPC("ChangeValues", RpcTarget.Others, playBoardStrings );
            //}

            //// Then Save on all clients
        }

        public void OnClickLoadPlayBoardPanel()
        {
            //Call masterclient to load
        }

        public void OnClickRestoreState()
        {
            Debug.Log("OnClickRestoreState() was called");
            matchState = SaveSystem.Load();
            //Update the inputboxes
            inputpanelGO.GetComponent<InputPanelController>().RestorePanel(matchState.InputBoxes);
            //Update the Playboardboxes
            playboardGO.GetComponent<PlayboardManager>().RestorePanel(matchState.Boxes);
            //Update the oldinputs
            oldInputs = matchState.OldInputs;
            //clear newinput list
            DragHandler.newInput.Clear();
            PhotonNetwork.RaiseEvent(EVENT_CHANGE_TURN, new object[] { }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

        }

        public void UpdateInputState()
        {
            Debug.Log("UpdateInputState() was called");
            matchState.stateUpdate(inputpanelGO.GetComponent<InputPanelController>().inputBoxes);
            SaveSystem.Save(matchState);
        }

        public void UpdatePlayBoardState()
        {
            Debug.Log("UpdatePlayBoardState() was called");
            matchState.stateUpdate(playboardGO.GetComponent<PlayboardManager>().boxes);
            SaveSystem.Save(matchState);
        }

        public void UpdateScore(int number)
        {
            //if(currentPlayer == PhotonNetwork.LocalPlayer)
            //{
            //    player1.score += number;
            //    Debug.Log(player1.score);
            //    scoreboard1.GetComponent<Text>().text = player1.score.ToString();
            //}
            //else
            //{
            //    player2.score += number;
            //    scoreboard2.GetComponent<Text>().text = player2.score.ToString();
            //}
            player2.score += number;
            scoreboard2.GetComponent<Text>().text = player2.score.ToString();
            //matchState.stateUpdate(player1)
            inputpanelGO.GetPhotonView().RPC("GenerateInputByMaster", RpcTarget.MasterClient);
        }

        public void ChangeTurn()
        {
            if(inputButtonsContainer.activeInHierarchy)
            {
                inputButtonsContainer.SetActive(false);
                //inputpanelGO.SetActive(false);
            }
            else
            {
                inputButtonsContainer.SetActive(true);
                //inputpanelGO.SetActive(true);
            }

            if (currentPlayer == PhotonNetwork.PlayerListOthers[0])
            {
                currentPlayer = PhotonNetwork.LocalPlayer;
            }
            else
                currentPlayer = PhotonNetwork.PlayerListOthers[0];
        }

        #endregion

        #region Event Listeners

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }

        private void NetworkingClient_EventReceived(EventData obj)
        {
            if (obj.Code == EVENT_CHANGE_INPUT)
            {
                UpdateInputState();
            }
            if(obj.Code == EVENT_SYNC_PLAYBOARD)
            {
                UpdatePlayBoardState();

            }
            if (obj.Code == EVENT_UPDATE_SCORE)
            {
                Debug.Log($" Score increases by: {5}");
                UpdateScore(5);
                PhotonNetwork.RaiseEvent(EVENT_CHANGE_TURN, new object[] {  }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
            if (obj.Code == EVENT_CHANGE_TURN)
            {
                //object[] data = (object[])obj.CustomData;
                //if(data.Length>0)
                //    resetPB += (int)data[0];
                //if(resetPB == 2)
                //{
                //    //Reset Playboard
                //}
                Debug.Log("Changing Turn");
                ChangeTurn();
            }
        }


        #endregion

    }
}
