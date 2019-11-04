using Com.Hattimatim.BWMG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts
{
    public static class SaveSystem
    {
        static string binPath = Application.persistentDataPath + "/gamesave.bin";
        public static void Save(GameState game)
        {
            FileStream stream = new FileStream(binPath, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, game);
            stream.Close();
            Debug.Log("Data Saved!");
        }

        public static GameState Load()
        {
            if (File.Exists(binPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(binPath, FileMode.Open);

                GameState data = formatter.Deserialize(stream) as GameState;
                stream.Close();

                Debug.Log("Data Loaded!");
                return data;
            }
            else
            {
                Debug.Log("Saved data not found");
                return null;
            }
        }
    }
}
