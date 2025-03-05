using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundDynamic : MonoBehaviour
{
    public CanvasGroup secondaryScreen;

    // Start is called before the first frame update
    void Start()
    {
        HideScreen();
    }

    public void ShowScreen()
    {
        secondaryScreen.alpha = 1;
        secondaryScreen.interactable =  true;
        secondaryScreen.blocksRaycasts = true;
    }

    public void HideScreen()
    {
        secondaryScreen.alpha = 0;
        secondaryScreen.interactable =  false;
        secondaryScreen.blocksRaycasts = false;
    }
}
