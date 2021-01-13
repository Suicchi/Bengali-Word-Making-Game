using Assets.Scripts;
using ban_u2a;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Hattimatim.BWMG
{
    public class PlayboardManager : MonoBehaviourPun
    {
        #region Event Codes

        const byte EVENT_SYNC_PLAYBOARD = 1;
        const byte EVENT_UPDATE_SCORE = 2;

        #endregion

        #region Private Variables

        public GameObject[,] boxes = new GameObject[7, 7];
        [SerializeField]
        GameObject tmpPrefab, nonTmpPrefab, scoreboard1;
        GameObject Instantiator;
        bool stateEmpty;
        string dictfile = Application.streamingAssetsPath + "/sorted159kwords.json";

        #endregion

        #region Monobehavious Callbacks
        // Start is called before the first frame update
        void Awake()
        {
            //Initialize the input boxes and play boxes
            GameObject playBoard = this.gameObject;

            playBoard.name = "PlayBoardPanel";
            //Debug.Log(playBoard.name);
            //We are getting the slots into the object variable for future use
            if (playBoard.transform.childCount > 0)
            {
                for (int i = 0, k = 0; k < playBoard.transform.childCount && i < 7; i++)
                {
                    for (int j = 0; j < 7; j++, k++)
                    {
                        playBoard.transform.GetChild(k).name = $"{i}{j}";
                        boxes[i, j] = playBoard.transform.GetChild(k).gameObject;
                        // Debug.Log(boxes[i,j].name);
                    }

                }

            }
            stateEmpty = true;
            Instantiator = GameObject.Find("Checker");
            scoreboard1 = GameObject.Find("SelfScore");
            if(InstantiatePlayBoard.oldInputs == null)
            {
                Debug.Log("Oldinputs is null for some reason");
                InstantiatePlayBoard.oldInputs = new List<string>();
            }
        }

        #endregion

        #region Photon RPC functions

        #endregion

        #region Private Functions

        void ChangeTo2DArray(string[] myArray)
        {
            string[,] playBoardStrings = new string[7, 7];
            Debug.Log("ChangeTo2DArray() was called");
            int k = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    playBoardStrings[i, j] = myArray[k];
                    if (k < myArray.Length - 1)
                    {
                        k++;
                    }
                }
                //Debug.Log(myArray[k]);
            }

            ChangeValues(playBoardStrings);
        }

        void ResetValues()
        {
            // for (int i=0; i<7; i++)
            // {
            //     for (int j = 0; j < 7; j++)
            //     {
            //         GameObject temp = Instantiate(nonTmpPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            //         Destroy(boxes[i, j]);
            //         //temp.GetComponentInChildren<TextMeshProUGUI>().text = childtexts[i, j];
            //         temp.transform.SetParent(this.transform, false);
            //         temp.name = $"{i}{j}";
            //         boxes[i, j] = temp;
            //     }
            // }

            //Initialize the input boxes and play boxes
            GameObject playBoard = this.gameObject;

            playBoard.name = "PlayBoardPanel";
            //Debug.Log(playBoard.name);
            //We are getting the slots into the object variable for future use
            if (playBoard.transform.childCount > 0)
            {
                for (int i = 0, k = 0; k < playBoard.transform.childCount && i < 7; i++)
                {
                    for (int j = 0; j < 7; j++, k++)
                    {
                        playBoard.transform.GetChild(k).name = $"{i}{j}";
                        boxes[i, j] = playBoard.transform.GetChild(k).gameObject;
                        // Debug.Log(boxes[i,j].name);
                    }

                }

            }
            stateEmpty = true;
        }

        void ChangeValues(string[,] childtexts)
        {
            Debug.Log("ChangeValues() was called");
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    //Debug.Log(childtexts[i, j]);
                    if (childtexts[i, j] != "\0")
                    {
                        GameObject temp = Instantiate(tmpPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        Destroy(boxes[i, j]);
                        temp.GetComponentInChildren<TextMeshProUGUI>().text = childtexts[i, j];
                        Destroy(temp.GetComponentInChildren<DragHandler>());
                        temp.transform.SetParent(this.transform, false);
                        temp.name = $"{i}{j}";
                        boxes[i, j] = temp;
                    }
                    else
                    {
                        GameObject temp = Instantiate(nonTmpPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        Destroy(boxes[i, j]);
                        //temp.GetComponentInChildren<TextMeshProUGUI>().text = childtexts[i, j];
                        temp.transform.SetParent(this.transform, false);
                        temp.name = $"{i}{j}";
                        boxes[i, j] = temp;
                    }
                }
            }
        }

        bool CheckExisting()
        {
            Debug.Log("CheckExisting() was called");
            string Word = String.Empty;
            string Word2 = String.Empty;
            // var spaceExists = false;
            bool wordfound = false;
            int length = 0;
            int length2 = 0;

            //Since there is already old values there we need to do something with them on this :D
            DragHandler.newInput.Sort();
             // We are getting null reference sometimes
            if(InstantiatePlayBoard.oldInputs != null)
            {
                InstantiatePlayBoard.oldInputs.Sort();
            }
            else
            {
                Debug.Log("oldInputs was not initialized");
            }
            var oList = InstantiatePlayBoard.oldInputs;
            var nList = DragHandler.newInput;

            //Create a temporary list for checking
            List<string> temporaryList = new List<string>();

            //These variables are declared in case there is an isolated case (single new input)
            int valuesAbove = 0;
            int valuesBelow = 0;
            int valuesLeft = 0;
            int valuesRight = 0;

            //try { 
            //Check if row or column matches if newInput is one
            if (DragHandler.newInput.Count == 1)
            {
                var rowMatch = false;
                var colMatch = false;
                //check for values
                var row = Int32.Parse(DragHandler.newInput[0][0].ToString());
                var col = Int32.Parse(DragHandler.newInput[0][1].ToString());

                // values before
                for( int i= Int32.Parse(DragHandler.newInput[0][1].ToString()) - 1; i>=0 ; i-- )
                {
                    if(oList.Contains($"{row}{i}"))
                    {
                        valuesLeft++;
                    }
                    else
                    {
                        break;
                    }
                }

                // values after
                for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) + 1; i < 7; i++)
                {
                    if (oList.Contains($"{row}{i}"))
                    {
                        valuesRight++;
                    }
                    else
                    {
                        break;
                    }
                }

                // values above
                for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()) - 1; i > -1; i--)
                {
                    if (oList.Contains($"{i}{col}"))
                    {
                        valuesAbove++;
                    }
                    else
                    {
                        break;
                    }
                }

                // values below
                for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()) + 1; i < 7; i++)
                {
                    if (oList.Contains($"{i}{col}"))
                    {
                        valuesBelow++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (valuesAbove == 0 && valuesBelow == 0 && valuesLeft == 0 && valuesRight == 0)
                {
                    //Error: Restore game state
                    Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                }
                else if( (valuesAbove > 0 || valuesBelow > 0) && (valuesLeft > 0 || valuesRight > 0))
                {
                    // We have two possible word combinations now
                    colMatch = true;
                    rowMatch = true;
                }
                else if (valuesAbove > 0 || valuesBelow > 0)
                {
                    colMatch = true;
                }
                else if (valuesLeft > 0 || valuesRight > 0)
                {
                    rowMatch = true;
                }

                if(rowMatch && !colMatch)
                {
                    Debug.Log("Row matches");
                    length = valuesLeft + 1 + valuesRight;
                    for(int i= col - valuesLeft, j= 0; j<length; i++, j++)
                    {
                        Word += boxes[row, i].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }
                else if(colMatch && !rowMatch)
                {
                    Debug.Log("Column matches");
                    length = valuesAbove + 1 + valuesBelow;
                    for (int i = row - valuesAbove, j=0; j<length; i++, j++)
                    {
                        Word += boxes[i, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }
                else if(rowMatch && colMatch)
                {
                    Debug.Log("Both matches");
                    length = valuesLeft + 1 + valuesRight;
                    for(int i= col - valuesLeft, j= 0; j<length; i++, j++)
                    {
                        Word += boxes[row, i].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                    length2 = valuesAbove + 1 + valuesBelow;
                    for (int i = row - valuesAbove, j=0; j<length2; i++, j++)
                    {
                        Word2 += boxes[i, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }
                else
                {
                    Debug.Log("Row or column doesn't match");
                    //Error: Restore game state
                    Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                }

                var b2u = new BijoyToUni();
                var unicodeWord = b2u.Convert(Word);
                Debug.Log(unicodeWord);
                string unicodeWord2 = String.Empty;
                if(Word2 != String.Empty)
                {
                    unicodeWord2 = b2u.Convert(Word2);
                }
                Debug.Log(unicodeWord2);

                //Check if the unicodeword exists in dictionary
                Dictionary<string, List<string>> elist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));

                if(Word2 != String.Empty)
                {
                    if (elist[length.ToString()].Contains(unicodeWord) || elist[length2.ToString()].Contains(unicodeWord2))
                    {
                        wordfound = true;
                        Debug.Log("we are putting new inputs into old ones");
                        InstantiatePlayBoard.oldInputs.AddRange(DragHandler.newInput);
                        InstantiatePlayBoard.oldInputs = InstantiatePlayBoard.oldInputs.Distinct().ToList();
                        DragHandler.newInput.Clear();
                    }
                }
                else 
                {
                    if (elist[length.ToString()].Contains(unicodeWord))
                    {
                        wordfound = true;
                        Debug.Log("we are putting new inputs into old ones");
                        InstantiatePlayBoard.oldInputs.AddRange(DragHandler.newInput);
                        InstantiatePlayBoard.oldInputs = InstantiatePlayBoard.oldInputs.Distinct().ToList();
                        DragHandler.newInput.Clear();
                    }
                    else
                    {
                        wordfound = false;
                        DragHandler.newInput.Clear();
                        Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                    }
                }
                
                Debug.Log(wordfound);
                return wordfound;
            }
            else if(DragHandler.newInput.Count > 1)
            {
                var rowMatch = true;
                var colMatch = true;
                //First check if new inputs are in same row or column
                var row = Int32.Parse(DragHandler.newInput[0][0].ToString());
                var col = Int32.Parse(DragHandler.newInput[0][1].ToString());
                Debug.Log(row);
                Debug.Log(col);
                for(int i=0; i<DragHandler.newInput.Count; i++)
                {
                    if(Int32.Parse(DragHandler.newInput[i][0].ToString()) != row)
                    {
                        rowMatch = false;
                        break;
                    }
                }

                for (int i = 0; i < DragHandler.newInput.Count; i++)
                {
                    if ( Int32.Parse(DragHandler.newInput[i][1].ToString()) != col)
                    {
                        colMatch = false;
                        break;
                    }
                }

                if(!rowMatch && !colMatch)
                {
                    Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                    DragHandler.newInput.Clear();
                }
                else if(rowMatch)
                {
                    //check for gaps among the new inputs
                    var gapExists = false;
                    for(int i=Int32.Parse(DragHandler.newInput[0][1].ToString()); i<= Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count-1][1].ToString()); i++ )
                    {
                        if (!oList.Contains($"{row}{i}"))
                        {
                            //Reset game state and break
                            //spaceExists = true;
                            if (!DragHandler.newInput.Contains($"{row}{i}"))
                            {
                                Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                                gapExists = true;
                                break;
                            }
                            else
                            {
                                length++;
                            }
                        }
                        else
                        {
                            length++;
                        }
                    }

                    //Now if there is no gap  present
                    if(!gapExists)
                    {
                        // values before
                        for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) - 1; i >= 0; i--)
                        {
                            if (oList.Contains($"{row}{i}"))
                            {
                                valuesLeft++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // values after
                        for (int i = Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count - 1][1].ToString()) + 1; i < 7; i++)
                        {
                            if (oList.Contains($"{row}{i}"))
                            {
                                valuesRight++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if(valuesLeft >0 || valuesRight >0)
                        {
                            length += valuesLeft + valuesRight;
                        }

                        for(int i = col-valuesLeft, j=0; j<length  ; i++, j++  )
                        {
                            Word += boxes[row, i].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                        }

                        //We took the word now we convert to unicode
                        var b2u = new BijoyToUni();
                        var unicodeWord = b2u.Convert(Word);
                        Debug.Log(unicodeWord);

                        //Check if the unicodeword exists in dictionary
                        Dictionary<string, List<string>> elist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));

                        if (elist[length.ToString()].Contains(unicodeWord))
                        {
                            wordfound = true;
                            Debug.Log("We are again putting values in oldInputs");
                            InstantiatePlayBoard.oldInputs.AddRange(DragHandler.newInput);
                            InstantiatePlayBoard.oldInputs = InstantiatePlayBoard.oldInputs.Distinct().ToList();
                            DragHandler.newInput.Clear();
                        }
                        else
                        {
                            wordfound = false;
                            DragHandler.newInput.Clear();
                            Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                        }
                        Debug.Log(wordfound);
                        return wordfound;
                    }
                    // If there are gaps we can't do anything
                    else
                    {
                        length = 0;
                        DragHandler.newInput.Clear();
                        Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                        return false;
                    }

                }
                else if(colMatch)
                {
                    //check for gaps among the new inputs
                    var gapExists = false;
                    for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()); i <= Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count - 1][0].ToString()); i++)
                    {
                        if (!oList.Contains($"{i}{col}"))
                        {
                            //Reset game state and break
                            //spaceExists = true;
                            if (!DragHandler.newInput.Contains($"{i}{col}"))
                            {
                                Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                                gapExists = true;
                                break;
                            }
                            else
                            {
                                length++;
                            }
                        }
                        else
                        {
                            length++;
                        }
                    }

                    //Now if there is no gap  present
                    if (!gapExists)
                    {
                        // values above
                        for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()) - 1; i >= 0; i--)
                        {
                            if (oList.Contains($"{i}{col}"))
                            {
                                valuesAbove++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // values after
                        for (int i = Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count - 1][0].ToString()) + 1; i < 7; i++)
                        {
                            if (oList.Contains($"{i}{col}"))
                            {
                                valuesBelow++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (valuesAbove > 0 || valuesBelow > 0)
                        {
                            length += valuesAbove + valuesBelow;
                        }

                        for (int i = row - valuesAbove, j = 0; j < length; i++, j++)
                        {
                            Word += boxes[i, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                        }

                        //We took the word now we convert to unicode
                        var b2u = new BijoyToUni();
                        var unicodeWord = b2u.Convert(Word);
                        Debug.Log(unicodeWord);

                        //Check if the unicodeword exists in dictionary
                        Dictionary<string, List<string>> elist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));

                        if (elist[length.ToString()].Contains(unicodeWord))
                        {
                            wordfound = true;
                            InstantiatePlayBoard.oldInputs.AddRange(DragHandler.newInput);
                            InstantiatePlayBoard.oldInputs = InstantiatePlayBoard.oldInputs.Distinct().ToList();
                            DragHandler.newInput.Clear();
                        }
                        else
                        {
                            wordfound = false;
                            Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                            DragHandler.newInput.Clear();
                        }
                        Debug.Log(wordfound);
                        return wordfound;
                    }
                    // If there are gaps we can't do anything
                    else
                    {
                        length = 0;
                        Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                        DragHandler.newInput.Clear();
                        return false;
                    }
                }

            }
            //}
            //catch(Exception e)
            //{
            //    Debug.Log(e.Message);
            //}
            DragHandler.newInput.Clear();
            return false;

        }

        bool CheckEmpty()
        {
            Debug.Log("CheckEmpty() was called");
            string Word = String.Empty;
            var rowMatch = true;
            var colMatch = true;
            var spaceExists = false;
            var wordfound = false;

            //Sort the newinput list
            DragHandler.newInput.Sort();

            //Check if row or column matches if input is more than one
            if (DragHandler.newInput.Count == 1)
            {
                rowMatch = true;
                colMatch = false;
            }
            else if (DragHandler.newInput.Count < 1)
            {
                //Do nothing
            }
            else
            {
                for (int i = 1; i < DragHandler.newInput.Count; i++)
                {
                    if (rowMatch)
                    {
                        if (DragHandler.newInput[i][0] != DragHandler.newInput[0][0])
                        {
                            rowMatch = false;
                        }
                    }
                    if (colMatch)
                    {
                        if (DragHandler.newInput[i][1] != DragHandler.newInput[0][1])
                        {
                            colMatch = false;
                        }
                    }
                }
            }

            if (rowMatch)
            {
                Debug.Log("Row matches");
                //we check if the items have spaces
                for (int i = DragHandler.newInput[0][1], j = 0; j < DragHandler.newInput.Count; i++, j++)
                {
                    if (i != DragHandler.newInput[j][1])
                    {
                        spaceExists = true;
                        Debug.Log("Error: Space exists");
                        break;
                    }
                }

                // If there are no spaces then we can collect the word
                if (!spaceExists)
                {
                    var row = Int32.Parse(DragHandler.newInput[0][0].ToString());
                    for (int col = Int32.Parse(DragHandler.newInput[0][1].ToString()); col <= Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count - 1][1].ToString()); col++)
                    {
                        Word += boxes[row, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }

            }
            else if (colMatch)
            {
                Debug.Log("Column matches");
                for (int i = DragHandler.newInput[0][0], j = 0; j < DragHandler.newInput.Count; i++, j++)
                {
                    if (i != DragHandler.newInput[j][0])
                    {
                        spaceExists = true;
                        Debug.Log("Error: Space exists");
                        break;
                    }
                }

                // If there are no spaces then we can collect the word
                if (!spaceExists)
                {
                    var col = Int32.Parse(DragHandler.newInput[0][1].ToString());
                    for (int row = Int32.Parse(DragHandler.newInput[0][0].ToString()); row <= Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count - 1][0].ToString()); row++)
                    {
                        Word += boxes[row, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }

            }
            else
            {
                //Restore the game statte
                Debug.Log("Game state reset");
                Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                //RestoreState();
            }

            Debug.Log(Word);

            //Now we convert it to Unicode
            var b2u = new BijoyToUni();
            var unicodeWord = b2u.Convert(Word);

            Debug.Log(unicodeWord);

            Dictionary<string, List<string>> wordDict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));


            // if we get a match with the dictionary we return word found
            if (wordDict[DragHandler.newInput.Count.ToString()].Contains(unicodeWord))
            {
                Debug.Log("Word exists in dictionary");
                // InstantiatePlayBoard.oldInputs.AddRange(DragHandler.newInput);
                // InstantiatePlayBoard.oldInputs = InstantiatePlayBoard.oldInputs.Distinct().ToList();
                InstantiatePlayBoard.oldInputs = DragHandler.newInput.Distinct().ToList();
                DragHandler.newInput.Clear();
                wordfound = true;
                return wordfound;
            }
            else
            {
                //Restore game state
                Debug.Log("Word not in dictionary");
                DragHandler.newInput.Clear();
                Instantiator.GetComponent<InstantiatePlayBoard>().OnClickRestoreState();
                wordfound = false;
                return wordfound;
            }
            
        }


        #endregion

        #region Public Functions

        public void ResetPanel()
        {
            ResetValues();
        }

        public void Check()
        {
            Debug.Log("Check() was called");
            bool wordfound = false;
            if(stateEmpty)
            {
                wordfound = CheckEmpty();
            }
            else
            {
                wordfound = CheckExisting();
            }

            string[] playBoardStrings = new string[49];
            
            if(wordfound)
            {
                stateEmpty = false;
                

                int k = 0;
                for (int i=0; i<7;i++)
                {
                    for(int j=0; j<7;j++)
                    {
                        if (boxes[i, j].transform.childCount > 0)
                        {
                            playBoardStrings[k] = boxes[i, j].GetComponentInChildren<TextMeshProUGUI>().text;
                        }
                        else
                        {
                            playBoardStrings[k] = "\0";
                        }
                        //Debug.Log($"Playboardstring: {playBoardStrings[k]}");
                        k++;
                    }
                }

                //Using RaiseEvent we synchronize all the players
                PhotonNetwork.RaiseEvent(EVENT_SYNC_PLAYBOARD, new object[] { playBoardStrings }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

                PhotonNetwork.RaiseEvent(EVENT_UPDATE_SCORE, new object[] { InstantiatePlayBoard.oldInputs.ToArray() }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                InstantiatePlayBoard.player1.score += 5;
                scoreboard1.GetComponent<Text>().text = InstantiatePlayBoard.player1.score.ToString();


            }
            
        }

        public void RestorePanel(string[,] boxes)
        {
            ChangeValues(boxes);
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
            if (obj.Code == EVENT_SYNC_PLAYBOARD)
            {
                object[] data = (object[])obj.CustomData;
                var playBoardStrings = (string[])data[0] ;
                ChangeTo2DArray(playBoardStrings);
                foreach(var item in InstantiatePlayBoard.oldInputs)
                {
                    Debug.Log(item);
                }
            }
            if (obj.Code == EVENT_UPDATE_SCORE)
            {
                object[] data = (object[])obj.CustomData;
                var oldinputsarr = (string[])data[0];
                InstantiatePlayBoard.oldInputs = oldinputsarr.Distinct().ToList();
                stateEmpty = false;
            }
        }

        #endregion
    }
}
