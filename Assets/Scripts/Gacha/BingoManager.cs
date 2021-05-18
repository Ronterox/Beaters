using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Plugins.Tools;

public class BingoManager : MonoBehaviour
{
    public GameObject panelBingo;
    public GameObject[] bingoBoxes;
    public Button redeemReward;
    private const int OPEN_BINGO_GACHA = 6578;

    void Start() {

    redeemReward.interactable = false;

    for(int i =0; i < DataManager.Instance.playerData.bingoBoxes; i++)
            {
                bingoBoxes[i].SetActive(true);
            }

    checkButton();

    if(GameManager.GetValue() is int value && value == OPEN_BINGO_GACHA) {
            panelBingo.SetActive(true);
            GameManager.PutValue(null);
            if(DataManager.Instance.playerData.bingoBoxes == 9){
                DataManager.Instance.playerData.bingoBoxes = 8;
            }
            bingoBoxes[DataManager.Instance.playerData.bingoBoxes++].SetActive(true);
            checkButton();
        }
    }

    void checkButton(){
        if(DataManager.Instance.playerData.bingoBoxes == 9){
            redeemReward.interactable = true;
        }
    }
}
