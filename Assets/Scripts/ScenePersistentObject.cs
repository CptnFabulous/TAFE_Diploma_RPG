using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersistentObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Prevents object from being destroyed on load
    }
}
