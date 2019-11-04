using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Com.Hattimatim.BWMG
{
    [System.Serializable]
    public class GameState
    {
        private int matchID;
        public string[,] Boxes = new string[7,7];
        public string[] InputBoxes = new string[6];
        public float[] asdf;
        public int player1ID;
        public int player2ID;
        public int player1score;
        public int player2score;
        public List<string> OldInputs;

        public GameState(int id, GameObject[,] boxes, GameObject[] inputBoxes, Player player1, Player player2, List<string> oldInputs )
        {
            matchID = id;

            //Boxes = new string[7, 7];
            //InputBoxes = new string[6];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (boxes[i, j].transform.childCount > 0)
                    {
                        Boxes[i, j] = boxes[i, j].GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                    else
                    {
                        Boxes[i, j] = "\0";
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (inputBoxes[i].transform.childCount > 0)
                {
                    InputBoxes[i] = inputBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text;
                }
                else
                {
                    InputBoxes[i] = "\0";
                }
            }

            player1ID = player1.id;
            player1score = player1.score;
            player2ID = player2.id;
            player2score = player2.score;
            OldInputs = oldInputs;
        }
        public GameState(Player player1, Player player2)
        {

        }

        public void stateUpdate(int id, GameObject[,] boxes, GameObject[] inputBoxes, Player player1, Player player2, List<string> oldInputs)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (boxes[i, j].transform.childCount > 0)
                    {
                        Boxes[i, j] = boxes[i, j].GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                    else
                    {
                        Boxes[i, j] = "\0";
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (inputBoxes[i].transform.childCount > 0)
                {
                    InputBoxes[i] = inputBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text;
                }
                else
                {
                    InputBoxes[i] = "\0";
                }
            }

            player1ID = player1.id;
            player1score = player1.score;
            player2ID = player2.id;
            player2score = player2.score;


            OldInputs = oldInputs;
        }
        public void stateUpdate(GameObject[,] boxes, GameObject[] inputBoxes, Player player1, Player player2, List<string> oldInputs)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (boxes[i, j].transform.childCount > 0)
                    {
                        Boxes[i, j] = boxes[i, j].GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                    else
                    {
                        Boxes[i, j] = "\0";
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (inputBoxes[i].transform.childCount > 0)
                {
                    InputBoxes[i] = inputBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text;
                }
                else
                {
                    InputBoxes[i] = "\0";
                }
            }

            player1ID = player1.id;
            player1score = player1.score;
            player2ID = player2.id;
            player2score = player2.score;


            OldInputs = oldInputs;
        }

        public void stateUpdate(GameObject[] inputBoxes)
        {
            for (int i = 0; i < 6; i++)
            {
                if (inputBoxes[i].transform.childCount > 0)
                {
                    InputBoxes[i] = inputBoxes[i].GetComponentInChildren<TextMeshProUGUI>().text;
                }
                else
                {
                    InputBoxes[i] = "\0";
                }
            }
        }

        public void stateUpdate(GameObject[,] boxes)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (boxes[i, j].transform.childCount > 0)
                    {
                        Boxes[i, j] = boxes[i, j].GetComponentInChildren<TextMeshProUGUI>().text;
                    }
                    else
                    {
                        Boxes[i, j] = "\0";
                    }
                }
            }
        }

        public void stateUpdate(Player player1, Player player2)
        {
            player1score = player1.score;
            player2score = player2.score;
        }
    }
}
