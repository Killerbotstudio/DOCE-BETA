using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{

    public List<Cell> allPositions;
    public GameObject board;

    public Cell checkCell;

    public Vector2 vertical = new Vector2(1, 0);
    public Vector2 horizontal = new Vector2(1, 0);

    void Start()
    {
        board = FindObjectOfType<BoardScript>().gameObject;
        foreach(Cell cell in board.GetComponentsInChildren<Cell>())
        {
            allPositions.Add(cell);
        }



        StartCoroutine(CellLineSelection());
    }

    
    void Update()
    {
        //Click();
    }


    public void Click()
    {
        if (Input.GetMouseButtonUp(0))
        {
            LayerMask layerMaskCell = LayerMask.GetMask("Cell");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskCell))
            {
                

                checkCell = hit.transform.GetComponent<Cell>();
                DebugSurroundingCells(checkCell);
                Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<" + checkCell + ">>>>>>>>>>>>>>>>>>>>>>>>>>");
            }
        }
    }

    public Cell SelectedCell()
    {
        //Cell clickCell = new Cell();
        Cell clickCell = null;
        if (Input.GetMouseButtonUp(0))
        {
            LayerMask layerMaskCell = LayerMask.GetMask("Cell");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskCell))
            {
                clickCell = hit.transform.GetComponent<Cell>();
                Debug.Log(clickCell);
            }
        }
        return clickCell;
    }



    IEnumerator CellLineSelection()
    {
        Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<" + checkCell + ">>>>>>>>>>>>>>>>>>>>>>>>>>");
        Cell cell1 = null;
        Cell cell2 = null;
        Cell cell3 = null;

        while (!cell1)
        {
            cell1 = SelectedCell();
            yield return null;
        }
        //Debug.Log(cell1);
        Cell tempCell2 = null;
        while (!cell2)
        {
            Cell temp = SelectedCell();
            if(CheckSurroundingPositions(temp, cell1)){
                Debug.Log("check");
            }
            //if (CheckSurroundingPositions(cell1, tempCell2))
            //{
            //    cell2 = tempCell2;
            //}
                
            
            yield return null;
        }




    }








    public void DebugSurroundingCells(Cell checkingCell)
    {
        foreach (Cell cell in allPositions)
        {
            cell.GetComponent<SpriteRenderer>().enabled = false;
        }
        List<Cell> surroundingCells = new List<Cell>();
        foreach (Cell cell in allPositions)
        {
            if(CheckSurroundingPositions(checkingCell, cell))
            {
                surroundingCells.Add(cell);
                cell.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        foreach(Cell cell in surroundingCells)
        {
            Debug.Log(checkCell.name + " " + checkCell.row + "," + checkCell.collum +  " chk>> (" + cell.name + " pos: " + cell.row + ","+cell.collum+")");
        }
    }












    //SAME METHOD AS IN GAME MANAGER
    public bool CheckSurroundingPositions(Cell lastCell, Cell newCell)
    {
        //notice this returns TRUE when surrounding a cell

        if (lastCell.row == newCell.row + 1)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (lastCell.row == newCell.row)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                //lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (lastCell.row == newCell.row - 1)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}
