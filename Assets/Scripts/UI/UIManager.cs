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

    [SerializeField]
    GameObject gameOverlay;

    [SerializeField]
    GameObject gameEnd;


    //// FUNCTIONS TO (DEACTIVATE) THE TOP LEVEL MENUS ////
    
    public void MainMenuActiveState(bool activeState)
    {
        mainMenu.SetActive(activeState);
    }

    public void MapSelectActiveState(bool activeState)
    {
        mapSelect.SetActive(activeState);
    }

    public void GameOverlayActiveState(bool activeState)
    {
        gameOverlay.SetActive(activeState);
    }

    public void GameEndActiveState(bool activeState)
    {
        gameEnd.SetActive(activeState);
    }

}
