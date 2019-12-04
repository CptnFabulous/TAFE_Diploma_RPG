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
        
        string loadInventoryURL = "http://localhost/nsirpg/loadinventory.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        UnityWebRequest webRequest = UnityWebRequest.Post(loadInventoryURL, form);
        yield return webRequest.SendWebRequest();

        Debug.Log(webRequest.downloadHandler.text);

        GameObject playerObject = playerHandler.gameObject;
        PlayerHandler ph = playerObject.GetComponent<PlayerHandler>();

        ph.Awake(); // Declares player's inventory before spawning

        List<ItemStack> newInventory = new List<ItemStack>();
        if (webRequest.downloadHandler.text != "Error, inventory could not be found")
        {
            newInventory = ItemData.LoadInventory(webRequest.downloadHandler.text);
        }
        ph.pi.items = newInventory;

        GameObject p = Object.Instantiate(playerObject, spawnPoint.position, Quaternion.identity);
        p.name = username;
    }

    public static IEnumerator SaveAndQuit(PlayerHandler ph, string mainMenuSceneName)
    {
        Debug.Log("Save and quit sequence started");
        string saveInventoryURL = "http://localhost/nsirpg/saveinventory.php";
        WWWForm form = new WWWForm();
        form.AddField("username", ph.name);
        form.AddField("inventorystring", ItemData.SaveInventory(ph.pi.items));
        UnityWebRequest webRequest = UnityWebRequest.Post(saveInventoryURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);

        // After everything has saved, destroy player object
        Object.Destroy(ph.gameObject);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
