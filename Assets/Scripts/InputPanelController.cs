using Assets.Scripts;
using ban_u2a;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Com.Hattimatim.BWMG
{
    public class InputPanelController : MonoBehaviourPun
    {

        #region Event Codes

        private const byte EVENT_CHANGE_INPUT = 0;

        #endregion

        #region Private objects

        public GameObject[] inputBoxes = new GameObject[6];
        [SerializeField]
        GameObject tmpPrefab;

        GameObject Instantiator;

        //Dictionary
        string dictfile = Application.streamingAssetsPath +"/sorted159kwords.json";


        #endregion

        #region Public Functions

        public void RestorePanel(string[] inputboxes)
        {
            List<string> InputBoxes = new List<string>();
            foreach(var s in inputboxes)
            {
                InputBoxes.Add(s);
            }
            FillInputBoxes(InputBoxes);
        }

        #endregion

        #region Private Functions

        List<string> GenerateInput()
        {
            /**
             * Todo:
             * if state is empty, then select a random number among 1-6
             * if the number 'n' is less than 6 then select another number between 1 to 6-n
             * keep repeating till the numbers do not total 6
             * now for each number, open the json and select a random word containing the equal amount of parts as the number
             * Convert the unicode value to Bijoy
             * Put it into input box
             */
            // if(stateEmpty)
            //{
            var x = 0; // for debugging purpose
            Debug.Log("GenerateInput() was called");
            var random = new System.Random();
            var total = 0;
            string words = "";
            List<string> dividedWords = new List<string>();
            //Debug.Log($"Still working {++x}");
            Dictionary<string, List<string>> elist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));
            //Debug.Log($"Still working {++x}");
            List<int> wordPartsNumber = new List<int>();
            //Debug.Log($"Still working {++x}");
            var u2b = new UniToBijoy();
            //Debug.Log($"Still working {++x}");
            while (total < 6)
            {
                var currentNumber = random.Next(1, 6 - total);
                wordPartsNumber.Add(currentNumber);
                total += currentNumber;
            }

            foreach (var item in wordPartsNumber)
            {
                int rand = random.Next(elist[item.ToString()].Count);
                words += elist[item.ToString()][rand];
            }
            // Divide the words into parts if parts is greater than one
            dividedWords = BanglaHandler.DividedWords(words);

            foreach (var part in dividedWords)
            {
                Debug.Log(part);
            }

            //Convert them to Bijoy
            for (int i = 0; i < dividedWords.Count; i++)
            {
                dividedWords[i] = u2b.Convert(dividedWords[i]);
            }

            foreach (var part in dividedWords)
            {
                Debug.Log(part);
            }

            return dividedWords;

        }



        void FillInputBoxes(List<string> InputBoxes)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject temp = Instantiate(tmpPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                Destroy(inputBoxes[i]);
                //Debug.Log(inputBoxes[i].name);

                temp.GetComponentInChildren<TextMeshProUGUI>().text = InputBoxes[i];

                temp.transform.SetParent(this.transform, false);
                inputBoxes[i] = temp;

            }
        }




        #endregion

        #region Monobehaviour Callbacks

        private void Awake()
        {
            var inputPanel = this.gameObject;
            //We are getting the slots into the object variable for future use 
            if (inputPanel.transform.childCount > 0)
            {
                for (int i = 0; i < inputPanel.transform.childCount; i++)
                    inputBoxes[i] = inputPanel.transform.GetChild(i).gameObject;
            }
            //Shuffle at start
            inputPanel.GetPhotonView().RPC("GenerateInputByMaster", RpcTarget.MasterClient);
            Instantiator = GameObject.Find("Checker");

        }

        #endregion

        #region Photon RPC Functions

        [PunRPC]
        void GenerateInputByMaster()
        {
            Debug.Log("GenerateInputByMaster() was called");
            var inputList = GenerateInput();
            
            //We raise an event so that everyone changes input boxes
            //RaiseEvent Codes
            object datas = new object[] { inputList.ToArray() };
            PhotonNetwork.RaiseEvent(EVENT_CHANGE_INPUT, datas, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);

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
                object[] data = (object[])obj.CustomData;
                var inputArray = (string[])data[0];
                List<string> inputlist = inputArray.OfType<string>().ToList();
                FillInputBoxes(inputlist);
            }
        }

        #endregion


    }
}
