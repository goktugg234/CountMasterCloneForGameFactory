using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GateManager : MonoBehaviour
{
    public TextMeshPro GateNo;
    public int randomNumber;
    public bool multiplyToStickmans;
    void Start()
    {
        if (multiplyToStickmans)
        {
            randomNumber = Random.Range(1, 3);
            GateNo.text = "X" + randomNumber;
        }
        else
        {
            randomNumber = Random.Range(30, 70);

            if (randomNumber % 2 != 0)
                randomNumber += 1;
            
            GateNo.text ="+" + randomNumber.ToString();
        }
    }
}