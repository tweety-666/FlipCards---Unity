using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    public CardState cardState;
    public CardPattern cardPattern;
    public GameManager gameManager;
   
    //一開始狀態都是未翻牌
    void Start () {
        cardState = CardState.未翻牌;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();//被生成到場景物件的預製資源，需要掛載到Card腳本組件
    }
    //點牌，如果已經翻開不能再執行動作
    //一次只能翻兩張，所以已經開始比較卡牌時，不能再繼續翻其他牌
    //翻開卡牌，加入比較清單
    //加入清單後開始比較
    public void OnMouseUp()
    {
        if (cardState.Equals(CardState.已翻牌))
        {
            return;
        }
        if (gameManager.ReadyToCompareCards)
        {
            return;
        }

        openCard();
        gameManager.addCardInCardComparison(this);
        gameManager.compareCardsInList();
    }
    void openCard()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
        cardState = CardState.已翻牌;
    }
    public enum CardState {未翻牌 ,已翻牌 ,配對成功};
    public enum CardPattern
    {
        無, 奇異果, 柳橙, 橘子, 水蜜桃, 芭樂, 葡萄, 蘋果, 西瓜
    }
}
