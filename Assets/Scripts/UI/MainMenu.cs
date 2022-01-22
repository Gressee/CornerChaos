using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    GameObject inputJoinCode;

    string joinCode;

    void Start()
    {
        var input = inputJoinCode.GetComponent<TMP_InputField>();
        Debug.Log(input);
        input.onValueChanged.AddListener(SetJoinCode);
    }


    public void SetJoinCode(string joinCode)
    {
        this.joinCode = joinCode;
    }
   
    public void ButtonHost()
    {
        GameManager.Singelton.StartHost();
        UIManager.Singelton.MainMenuActiveState(false);
        UIManager.Singelton.MapSelectActiveState(true);
    }


    public void ButtonJoin()
    {       
        GameManager.Singelton.StartJoin(joinCode);
        UIManager.Singelton.MainMenuActiveState(false);
        UIManager.Singelton.MapSelectActiveState(true);
    }

    


    public void ButtonSingleplayer()
    {
        GameManager.Singelton.StartSingleplayer();
        UIManager.Singelton.MainMenuActiveState(false);
        UIManager.Singelton.MapSelectActiveState(true);
    }

}
