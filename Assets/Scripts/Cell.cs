using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public bool isBomb = false;
    public CellState state;
    public int neighbourBombCount = 0;
    public int thisOrder;
    private TextMeshProUGUI text;
    private string originChar;
    private FieldGen field;
    private CircleCollider2D collider;
    [SerializeField] private AudioClip bombSE;

    private void Start()
    {
        text = this.GetComponentInChildren<TextMeshProUGUI>();
        originChar = text.text;
        field = GameObject.Find("Field").GetComponent<FieldGen>();
        collider = this.GetComponent<CircleCollider2D>();
    }
    public void OnLeftOrRightClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Open(OpenType.Normal);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ChangeMark();
        }
    }

    public void Open(OpenType type)
    {
        field.CheckFirstOpen(thisOrder);
        if (type == OpenType.Normal)
        {
            if (state == CellState.Opened || state == CellState.Flag || state == CellState.Question)
                return;
        }
        else
        {
            if (state == CellState.Opened)
                return;
        }
        state = CellState.Opened;
        field.openedCount++;
        if (isBomb)
        {
            text.text = "X";
            if (type != OpenType.Reveal) field.OpenBombCell();
            GetComponent<AudioSource>().PlayOneShot(bombSE);
            return;
        }
        else if (neighbourBombCount == 0)
        {
            text.text = string.Empty;
            field.OpenNeighbourCell(thisOrder);
        }
        else
        {
            text.text = neighbourBombCount.ToString();
        }
        this.GetComponent<Image>().enabled = false;
        field.CheckGameClear();
    }
    // セルにマークを付けます
    private void ChangeMark()
    {
        switch (state)
        {
            case CellState.Closing:
                state = CellState.Flag;
                // フラグは適当な文字で代替してます
                text.text = "F";
                field.flagCount++;
                field.FetchUI();
                break;
            case CellState.Flag:
                state = CellState.Question;
                text.text = "?";
                field.flagCount--;
                field.FetchUI();
                break;
            case CellState.Question:
                state = CellState.Closing;
                text.text = originChar;
                break;
            default:
                break;
        }
    }
}

// フィールド上のセルがどのような状態にあるかを表します
public enum CellState
{
    Closing = 0,    // セルが閉じられた状態
    Opened = 1,     // セルが展開された状態
    Flag = 2,       // セルにフラグが立てられた状態
    Question = 3    // セルに？が付けられた状態
}

public enum OpenType
{
    Normal = 0,
    Force = 1,
    Reveal = 2
}