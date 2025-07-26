using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class OverviewFunction : MonoBehaviour
{

    public static OverviewFunction reference;

    Global_values GB_script;
    LabelMan MoneyManager;
    NetworkServer netServer;

    [SerializeField] TMP_Text Stats;
    [SerializeField] TMP_Text StockInfo;

    [SerializeField] GameObject[] StockBadges = new GameObject[4];
    [SerializeField] GameObject GameOver;

    int Quarter = 0;
    long StockPrice = 0;


    bool active_message = false;

    void Awake()
    {
        reference = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GB_script = Global_values.reference;
        MoneyManager = LabelMan.reference;
        netServer = NetworkServer.reference;
    }


    public void SceneInit()
    {
        //DisplayStats() Needs to be called first
        //GetStockPrice() reports asset price to the server
        DisplayStats();
        UpdateStockInfo();
    }

    //Give the server Asset price
    private long GetStockPrice()
    {
        float procentage = 0.25f;
        long Property = Global_values.Starting_station_price;
        long Assets = 0;
        foreach (var keyvalue in GB_script.Dic_item_amount)
        {
            string name = keyvalue.Key;
            long item_amount = keyvalue.Value;
            Assets = GB_script.Dic_item_price[name] * item_amount;
        }
        Property += Assets;
        netServer.UpdateAssetsRpc(Global_values.localID, Assets);
        long Profits = GB_script.ShortTermProfit + (long)(GB_script.AllProfit * procentage);
        return (Property + Profits);
    }

    public void UpdateStockInfo()
    {
        string info = "";
        netServer.requestGetOpponentStockRpc(Global_values.localID);
        long StockPrice = Global_values.OpponentStock;
        info += "25% Opponents Degan:\n" + MoneyManager.Format_number(StockPrice);
        StockInfo.text = info;
    }


    public void BuyStocks(TMP_Text button_label)
    {
        if (Global_values.money < StockPrice)
        {
            active_message = true;
            StartCoroutine(label_message(1.5f, "Strapped for cash", button_label));
        }
        else
        {
            Global_values.money -= StockPrice;
            StockBadges[Quarter].SetActive(true);
            Global_values.StockQuartersOwned += 1;
            Quarter += 1;
            netServer.StockBoughtRpc(Global_values.localID, StockPrice);
            if (Global_values.StockQuartersOwned == 4)
                GameOver.SetActive(false);
            //Server call to the other player;
        }
    }

    public void DisplayStats()
    {
        string text = "";
        text += "My Stock price: " + MoneyManager.Format_number(GetStockPrice()) + "\n";
        text += "Total Profit: " + MoneyManager.Format_number(GB_script.AllProfit) + "\n";
        text += "Profit THIS week: " + MoneyManager.Format_number(GB_script.ShortTermProfit) + "\n";
        Stats.text = text;
    }



    IEnumerator label_message(float delay, string msg, TMP_Text button_label)
    {
        string tmp = button_label.text;
        button_label.text = msg;
        yield return new WaitForSeconds(delay);
        button_label.text = tmp;
        active_message = false;
    }
}
