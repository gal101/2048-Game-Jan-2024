using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public float offsetx;
    public float offsety;
    GameManager gameManager;
    Cell[] cells = new Cell[16];
    int[,] A = new int[4, 4];
    public GameObject cellPrefab;
    public Color[] cellColors = new Color[12];
    public Dictionary<int, int> ColorsDictionary = new Dictionary<int, int>();
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        InitColorDict();
        CreateBoard();
        GenerateRandomCell(2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            ArrowPress(0);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ArrowPress(1);
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ArrowPress(2);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ArrowPress(3);
        if (Input.GetKeyDown(KeyCode.R))
            ResetBoard();
    }

    void ArrowPress(int dir)
    {
        if(CountMovingSquares(dir) > 0)
        {
            int scoreadd = Move(dir);
            gameManager.AddScore(scoreadd);
            if (!BoardIsFull())
                GenerateRandomCell(1);
        }
    }

    void InitColorDict()
    {
        ColorsDictionary[0] = 0;
        int p = 2;
        for(int i = 1; i < 12; i++)
        {
            ColorsDictionary[p] = i;
            p *= 2;
        }
    }

    void CreateBoard()
    {
        for (int i = 0; i < 16; i++)
        {
            int linie = i / 4;
            int col = i % 4;
            GameObject newcell = Instantiate(cellPrefab, transform.position, Quaternion.identity);
            newcell.transform.SetParent(transform);

            float x = col * 85f;
            float y = -linie * 85f;
            Vector3 position = new Vector3(x + offsetx, y + offsety, 0f);
            newcell.transform.position = position + transform.position;
            newcell.GetComponent<RectTransform>().localScale = Vector3.one;
            newcell.GetComponent<RectTransform>().sizeDelta = new Vector2(80f, 80f);
            cells[i] = newcell.GetComponent<Cell>();
            cells[i].value = 0;
            cells[i].board = this;
            A[linie, col] = cells[i].value;
        }
    }

    void GetCellValues()
    {
        for(int i = 0; i < 4; i++)
            for(int j = 0; j < 4; j++)
                A[i, j] = cells[i * 4 + j].value;   
    }

    void PutCellValues()
    {
        for (int i = 0; i < 16; i++)
            cells[i].UpdateValue(A[i / 4, i % 4]);
    }

    public int Move(int dir)
    {
        GetCellValues();

        int[] vx = { 0, 1, 0, -1 };
        int x = vx[dir];

        int i, j, startj, finj = 4, incrj;
        int[] vsj = { 1, 2, 2, 1 };
        int[] vij = { 1, -1, -1, 1 };
        startj = vsj[dir];
        incrj = vij[dir];

        int scor = 0;
        for (i = 0; i < 4; i++)
        {
            bool imbinata = false;
            for (j = startj; j < finj; j += incrj)
            {
                if (j == -1) break;
                if (x != 0)
                {
                    if (A[i,j] > 0)
                    {
                        int poz = 1;
                        while (A[i, j - incrj * poz] == 0 && j - incrj * poz > 0 && j - incrj * poz < 3)
                        {
                            poz++;
                        }
                        if (A[i, j - incrj * poz] > 0)
                        {
                            if (A[i, j - incrj * poz] == A[i,j] && !imbinata)
                            {
                                imbinata = true;
                                A[i, j - incrj * poz] *= 2;
                                scor += A[i,j] * 2;
                                A[i,j] = 0;
                            }
                            else
                            {
                                int aux = A[i,j];
                                A[i, j] = 0;
                                A[i, j - incrj * (poz - 1)] = aux;
                                imbinata = false;
                            }
                        }
                        else
                        {
                            int aux = A[i,j];
                            A[i, j] = 0;
                            A[i, j - incrj * poz] = aux;
                        }
                    }
                }
                else
                {
                    if (A[j, i] > 0)
                    {
                        int poz = 1;
                        while (A[j - incrj * poz, i] == 0 && j - incrj * poz > 0 && j - incrj * poz < 3)
                        {
                            poz++;
                        }
                        if (A[j - incrj * poz, i] > 0)
                        {
                            if (A[j - incrj * poz, i] == A[j, i] && !imbinata)
                            {
                                imbinata = true;
                                A[j - incrj * poz, i] *= 2;
                                scor += A[j, i] * 2;
                                A[j, i] = 0;
                            }
                            else
                            {
                                int aux = A[j, i];
                                A[j, i] = 0;
                                A[j - incrj * (poz - 1), i] = aux;
                                imbinata = false;
                            }
                        }
                        else
                        {
                            int aux = A[j, i];
                            A[j, i] = 0;
                            A[j - incrj * poz, i] = aux;
                        }
                    }
                }
            }

        }
        PutCellValues();
        return scor;
    }

    public int CountMovingSquares(int dir)
    {

        int i, j, startj, finj = 4, incrj;
        int[,] B = new int[4, 4];
        for (i = 0; i < 16; i++)
            B[i / 4, i % 4] = cells[i].value;
        int[] vx = { 0, 1, 0, -1 };
        int x = vx[dir];

        
        int[] vsj = { 1, 2, 2, 1 };
        int[] vij = { 1, -1, -1, 1 };
        startj = vsj[dir];
        incrj = vij[dir];

        int nrmut = 0;

        for (i = 0; i < 4; i++)
        {
            bool imbinata = false;
            for (j = startj; j < finj; j += incrj)
            {
                if (j == -1) break;
                if (x != 0)
                {
                    if (B[i, j] > 0)
                    {
                        int poz = 1;
                        while (B[i, j - incrj * poz] == 0 && j - incrj * poz > 0 && j - incrj * poz < 3)
                        {
                            poz++;
                        }
                        if (B[i, j - incrj * poz] > 0)
                        {
                            if (B[i, j - incrj * poz] == B[i, j] && !imbinata)
                            {
                                imbinata = true;
                                B[i, j - incrj * poz] *= 2;
                                B[i, j] = 0;
                                nrmut++;
                            }
                            else
                            {
                                int aux = B[i, j];
                                B[i, j] = 0;
                                B[i, j - incrj * (poz - 1)] = aux;
                                imbinata = false;
                                if (poz > 1)
                                    nrmut++;
                            }
                        }
                        else
                        {
                            int aux = B[i, j];
                            B[i, j] = 0;
                            B[i, j - incrj * poz] = aux;
                            nrmut++;
                        }
                    }
                }
                else
                {
                    if (B[j, i] > 0)
                    {
                        int poz = 1;
                        while (B[j - incrj * poz, i] == 0 && j - incrj * poz > 0 && j - incrj * poz < 3)
                        {
                            poz++;
                        }
                        if (B[j - incrj * poz, i] > 0)
                        {
                            if (B[j - incrj * poz, i] == B[j, i] && !imbinata)
                            {
                                imbinata = true;
                                B[j - incrj * poz, i] *= 2;
                                nrmut++;
                                B[j, i] = 0;
                            }
                            else
                            {
                                int aux = B[j, i];
                                B[j, i] = 0;
                                B[j - incrj * (poz - 1), i] = aux;
                                imbinata = false;
                                if (poz > 1)
                                    nrmut++;
                            }
                        }
                        else
                        {
                            int aux = B[j, i];
                            B[j, i] = 0;
                            B[j - incrj * poz, i] = aux;
                            nrmut++;
                        }
                    }
                }
            }

        }
        
        return nrmut;
    }

    public void GenerateRandomCell(int x)
    {
        for(int i = 0; i < x; i++)
        {
            int position = Random.Range(0, 16);
            while (cells[position].value > 0)
                position = Random.Range(0, 16);
            int value = Random.Range(1, 3) * 2;
            cells[position].UpdateValue(value);
            A[position / 4, position % 4] = value;
        }
    }

    bool BoardIsFull()
    {
        for(int i = 0; i < 16; i++)
            if (cells[i].value == 0)
                return false;
        return true;
    }

    void ResetBoard()
    {
        for(int i = 0; i < 16; i++)
            cells[i].UpdateValue(0);
        GenerateRandomCell(2);
        GetCellValues();
        gameManager.ResetScore();
    }
}
