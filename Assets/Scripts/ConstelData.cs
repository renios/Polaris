using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstelData
{
    public string Name;
    public string InternalName;
    public int Group;
    public List<int> AvailableCharacter;

    public ConstelData()
    {
        AvailableCharacter = new List<int>();
    }

    public ConstelData(string key, string name, int index)
    {
        Name = name;
        InternalName = key;
        Group = index;
        AvailableCharacter = new List<int>();
    }
}