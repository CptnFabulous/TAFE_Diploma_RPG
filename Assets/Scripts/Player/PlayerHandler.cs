using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour
{
    public string properName;
    
    [HideInInspector] public PlayerInventory pi;

    public void Awake()
    {
        pi = GetComponent<PlayerInventory>();
    }

    public void QuitGame()
    {

    }
}
