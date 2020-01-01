using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Observe
{
    [Serializable]
    public enum ObserveBehaviour { Idle, Observing, Finished, Tutorlal = 10 }

    [Serializable]
    public class ObserveStatus
    {
        public bool isTutorial;
        public bool userChecked;

        public ObserveBehaviour behaviour;
        public DateTime endTime;
        public int pickTryCount;
        public int favIncrement;
        public Dictionary<int, float> charProb;
        public Dictionary<int, int> pickResult;
        public Dictionary<int, int> charFavData;

        public float[] scopePos;

        public ObserveStatus()
        {
            behaviour = ObserveBehaviour.Idle;
            charProb = new Dictionary<int, float>();
            pickResult = new Dictionary<int, int>();
            charFavData = new Dictionary<int, int>();

            userChecked = true;
        }

        public static ObserveStatus Load()
        {
            if (File.Exists(Application.persistentDataPath + "/obs_status"))
            {
                var stream = new FileStream(Application.persistentDataPath + "/obs_status", FileMode.Open);
                var formatter = new BinaryFormatter();
                var res = (ObserveStatus)formatter.Deserialize(stream);
                stream.Close();
                return res;
            }
            else
                return new ObserveStatus();
        }

        public void Save()
        {
            var stream = new FileStream(Application.persistentDataPath + "/obs_status", FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Close();
        }
    }
}