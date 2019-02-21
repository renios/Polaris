using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstelData
{
    public string Name;
    public string InternalName;
    public List<int> AvailableCharacter;

    public ConstelData()
    {
        AvailableCharacter = new List<int>();
    }

    public ConstelData(string key, string name)
    {
        Name = name;
        InternalName = key;
        AvailableCharacter = new List<int>();
    }
}