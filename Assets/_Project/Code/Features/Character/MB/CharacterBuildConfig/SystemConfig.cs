using UnityEngine;
using System;

public abstract class SystemConfig : ScriptableObject
{
    public abstract Type SystemType { get; }
    
    [SerializeField, HideInInspector] 
    private int order = 0;
    public int Order => order;
    
    public void SetOrder(int newOrder) => order = newOrder;
}