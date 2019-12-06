using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public static class SaveAndLoad
{
    public static IEnumerator LoadCharacter(string username, PlayerHandler playerHandler, Transform spawnPoint)
    {
        Debug.Log("Load sequence started");
        
        string loadInventoryURL = "http://localhost/nsirpg/loadinventory.php"; // Accesses appropriate PHP file to retrieve inventory data
        WWWForm form = new WWWForm(); // Creates new web form
        form.AddField("username", username); // Adds username to form
        UnityWebRequest webRequest = UnityWebRequest.Post(loadInventoryURL, form); // Submits form
        yield return webRequest.SendWebRequest(); // Waits while form is processed

        Debug.Log(webRequest.downloadHandler.text);

        GameObject playerObject = playerHandler.gameObject; // Creates new instance of player object prefab
        PlayerHandler ph = playerObject.GetComponent<PlayerHandler>(); // References player's individual PlayerHandler prefab

        ph.Awake(); // Declares player's inventory before spawning

        
        List<ItemStack> newInventory = new List<ItemStack>(); // Declares new, empty inventory
        if (webRequest.downloadHandler.text != "Error, inventory could not be found") // If inventory was found, extrapolate a new inventory from the provided data string
        {
            newInventory = ItemData.LoadInventory(webRequest.downloadHandler.text);
        }
        ph.pi.items = newInventory; // Add items to player instance's inventory

        GameObject p = Object.Instantiate(playerObject, spawnPoint.position, Quaternion.identity); // Instantiate player instance into game world
        p.name = username; // Change player object's name to player username for easy reference
    }

    public static IEnumerator SaveAndQuit(PlayerHandler ph, string mainMenuSceneName)
    {
        Debug.Log("Save and quit sequence started");
        string saveInventoryURL = "http://localhost/nsirpg/saveinventory.php"; // Accesses appropriate PHP file to save inventory data
        WWWForm form = new WWWForm(); // Creates new web form
        form.AddField("username", ph.name); // Adds username to form
        form.AddField("inventorystring", ItemData.SaveInventory(ph.pi.items)); // Generates data string based on items in inventory
        UnityWebRequest webRequest = UnityWebRequest.Post(saveInventoryURL, form); // Submits form
        yield return webRequest.SendWebRequest(); // Waits while form is processed
        Debug.Log(webRequest.downloadHandler.text);

        // After everything has saved, destroy player object
        Object.Destroy(ph.gameObject);

        SceneManager.LoadScene(mainMenuSceneName); // Load main menu
    }
}
