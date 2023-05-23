using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorStack : MonoBehaviour
{
    [SerializeField] List<MazeCell> cells = new List<MazeCell>();
    public int StackLength => cells.Count;
    public void Push(MazeCell cell) => cells.Add(cell);
    public MazeCell Pull()
    {
        var returnCell = cells[^1];
        cells.Remove(returnCell);
        return returnCell;
    }
}
