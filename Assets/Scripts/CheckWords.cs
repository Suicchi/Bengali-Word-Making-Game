using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ban_u2a;
using Assets.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Com.Hattimatim.BWMG;

/**
 * TODO:
 * Replace checkList with the dictfile.
 * 
 */

public class CheckWords : MonoBehaviour
{
    private static GameObject[,] boxes = new GameObject[7,7];
    // public GameObject successmessage;

    public static GameObject[] inputBoxes = new GameObject[6];

    public static List<string> oldInputs = new List<string>();

    public static int score1 = 0;
    public static int score2 = 0;

    //Create Match state
    GameState matchState;

    //Create two players
    Player player1, player2;


    // string checkWord = "আমাদেরকে";
    static bool stateEmpty;

    //Prefab used when loading
    public GameObject myPrefab;
    public GameObject myPlayBoardEmptyPrefab;
    public GameObject myPlayBoardTextPrefab;

    //Dictionary
    string dictfile = "sorted159kwords.json";
    GameObject inputPanel;

    private void Start()
    {
        //Initialize the input boxes and play boxes
        GameObject playBoard = GameObject.Find("PlayBoardPanel");

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



        //Save this state
        player1 = new Player(1,0, true);
        player2 = new Player(2, 0, false);
        
    }


    private void Update()
    {

        if (inputPanel == null)
        {
            if (GameObject.Find("InputPanel"))
            {
                inputPanel = GameObject.Find("InputPanel");
                //We are getting the slots into the object variable for future use 
                if (inputPanel.transform.childCount > 0)
                {
                    for (int i = 0; i < inputPanel.transform.childCount; i++)
                        inputBoxes[i] = inputPanel.transform.GetChild(i).gameObject;
                }
                matchState = new GameState(123, boxes, inputBoxes, player1, player2, oldInputs);
                GenerateInput();
                SaveSystem.Save(matchState);
            }
        }
    }

    List<string> checkList = new List<string>(new string[] { "আমার", "আম", "আমাদেরকে", "আমাদের", "মার", "মা" });


    public void CheckDone()
    {
        string Word = String.Empty;
        var rowMatch = true;
        var colMatch = true;
        var spaceExists = false;
        
        if(stateEmpty)
        {
            //Sort the newinput list
            DragHandler.newInput.Sort();

            //Check if row or column matches if input is more than one
            if(DragHandler.newInput.Count == 1)
            {
                rowMatch = true;
                colMatch = false;
            }
            else if(DragHandler.newInput.Count <1)
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

            if(rowMatch)
            {
                Debug.Log("Row matches");
                //we check if the items have spaces
                for (int i=DragHandler.newInput[0][1], j=0; j<DragHandler.newInput.Count; i++,j++)
                {
                    if(i!=DragHandler.newInput[j][1])
                    {
                        spaceExists = true;
                        Debug.Log("Error: Space exists");
                        break;
                    }
                }

                // If there are no spaces then we can collect the word
                if(!spaceExists)
                {
                    var row = Int32.Parse(DragHandler.newInput[0][0].ToString());
                    for (int col = Int32.Parse(DragHandler.newInput[0][1].ToString()); col<=Int32.Parse(DragHandler.newInput[DragHandler.newInput.Count-1][1].ToString()); col++)
                    {
                        Word += boxes[row,col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }

            }
            else if(colMatch)
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
                RestoreState();
            }
        }
        else
        {
            //Since there is already old values there we need to do something with them on this :D

            //Sort the newinput list
            DragHandler.newInput.Sort();
            //Sort the oldinput list
            oldInputs.Sort();
            //Create a temporary list for checking
            List<string> temporaryList = new List<string>();

            //These variables are declared in case there is an isolated case (single new input)
            int valuesAbove = 0;
            int valuesBelow = 0;
            int valuesLeft = 0;
            int valuesRight = 0;

            //Check if row or column matches if newInput is more than one
            if (DragHandler.newInput.Count == 1)
            {

                //check for values
                //row wise
                var row = Int32.Parse(DragHandler.newInput[0][0].ToString());
                var col = Int32.Parse(DragHandler.newInput[0][1].ToString());
                //before 
                for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) - 1; i > -1; i--)
                {
                    if (oldInputs.Contains($"{row}{i}"))
                    {
                        valuesLeft++;
                    }
                    else
                    {
                        break;
                    }
                }

                // after
                for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) + 1; i < 7; i++)
                {
                    if (oldInputs.Contains($"{row}{i}"))
                    {
                        valuesRight++;
                    }
                    else
                    {
                        break;
                    }
                }

