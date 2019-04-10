using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("比對卡牌清單")]
    public List<Card> cardComparison;
    [Header("卡牌種類清單")]
    public List<Card.CardPattern> cardsToBePutIn;
    public Transform[] positions;
    public int matchedCardsCount = 0; //成功配對卡牌的數量，初始為零

    // 載入遊戲要先隨機生成卡牌
    void Start () {
        generateRandomCards();
        //setUpCardsToBePutIn(); //看是否能放入8個卡牌
        //addNewCard(Card.CardPattern.水蜜桃); //這行是測試能否自動生成卡牌水蜜桃

    }

    void setUpCardsToBePutIn(){
        //把物件種類為Card腳本內CardPattern的Enum(水果名稱)轉成陣列，因為要把這個水果卡牌陣列加入空陣列cardsToBePutIn
        Array array = Enum.GetValues(typeof(Card.CardPattern)); 
        foreach (var item in array)
        {
            cardsToBePutIn.Add((Card.CardPattern)item);//強制轉型陣列理的東西變成CardPattern
        }
        cardsToBePutIn.RemoveAt(0);//刪掉CardPattern.無
    }

    void generateRandomCards()//發牌
    {
        int positionIndex = 0; //從位置0開始放牌

        for (int i = 0; i < 2; i++) //讓下方迴圈執行2次，共放16次牌
        {
            setUpCardsToBePutIn();//準備卡牌
            int maxRandomNumber = cardsToBePutIn.Count;//最大亂數不超過陣列內項目數目
            for (int j = 0; j < maxRandomNumber; maxRandomNumber--) //執行8次
            {
                int randomNumber = UnityEngine.Random.Range(0, maxRandomNumber);//0到8之間產生亂數 最小是0 最大是7
                addNewCard(cardsToBePutIn[randomNumber], positionIndex);//從陣列中依照亂數抽牌，依序放入位置
                cardsToBePutIn.RemoveAt(randomNumber);//放入後就從陣列中移除
                positionIndex++;//放完後換下一個位置放
            }
        }
    }


    //隨機生成卡牌的函式
    void addNewCard(Card.CardPattern cardPattern, int positionIndex)
    {
        GameObject card = Instantiate(Resources.Load<GameObject>("Prefabs/牌")); //從Resources資料夾中的Prefabs資料夾拿圖，載入卡牌背面
        card.GetComponent<Card>().cardPattern = cardPattern; //取得card物件中的cardPattern組件中的cardPattern
        card.name = "牌_" + cardPattern.ToString(); //利用上方cardPattern的名字，賦予card物件名字
        card.transform.position = positions[positionIndex].position;

        GameObject graphic = Instantiate(Resources.Load<GameObject>("Prefabs/圖"));//丟入預製資源到物件欄中
        graphic.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Graphics/" + cardPattern.ToString());//載入圖片，變成卡牌圖
        graphic.transform.SetParent(card.transform);//變成牌的子物件
        graphic.transform.localPosition = new Vector3(0, 0, 0.1f);//設定座標
        graphic.transform.eulerAngles = new Vector3(0, 180, 0);//順著Y軸轉180度 翻牌時不會左右顛倒
    }

    //把牌加到cardComparison清單裡
    public void addCardInCardComparison(Card card)
    {
        cardComparison.Add(card);
    }

    //把牌加到清單裡之後，確認是否有兩張牌，也只能有兩張牌
    public bool ReadyToCompareCards
    {
        get
        {
            if (cardComparison.Count == 2) { return true; }
            else { return false; }
        }
    }
    //有兩張牌就開始比對
    //兩張一樣，顯示狀態配對成功，不能再翻回去
    //兩張不一樣，翻牌後不一樣，等1.5秒才翻回去，並清除卡牌比較
    public void compareCardsInList()
    {
        if (ReadyToCompareCards)
        {
            Debug.Log("有兩張牌了，可以開始比對卡牌。");
            if (cardComparison[0].cardPattern == cardComparison[1].cardPattern)
            {
                Debug.Log("兩張牌一樣");
                foreach (var card in cardComparison)
                {
                    card.cardState = Card.CardState.配對成功;
                }
                clearCardComparison();
                matchedCardsCount = matchedCardsCount + 2; 
                if (matchedCardsCount >= positions.Length)
                {
                    StartCoroutine(ReloadScene());
                }
            }
            else
            {
                Debug.Log("兩張牌不一樣");
                StartCoroutine(missMatchCards());
            }
        };
    }
    //清除卡牌比較的函式
    void clearCardComparison()
    {
        cardComparison.Clear();
    }
    //把牌翻回去的函式
    void turnBackCards()
    {
        foreach (var card in cardComparison)
        {
            card.gameObject.transform.eulerAngles = Vector3.zero;
            //card.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            card.cardState = Card.CardState.未翻牌;
        }
    }
    //翻牌後不一樣，等1.5秒才翻回去，並清除卡牌比較
    IEnumerator missMatchCards()
    {
        yield return new WaitForSeconds(1.5f);
        turnBackCards();
        clearCardComparison();
    }
    //全部配對成功後，3秒後自動重新洗牌
    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}





