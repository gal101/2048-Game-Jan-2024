using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int value;
    public Text valueText;
    public Image cellImage;
    public Board board;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValue(int x)
    {
        value = x;
        if (x > 0)
            valueText.text = x.ToString();
        else
            valueText.text = "";
        cellImage.color = board.cellColors[board.ColorsDictionary[value]];
    }
}
