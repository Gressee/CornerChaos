using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenu;

    public void MainMenuActive(bool active)
    {
        mainMenu.SetActive(active);
    }
}
