//////////////BoardScript//////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    private GameManager manager;
    public Cell[,] cellsOfBoard;
    public List<Cell> A, B, C, D, E;
    public Cell prefabCell;
    public Cell blankCell;
    public List<Cell>[] Boards;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        SetUpBoard(7, 7);
    }

    private void SetUpBoard(int rows, int collums)
    {
        cellsOfBoard = new Cell[rows, collums];
        Boards = new List<Cell>[] { A, B, C, D, E };
        for(int j = 1; j < rows-1; j++)
        {
            for (int i = 1; i < collums - 1; i++)
            {
                cellsOfBoard[j, i] = Boards[j-1][i - 1];
            }
        }
    }
}

