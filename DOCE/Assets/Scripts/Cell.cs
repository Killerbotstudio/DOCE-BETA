////////////Cell////////////
using UnityEngine;
[System.Serializable]
public class Cell : MonoBehaviour
{
    public string cellName;
    public int value;
    public bool free;
    public bool blocked;
    public int row;
    public int collum;
    public GameObject player;
    public SpriteRenderer decal;
    public SpriteRenderer background;

    public void Start()
    {
        CellReset();
    }
    public void CellReset()
    {
        //cellName = gameObject.name;
        player = null;
        value = 0;
        blocked = false;
        free = true;
        decal.sprite = null;

    }
    public void CallPieceDATA(PlayerPiece piece)
    {
        player = piece.player;
        free = false;
        if (!piece.blocker)
        {
            decal.sprite = piece.decal.sprite;
            decal.color = piece.decal.color;
            decal.enabled = true;
            value = piece.value;
            
        } else
        {
            decal.sprite = piece.decal.sprite;
            decal.color = piece.decal.color;
            decal.enabled = true;
            blocked = true;
        }
    }
}
