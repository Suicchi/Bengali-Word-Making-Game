using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ban_u2a;
using TMPro;

public class NewModeInput : MonoBehaviourPun
{
    const byte EVENT_CHANGE_INPUT = 0;
    string dictfile = Application.streamingAssetsPath + "/sorted159kwords.json";
    string currentWord = string.Empty;
    [SerializeField]
    GameObject gridtmpprefab;


    [PunRPC]
    void GenerateInputByMaster()
    {
        Debug.Log("GenerateInputByMaster() was called");
        var inputList = GenerateInput();

        //We raise an event so that everyone changes input boxes
        //RaiseEvent Codes

        object datas = new object[] { inputList.ToArray(), currentWord };
        PhotonNetwork.RaiseEvent(EVENT_CHANGE_INPUT, datas, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);

    }

    List<string> GenerateInput()
    {
        var random = new System.Random();
        string words = "";
        List<string> dividedWords = new List<string>();
        var currentNumber = random.Next(1, 6);
        Dictionary<string, List<string>> elist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));

        int rand = random.Next(elist[currentNumber.ToString()].Count);
        words += elist[currentNumber.ToString()][rand];
        dividedWords = BanglaHandler.DividedWords(words);
        var u2b = new UniToBijoy();
        for (int i = 0; i < dividedWords.Count; i++)
        {
            dividedWords[i] = u2b.Convert(dividedWords[i]);
        }
        currentWord = words;
        return dividedWords;
    }


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
            //var arrayLength = (int)data[1];
            FillInputBox(inputArray);
        }
    }

    private void FillInputBox(string[] inputArray)
    {
        if(this.transform.childCount > 0)
        {
            for(int i = 0; i< this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i));
            }
        }

        for(int i=0; i<inputArray.Length; i++)
        {
            GameObject temp = Instantiate(gridtmpprefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = inputArray[i];
            temp.transform.SetParent(this.transform, false);
        }

    }


}
