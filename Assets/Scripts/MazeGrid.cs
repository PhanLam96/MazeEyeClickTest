using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGrid : MonoBehaviour
{
    public bool isCreated;

    [Header("Maze Settings")]
    [SerializeField] private MazeCell cellPrefab;

    //General
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    [SerializeField] private MazeCell[,] mazeCellArray;
    public MazeCell currentCell;
    public MazeCell startCell;
    public MazeCell endCell;

    //Generator
    [SerializeField] private Transform gridOrigin;
    [SerializeField] private MazeGeneratorStack mazeStack;

    //Solver
    [SerializeField] private List<MazeCell> openSet;
    [SerializeField] private List<MazeCell> closedSet;

    #region Generator
    public void CreateMaze()
    {
        if (isCreated) return;
        mazeCellArray = new MazeCell[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                MazeCell cell = Instantiate(cellPrefab, this.transform);
                cell.SetUp(gridOrigin, i, j);
                mazeCellArray[i, j] = cell;
            }
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var cell = mazeCellArray[i, j];

                if (i > 0)
                    cell.SetNeighbor("BOTTOM", mazeCellArray[i - 1, j]);
                if (i < rows - 1)
                    cell.SetNeighbor("TOP", mazeCellArray[i + 1, j]);
                if (j > 0)
                    cell.SetNeighbor("LEFT", mazeCellArray[i, j - 1]);
                if (j < columns - 1)
                    cell.SetNeighbor("RIGHT", mazeCellArray[i, j + 1]);

                if (cell.rowIndex == 0 && cell.columnIndex == 0)
                    cell.SetGround(MazeCell.MazeCellGroundType.Start);
                else if (cell.rowIndex == rows - 1 && cell.columnIndex == rows - 1)
                    cell.SetGround(MazeCell.MazeCellGroundType.End);
                else
                    cell.SetGround(MazeCell.MazeCellGroundType.Normal);
            }
        }


        startCell = mazeCellArray[0, 0];
        endCell = mazeCellArray[rows - 1, columns - 1];
        currentCell = mazeCellArray[0,0];
        currentCell.isVisited = true;

        InvokeRepeating(nameof(MazeGeneratorLoop), 1, 0.02f);
    }
    public void ResetMaze()
    {
        mazeCellArray.ConvertTo<List<MazeCell>>().ForEach(cell => cell.ResetCell());
        currentCell = mazeCellArray[0, 0];
        currentCell.isVisited = true;
        currentCell.BreakWall("BOTTOM");
        InvokeRepeating(nameof(MazeGeneratorLoop), 1, 0.02f);
    }
    private void MazeGeneratorLoop()
    {
        if (currentCell != null)
        {
            MazeCell nextCell = currentCell.GetRandomNeighborUnvisited();
            if (nextCell != null)
            {
                nextCell.isVisited = true;
                mazeStack.Push(currentCell);
                RemoveWallBetween2Cell(currentCell, nextCell);
                currentCell = nextCell;
            }
            else if (mazeStack.StackLength > 0)
            {
                currentCell = mazeStack.Pull();
            }
            if (IsFullGridVisited())
            {
                CancelInvoke();
                Debug.Log("Generator finished");
                isCreated = true;
                foreach (var cell in mazeCellArray)
                {
                    if (cell.cellType == MazeCell.MazeCellGroundType.Start)
                        cell.BreakWall("BOTTOM");
                    if (cell.cellType == MazeCell.MazeCellGroundType.End)
                        cell.BreakWall("TOP");
                }
                GameManager.Instance.uiManager.SetNotice($"Finished! You can try escape the maze", 0, 3);
            }
        }
    }
    private bool IsFullGridVisited()
    {
        return mazeCellArray.ConvertTo<List<MazeCell>>().FindAll(cell => cell.isVisited == true).Count == (rows * columns);
    }    
    private void RemoveWallBetween2Cell(MazeCell cell1, MazeCell cell2)
    {
        if (cell1.rowIndex ==  cell2.rowIndex)
        {
            if (cell1.columnIndex == cell2.columnIndex - 1) //Cell1 is Left of Cell2
            {
                cell1.BreakWall("RIGHT");
                cell2.BreakWall("LEFT");
            }
            else if (cell1.columnIndex == cell2.columnIndex + 1) //Cell1 is Right of Cell2
            {
                cell1.BreakWall("LEFT");
                cell2.BreakWall("RIGHT");
            }
        }
        else if (cell1.columnIndex == cell2.columnIndex)
        {
            if (cell1.rowIndex == cell2.rowIndex - 1) //Cell1 is Bottom of Cell2
            {
                cell1.BreakWall("TOP");
                cell2.BreakWall("BOTTOM");
            }
            else if (cell1.rowIndex == cell2.rowIndex + 1) //Cell1 is Top of Cell2
            {
                cell1.BreakWall("BOTTOM");
                cell2.BreakWall("TOP");
            }
        }
    }
    #endregion

    #region Solver
    public List<MazeCell> GetExitPath(MazeCell standingCell)
    {
        openSet = new List<MazeCell>();
        closedSet = new List<MazeCell>();
        startCell = standingCell;
        endCell = mazeCellArray[rows - 1, columns - 1];
        openSet.Add(startCell);

        var isFinished = false;
        var isFindedPath = false;

        while (isFinished == false)
        {
            if (openSet.Count > 0)
            {
                int bestOption = 0;
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].f < openSet[bestOption].f)
                    {
                        bestOption = i;
                    }
                }
                currentCell = openSet[bestOption];
                if (currentCell == endCell)
                {
                    isFinished = true;
                    isFindedPath = true;
                }
                else
                {
                    openSet.Remove(currentCell);
                    closedSet.Add(currentCell);

                    var neighbors = currentCell.neighbors;
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        var neighbor = neighbors[i];
                        if (neighbor != null)
                        {
                            if (!closedSet.Contains(neighbor) && currentCell.canPass[i] == true)
                            {
                                var tempG = currentCell.g + 1;
                                var newPath = false;
                                if (openSet.Contains(neighbor))
                                {
                                    if (tempG < neighbor.g)
                                    {
                                        neighbor.g = tempG;
                                        newPath = true;
                                    }
                                }
                                else
                                {
                                    neighbor.g = tempG;
                                    newPath = true;
                                    openSet.Add(neighbor);
                                }
                                if (newPath)
                                {
                                    neighbor.h = GetHeuristicScore(neighbor, endCell);
                                    neighbor.f = neighbor.g + neighbor.h;
                                    neighbor.cameFrom = currentCell;
                                }
                            }
                        }                        
                    }
                }
            }
            else
            {
                //No solution
                isFinished = true;
            }
        };
        if (isFindedPath)
        {
            var listPath = new List<MazeCell>();
            var temp = currentCell;
            listPath.Add(currentCell);
            while(temp.cameFrom != null)
            {
                listPath.Add(temp.cameFrom);
                temp = temp.cameFrom;
            }
            listPath.Reverse();
            return listPath;
        }
        else
        {
            return null;
        }
    }

    private float GetHeuristicScore(MazeCell cellA, MazeCell cellB)
    {
        return Mathf.Abs(cellA.rowIndex - cellB.rowIndex) + Mathf.Abs(cellA.columnIndex -  cellB.columnIndex);
    }
    #endregion
}
