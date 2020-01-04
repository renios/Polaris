using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Observe
{
    public class ConstelObsData
    {
        public string name;
        public bool observable;
        public List<int> charas;
        public Dictionary<int, float> charWeights;

        public ConstelObsData()
        {
            charas = new List<int>();
            charWeights = new Dictionary<int, float>();
        }

        public ConstelObsData(string n)
        {
            name = n;
            charas = new List<int>();
            charWeights = new Dictionary<int, float>();

            CheckObservability();
        }

        public void AddCharacter(int charKey, float charWeight)
        {
            charas.Add(charKey);
            charWeights.Add(charKey, charWeight);
        }

        public void CheckObservability()
        {
            var groupKey = Variables.Constels[name].Group;
            if (Variables.ObserveSkyLevel >= 0 && groupKey == 0)
                observable = true;
            else if (Variables.ObserveSkyLevel >= 1 && groupKey == 4)
                observable = true;
            else
                observable = false;
        }
    }
}