using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{   
    public List<MazeGrid> listMazeGrid;
    public GameObject endArea;
    public int currentLevel = -1;
    public MazeGrid currentMaze => listMazeGrid[currentLevel];
    public void EnableNextArea()
    {
        if (currentLevel < listMazeGrid.Count - 1)
        {
            listMazeGrid[currentLevel + 1].gameObject.SetActive(true);
        }
        else if (currentLevel == listMazeGrid.Count - 1)
        {
            endArea.gameObject.SetActive(true);
        }
    }
    public void OnTriggerMaze(int level)
    {
        currentLevel = level;
        if (currentMaze.isCreated == false)
        {
            currentMaze.CreateMaze();
            GameManager.Instance.uiManager.SetNotice($"Maze level {currentLevel} is generating...", 0, 3);
        }
        if (currentLevel == 0)
        {
            GameManager.Instance.uiManager.SetNotice($"You can press Space for auto solve the Maze", 10, 5);
            GameManager.Instance.uiManager.SetNotice($"The lesser step you walk, the higher score you get", 20, 5);
        }
        else
        {
            StartCoroutine(Utils.IEDelayCall(() => listMazeGrid[currentLevel - 1].endCell.BuildWall("TOP"), 2));
        }
    }
}
