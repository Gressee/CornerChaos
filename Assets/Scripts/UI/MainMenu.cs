using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject gameManager;

    [SerializeField]
    GameObject inputGameID;

    // For now this is the ip address    
    string gameID;

    public void ButtonHost()
    {
        gameManager.GetComponent<GameManager>().StartHost();
        gameObject.GetComponent<UIManager>().MainMenuActive(false);
    }

    public void ButtonJoin()
    {
        string gameID = inputGameID.GetComponent<InputField>().text;
        gameManager.GetComponent<GameManager>().StartJoin(gameID);
        gameObject.GetComponent<UIManager>().MainMenuActive(false);
    }

    public void ButtonSingleplayer()
    {
        gameManager.GetComponent<GameManager>().StartSingleplayer();
        gameObject.GetComponent<UIManager>().MainMenuActive(false);
    }

}
