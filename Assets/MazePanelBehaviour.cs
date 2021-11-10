using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class MazePanelBehaviour : MonoBehaviour
{
    public Text mazeTitle;
    public Image mazeView;
    public InputField repsField;

    int mazeIndex;

    public void SetPanelComponents(MazeInfo info, int index)
    {
        mazeTitle.text = info.name;
        if (info.topView != null)
            mazeView.sprite = info.topView;

        mazeIndex = index;
    }

    public void SetIndex(int index)
    {
        mazeIndex = index;
    }

    public void RemoveMaze()
    {
        UIManager.instance.RemoveMaze2Protocol(mazeIndex);
    }

    public void SetMazeReps(string value)
    {
        try
        {
            int reps= int.Parse(value);
            UIManager.instance.SetMazeRep(reps, mazeIndex);
        }
        catch
        {
            UIManager.instance.ShowPopUp("The input should be an integer!!!");
            repsField.text = "";
        }
    }

    public void SetMazeRepsDisplay(int i)
    {
        repsField.text = i.ToString();
    }
}
