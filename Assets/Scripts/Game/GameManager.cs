using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private Joystick _joystick;

    private void Start()
    {
        ConnectCharaAndJoystick();
    }

    private void ConnectCharaAndJoystick()
    {
        _characterManager.joystick = _joystick;
    }
}
