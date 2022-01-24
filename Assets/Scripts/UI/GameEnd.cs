using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public void ButtonMainMenu()
    {
        // Maybe spawn new standard Map
        UIManager.Singelton.GameEndActiveState(false);
        UIManager.Singelton.MainMenuActiveState(true);
    }

    public void ButtonMapSelect()
    {
        UIManager.Singelton.GameEndActiveState(false);
        UIManager.Singelton.MapSelectActiveState(true);
    }

    public void ButtonPlayAgain()
    {
        UIManager.Singelton.GameEndActiveState(false);
        UIManager.Singelton.GameOverlayActiveState(true);
    }
}

