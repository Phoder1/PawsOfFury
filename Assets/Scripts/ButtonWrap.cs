using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWrap : MonoBehaviour
{

    [SerializeField]
    Color ColorForButton;

    [SerializeField]
    List<ButtonWrap> AdjecentLevels;

    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    [ContextMenu("Mitz")]
    public void LevelWon()
    {
        ColorBlock tempBlock = button.colors;
        tempBlock.normalColor = ColorForButton;
        button.colors = tempBlock;

        for (int i = 0; i < AdjecentLevels.Count; i++)
        {
            AdjecentLevels[i].UnlockLevel();
        }
    }

    public void UnlockLevel()
    {
        button.interactable = true;
    }

  
}
