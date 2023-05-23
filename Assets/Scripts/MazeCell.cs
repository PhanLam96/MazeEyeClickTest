using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeCell : MonoBehaviour
{
    public enum MazeCellGroundType
    {
        Normal,
        Start,
        End,
        Path
    }
    public int rowIndex;
    public int columnIndex;
    public bool isVisited;
    public List<MazeCell> neighbors = new List<MazeCell>();
    public List<bool> canPass;
    public MazeCell cameFrom;
    public MazeCellGroundType cellType;

    public float f; // fCost
    public float g; // gCost is step from StartCell to ThisCell
    public float h; // hCost is step from ThisCell to EndCell (with fly way)

    [SerializeField] private List<GameObject> walls;
    [SerializeField] private List<GameObject> grounds;


    private void Awake()
    {
        canPass = new List<bool> { false, false, false, false };
        neighbors = new List<MazeCell>() { null, null, null, null };
    }

    public void SetUp(Transform origin, int rowIndex, int columnIndex)
    {
        float posX = origin.position.x + columnIndex;
        float posZ = origin.position.z + rowIndex;
        transform.position = new Vector3(posX, 0, posZ);
        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;
        this.gameObject.name = $"MazeCell_{rowIndex}{columnIndex}";
    }
    public void SetNeighbor(string neighborSide, MazeCell cell)
    {
        var neighborIndex = neighborSide switch
        {
            "TOP" => 0,
            "RIGHT" => 1,
            "BOTTOM" => 2,
            "LEFT" => 3,
            _ => -1
        };
        neighbors[neighborIndex] = cell;
    }
    public void BuildWall(string wallSide)
    {
        var wallIndex = wallSide switch
        {
            "TOP" => 0,
            "RIGHT" => 1,
            "BOTTOM" => 2,
            "LEFT" => 3,
            _ => -1
        };

        StartCoroutine(Utils.IEDelayCall(() => walls[wallIndex].SetActive(true), 1));
    }

    public void BreakWall(string wallSide)
    {
        var wallIndex = wallSide switch
        {
            "TOP" => 0,
            "RIGHT" => 1,
            "BOTTOM" => 2,
            "LEFT" => 3,
            _ => -1
        };
        walls[wallIndex].SetActive(false);
        canPass[wallIndex] = true;
    }
    public void SetGround(MazeCellGroundType groundType)
    {
        cellType = groundType;

        var groundIndex = groundType switch
        {
            MazeCellGroundType.Normal => 0,
            MazeCellGroundType.Start => 1,
            MazeCellGroundType.End => 2,
            MazeCellGroundType.Path => 3,
            _ => -1
        };

        for (int i = 0; i < grounds.Count; i++)
        {
            grounds[i].SetActive(i == groundIndex);
        }
    }
    public MazeCell GetRandomNeighborUnvisited()
    {
        List<MazeCell> listNeighbourValid = new List<MazeCell>();

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
            {
                if (!neighbor.isVisited)
                {
                    listNeighbourValid.Add(neighbor);
                }
            }
        }        

        if (listNeighbourValid.Count > 0)
        {
            return listNeighbourValid[Random.Range(0, listNeighbourValid.Count)];
        }
        else
        {
            return null;
        }
    }
    public void ResetCell()
    {
        isVisited = false;
        walls.ForEach(wall => wall.SetActive(true));
    }
}
