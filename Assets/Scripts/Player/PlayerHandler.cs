using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour // A class used to easily identify a player
{
    [HideInInspector] public PlayerInventory pi;

    public void Awake()
    {
        pi = GetComponent<PlayerInventory>(); // Obtains inventory data
    }
}
