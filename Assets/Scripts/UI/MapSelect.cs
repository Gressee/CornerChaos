using Core.Singleton;
using UnityEngine;
using TMPro;

public class MapSelect : MonoBehaviour
{

    [SerializeField]
    GameObject joinCodeDisplay;

    void Update()
    {
        joinCodeDisplay.GetComponent<TextMeshProUGUI>().text = GameManager.Singelton.GetJoinCode();
    }

    public void ButtonStartGame()
    {
        
        GameManager.Singelton.SetPauseStatus(false);
        UIManager.Singelton.MapSelectActiveState(false);
        // Make Game overlay active

    }
}
