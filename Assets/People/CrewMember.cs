using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
class CrewMember
{
    [SerializeField]
    private readonly string _name;

    public string name { get { return _name; } }

    public CrewMember(string name)
    {
        this._name = name;
    }
}
