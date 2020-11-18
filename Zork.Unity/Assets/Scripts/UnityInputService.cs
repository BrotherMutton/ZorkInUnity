using System;
using UnityEngine;
using Zork;

public class UnityInputService : MonoBehaviour, IInputService
{
    public event EventHandler<string> InputRecieved;
}