                //Above
                for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()) - 1; i > -1; i--)
                {
                    if (oldInputs.Contains($"{i}{col}"))
                    {
                        valuesAbove++;
                    }
                    else
                    {
                        break;
                    }
                }

                //Below
                for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()) + 1; i < 7; i++)
                {
                    if (oldInputs.Contains($"{i}{col}"))
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
                    RestoreState();
                }
                else if(valuesAbove > 0 || valuesBelow > 0)
                {
                    colMatch = true;
                    rowMatch = false;
                }
                else if (valuesLeft > 0 || valuesRight > 0)
                {
                    rowMatch = true;
                    colMatch = false;
                }
                //else
                //{
                //    //This is a special case this will be coded in the future
                //}

            }
            else if(DragHandler.newInput.Count <1)
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
                var row = Int32.Parse(DragHandler.newInput[0][0].ToString());
                //we check if the items have spaces
                for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()), j = 0; j < DragHandler.newInput.Count; i++, j++)
                {
                    if (i != DragHandler.newInput[j][1])
                    {
                        spaceExists = true;
                        temporaryList.Add(i.ToString());
                    }
                }

                if(spaceExists)
                {
                    for(int i=0; i<temporaryList.Count;i++)
                    {
                        if (!oldInputs.Contains($"{row}{temporaryList[0]}"))
                        {
                            Debug.Log("Error, contains space. Reset game state");
                            break;
                        }
                    }
                }
                else
                {

                }

                // Check if there is value before and/or after the new input
                int valuesBefore = 0;
                int valuesAfter = 0;

                if (valuesLeft > 0 || valuesRight >0)
                {
                    valuesBefore = valuesLeft;
                    valuesAfter = valuesRight;
                }
                else
                {
                    

                    //before 
                    for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) - 1; i > -1; i--)
                    {
                        if (oldInputs.Contains($"{row}{i}"))
                        {
                            valuesBefore++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // after
                    for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) + 1; i < 7; i++)
                    {
                        if (oldInputs.Contains($"{row}{i}"))
                        {
                            valuesAfter++;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                // If there are no spaces then we can collect the word
                if (!spaceExists && valuesBefore == 0 && valuesAfter == 0)
                {
                    Debug.Log("Invalid state. Restore earlier state");
                    RestoreState();
                }
                else
                {
                    for(int col = Int32.Parse(DragHandler.newInput[0][1].ToString())-valuesBefore; col< DragHandler.newInput.Count+valuesBefore+valuesAfter ; col++)
                    {
                        Word += boxes[row, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }


            }
            else if (colMatch)
            {
                Debug.Log("Column matches");
                var col = Int32.Parse(DragHandler.newInput[0][1].ToString());
                //we check if the items have spaces
                for (int i = Int32.Parse(DragHandler.newInput[0][0].ToString()), j = 0; j < DragHandler.newInput.Count; i++, j++)
                {
                    if (i != DragHandler.newInput[j][0])
                    {
                        spaceExists = true;
                        temporaryList.Add(i.ToString());
                    }
                }

                if (spaceExists)
                {
                    for (int i = 0; i < temporaryList.Count; i++)
                    {
                        if (!oldInputs.Contains($"{temporaryList[0]}{col}"))
                        {
                            Debug.Log("Error, contains space. Reset game state");
                            break;
                        }
                    }
                }
                else
                {

                }

                // Check if there is value before and/or after the new input
                int valuesBefore = 0;
                int valuesAfter = 0;

                if (valuesAbove > 0 || valuesBelow > 0)
                {
                    valuesBefore = valuesAbove;
                    valuesAfter = valuesBelow;
                }
                else
                {
                    //before 
                    for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) - 1; i > -1; i--)
                    {
                        if (oldInputs.Contains($"{i}{col}"))
                        {
                            valuesBefore++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // after
                    for (int i = Int32.Parse(DragHandler.newInput[0][1].ToString()) + 1; i < 7; i++)
                    {
                        if (oldInputs.Contains($"{i}{col}"))
                        {
                            valuesAfter++;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                // If there are no spaces then we can collect the word
                if (!spaceExists && valuesBefore == 0 && valuesAfter == 0)
                {
                    Debug.Log("Invalid state. Restore earlier state");
                    RestoreState();
                }
                else
                {
                    for (int row = Int32.Parse(DragHandler.newInput[0][0].ToString()) - valuesBefore; row < DragHandler.newInput.Count + valuesBefore + valuesAfter; row++)
                    {
                        Word += boxes[row, col].transform.GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                }
            }
            else
            {
                //Restore the game statte
                Debug.Log("Game state restore");
                RestoreState();
            }

        }

        Debug.Log(Word);

        //Now we convert it to Unicode
        var b2u = new BijoyToUni();
        var unicodeWord = b2u.Convert(Word);

        //Now we match it with our word base
        if(checkList.Contains(unicodeWord))
        {
            Debug.Log("Success :D");
            Debug.Log(unicodeWord);

            //Make them non draggable
            foreach (GameObject item in boxes)
            {
                if(item.transform.childCount>0)
                {
                    Destroy(item.GetComponentInChildren<DragHandler>());
                }
            }

            //Take the new inputs into old inputs
            oldInputs.AddRange(DragHandler.newInput);
            //Clears the newinput list
            DragHandler.newInput.Clear();


            //Finally, we adjust the score of the player, set the boxes to be non interactable, set the stage and save the current state
            stateEmpty = false;
            matchState.stateUpdate(123, boxes, inputBoxes, player1, player2, oldInputs);
            SaveSystem.Save(matchState);

        }
        else
        {
            RestoreState();
        }

    }


    public void RestoreState()
    {
        matchState = SaveSystem.Load();
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (matchState.Boxes[i, j] != "\0")
                {
                    GameObject temp = Instantiate(myPlayBoardTextPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    Destroy(boxes[i,j]);
                    temp.GetComponentInChildren<TextMeshProUGUI>().text = matchState.Boxes[i,j];
                    temp.transform.SetParent(GameObject.Find("PlayBoardPanel").transform, false);
                    temp.name = $"{i}{j}";
                    boxes[i, j] = temp;
                }
                else
                {
                    GameObject temp = Instantiate(myPlayBoardEmptyPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    Destroy(boxes[i, j]);
                    Destroy(temp.GetComponentInChildren<TextMeshProUGUI>());
                    temp.transform.SetParent(GameObject.Find("PlayBoardPanel").transform, false);
                    temp.name = $"{i}{j}";
                    boxes[i, j] = temp;
                }

            }
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject temp = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            Destroy(inputBoxes[i]);
            if (matchState.InputBoxes[i] != "\0")
            {
                temp.GetComponentInChildren<TextMeshProUGUI>().text = matchState.InputBoxes[i];

            }
            else
            {
                Destroy(temp.GetComponentInChildren<TextMeshProUGUI>());
            }

            temp.transform.SetParent(GameObject.Find("InputPanel").transform, false);
            inputBoxes[i] = temp;
        }

        player1.score = matchState.player1score;
        player2.score = matchState.player2score;
        oldInputs = matchState.OldInputs;

        //Reset the newinput too
        DragHandler.newInput.Clear();
    }


    public void GenerateInput()
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
            var random = new System.Random();
            var total = 0;
            string words = "";
            List<string> dividedWords = new List<string>();
            Dictionary<string, List<string>> elist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(dictfile));
            List<int> wordPartsNumber = new List<int>();
            var u2b = new UniToBijoy();

            while (total <6)
            {
                var currentNumber = random.Next(1, 6-total );
                wordPartsNumber.Add(currentNumber);
                total += currentNumber;
            }

            foreach(var item in wordPartsNumber)
            {
                int rand = random.Next(elist[item.ToString()].Count);
                words += elist[item.ToString()][rand];
            }
            // Divide the words into parts if parts is greater than one
            dividedWords = BanglaHandler.DividedWords(words);

            foreach(var part in dividedWords)
            {
                Debug.Log(part);
            }

            //Convert them to Bijoy
            for(int i=0; i< dividedWords.Count; i++)
            {
                dividedWords[i] = u2b.Convert(dividedWords[i]);
            }

            foreach (var part in dividedWords)
            {
                Debug.Log(part);
            }

        InputBoxesRegen(dividedWords);
        matchState.stateUpdate(inputBoxes);
        SaveSystem.Save(matchState);

        //}
        // else
        //{

        //}

    }

    void InputBoxesRegen(string[] InputBoxes)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject temp = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            Destroy(inputBoxes[i]);
            if (matchState.InputBoxes[i] != "\0")
            {
                temp.GetComponentInChildren<TextMeshProUGUI>().text = InputBoxes[i];

            }
            else
            {
                Destroy(temp.GetComponentInChildren<TextMeshProUGUI>());
            }

            temp.transform.SetParent(GameObject.Find("InputPanel").transform, false);
            inputBoxes[i] = temp;
        }
    }

    void InputBoxesRegen(List<string> InputBoxes)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject temp = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            Destroy(inputBoxes[i]);
            
            if (matchState.InputBoxes[i] != "\0")
            {
                temp.GetComponentInChildren<TextMeshProUGUI>().text = InputBoxes[i];

            }
            else
            {
                Destroy(temp.GetComponentInChildren<TextMeshProUGUI>());
            }
            
            temp.transform.SetParent(GameObject.Find("InputPanel").transform, false);
            inputBoxes[i] = temp;
        }
    }

}
