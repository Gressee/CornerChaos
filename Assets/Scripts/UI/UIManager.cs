using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    GameObject mainMenu;

    [SerializeField]
    GameObject mapSelect;

    public void MainMenuActiveState(bool activeState)
    {
        mainMenu.SetActive(activeState);
    }

    public void MapSelectActiveState(bool activeState)
    {
        mapSelect.SetActive(activeState);
    }

}
