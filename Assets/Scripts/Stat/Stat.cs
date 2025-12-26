using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float basevalue;
    public List<float> modifiers; // List to hold modifiers from items, buffs, Skills etc....
    
    public float GetValue()
    {
        float finalValue = basevalue;
        foreach (float  modifier in modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    public void SetDefaultValue(float value)
    {
        basevalue = value;
    }

    public void AddModifier(float modifier)
    {
        if(modifier != 0) modifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier)
    {
        if(modifier!=0) modifiers.Remove(modifier);
    }
}
