using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FieldGen : MonoBehaviour
{
    [SerializeField] int pageNum;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] TextAsset data;
    [SerializeField] int bombCount;
    [SerializeField] TextMeshProUGUI bombAndFlagCountText;
    private static float cellSize = 57.6f;
    private static Vector3 origin = new Vector3(-450, 100, 0);
    private static int colSize = 16;
    private int rowSize;
    private List<Cell> cells = new List<Cell>();
    private bool isFirst = true;
    public int flagCount = 0;
    public int openedCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Initialize(data.text);
        FetchUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Scene loadScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadScene.name);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(pageNum + 1);
        }
    }

    private void Initialize(string sentence)
    {
        GameObject canvas = GameObject.Find("Canvas");

        int r = 0, c = 0;
        int i = 0;
        while (sentence.Length > i)
        {
            if (c >= colSize)
            {
                r--;
                c = 0;
            }
            GameObject instance = Instantiate(cellPrefab, origin + new Vector3(cellSize / 2 + cellSize * c, cellSize / 2 + cellSize * r, 0), Quaternion.identity);
            instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sentence[i].ToString();
            instance.transform.SetParent(canvas.transform, false);
            cells.Add(instance.GetComponent<Cell>());
            cells[i].thisOrder = i;
            c++;
            i++;
        }
        rowSize = -r + 1;
        Debug.Log(rowSize);
        Debug.Log(cells.Count);
    }

    public void CheckFirstOpen(int order)
    {
        if (isFirst)
        {
            isFirst = false;
            BombGenerate(order, data.text);
        }
        return;
    }

    private void BombGenerate(int order, string sentence)
    {
        int r = (int)GetCellPosition(order).x;
        int c = (int)GetCellPosition(order).y;
        int size = sentence.Length;
        int i = 0;
        List<int> bomb = new List<int>();
        List<int> numbers = new List<int>();
        for (int j = 0; j < size; j++)
        {
            numbers.Add(j);
        }
        numbers.RemoveAt(r * colSize + c);
        while (i < bombCount)
        {
            int rand = Random.Range(0, numbers.Count);
            bomb.Add(numbers[rand]);
            numbers.RemoveAt(rand);
            i++;
        }
        bomb.Sort();
        for (int j = 0; j < bomb.Count; j++)
        {
            cells[bomb[j]].isBomb = true;
        }
        for (int j = 0; j < cells.Count; j++)
        {
            int count = 0;
            //左上
            if (GetCellPosition(j).x > 0 && GetCellPosition(j).y > 0)
                if (cells[GetCellOrder((int)GetCellPosition(j).x - 1, (int)GetCellPosition(j).y - 1)].isBomb)
                    count++;
            //上
            if (GetCellPosition(j).x > 0)
                if (cells[GetCellOrder((int)GetCellPosition(j).x - 1, (int)GetCellPosition(j).y)].isBomb)
                    count++;
            //右上
            if (GetCellPosition(j).x > 0 && GetCellPosition(j).y < colSize - 1)
                if (cells[GetCellOrder((int)GetCellPosition(j).x - 1, (int)GetCellPosition(j).y + 1)].isBomb)
                    count++;
            //左
            if (GetCellPosition(j).y > 0)
                if (cells[GetCellOrder((int)GetCellPosition(j).x, (int)GetCellPosition(j).y - 1)].isBomb)
                    count++;
            //右
            if (GetCellPosition(j).y < colSize - 1 && GetCellOrder((int)GetCellPosition(j).x, (int)GetCellPosition(j).y + 1) < cells.Count)
                if (cells[GetCellOrder((int)GetCellPosition(j).x, (int)GetCellPosition(j).y + 1)].isBomb)
                    count++;
            //左下
            if (GetCellPosition(j).x < rowSize - 1 && GetCellPosition(j).y > 0 && GetCellOrder((int)GetCellPosition(j).x + 1, (int)GetCellPosition(j).y - 1) < cells.Count)
                if (cells[GetCellOrder((int)GetCellPosition(j).x + 1, (int)GetCellPosition(j).y - 1)].isBomb)
                    count++;
            //下
            if (GetCellPosition(j).x < rowSize - 1 && GetCellOrder((int)GetCellPosition(j).x + 1, (int)GetCellPosition(j).y) < cells.Count)
                if (cells[GetCellOrder((int)GetCellPosition(j).x + 1, (int)GetCellPosition(j).y)].isBomb)
                    count++;
            //右下
            if (GetCellPosition(j).x < rowSize - 1 && GetCellPosition(j).y < colSize - 1 && GetCellOrder((int)GetCellPosition(j).x + 1, (int)GetCellPosition(j).y + 1) < cells.Count)
                if (cells[GetCellOrder((int)GetCellPosition(j).x + 1, (int)GetCellPosition(j).y + 1)].isBomb)
                    count++;

            cells[j].neighbourBombCount = count;
        }
    }

    public void OpenNeighbourCell(int order)
    {
        //左上
        if (GetCellPosition(order).x > 0 && GetCellPosition(order).y > 0 && GetCellOrder((int)GetCellPosition(order).x - 1, (int)GetCellPosition(order).y - 1) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x - 1, (int)GetCellPosition(order).y - 1)].Open(OpenType.Force);
        //上
        if (GetCellPosition(order).x > 0 && GetCellOrder((int)GetCellPosition(order).x - 1, (int)GetCellPosition(order).y) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x - 1, (int)GetCellPosition(order).y)].Open(OpenType.Force);
        //右上
        if (GetCellPosition(order).x > 0 && GetCellPosition(order).y < colSize - 1 && GetCellOrder((int)GetCellPosition(order).x - 1, (int)GetCellPosition(order).y + 1) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x - 1, (int)GetCellPosition(order).y + 1)].Open(OpenType.Force);
        //左
        if (GetCellPosition(order).y > 0 && GetCellOrder((int)GetCellPosition(order).x, (int)GetCellPosition(order).y - 1) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x, (int)GetCellPosition(order).y - 1)].Open(OpenType.Force);
        //右
        if (GetCellPosition(order).y < colSize - 1 && GetCellPosition(order).y + 1 < cells.Count && GetCellOrder((int)GetCellPosition(order).x, (int)GetCellPosition(order).y + 1) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x, (int)GetCellPosition(order).y + 1)].Open(OpenType.Force);
        //左下
        if (GetCellPosition(order).x < rowSize - 1 && GetCellPosition(order).y > 0 && GetCellOrder((int)GetCellPosition(order).x + 1, (int)GetCellPosition(order).y - 1) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x + 1, (int)GetCellPosition(order).y - 1)].Open(OpenType.Force);
        //下
        if (GetCellPosition(order).x < rowSize - 1 && GetCellPosition(order).y < colSize - 1 && GetCellOrder((int)GetCellPosition(order).x + 1, (int)GetCellPosition(order).y) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x + 1, (int)GetCellPosition(order).y)].Open(OpenType.Force);
        //右下
        if (GetCellPosition(order).x < rowSize - 1 && GetCellPosition(order).y < colSize - 1 && GetCellOrder((int)GetCellPosition(order).x + 1, (int)GetCellPosition(order).y + 1) < cells.Count)
            cells[GetCellOrder((int)GetCellPosition(order).x + 1, (int)GetCellPosition(order).y + 1)].Open(OpenType.Force);
    }
    public void OpenBombCell()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].isBomb)
                cells[i].Open(OpenType.Reveal);
        }
    }

    public void FetchUI()
    {
        bombAndFlagCountText.text = "Bomb: " + bombCount + "\nFlag: " + flagCount;
    }

    public void CheckGameClear()
    {
        if (openedCount == cells.Count - bombCount)
        {
            Debug.Log("Game Clear");
            SceneManager.LoadScene(pageNum + 1);
            // gameClear = true;
        }
    }


    private Vector2 GetCellPosition(int order)
    {
        int r = order / colSize;
        int c = order % colSize;
        return new Vector2(r, c);
    }

    private int GetCellOrder(int r, int c)
    {
        if (r < 0 || c < 0 || r >= rowSize || c >= colSize)
        {
            Debug.LogError("GetCellOrder Error");
            return -1;
        }
        return r * colSize + c;
    }
}
