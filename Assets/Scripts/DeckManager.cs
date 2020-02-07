using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{


    enum CardsInfo
    {
        Text,
        NumberCards,
        Description

    };
    // Start is called before the first frame update
    public static DeckManager deckManager;
    public DataManager data;

    [SerializeField] public GameObject prefabsLign;
    [SerializeField] public GameObject prefabsCards;
    [SerializeField] public GameObject containsLign;
    [SerializeField] public GameObject canvasDeck;
    [SerializeField] public GameObject canvasBooster;
    [SerializeField] public GameObject textNumberBooster;
    [SerializeField] public int numberCardsByLign = 10;
    private int numberLign = 0;
    private bool haveThisCards = false;
    int numberBooster = 0;
  
    void Start()
    {
        if (DeckManager.deckManager == null)
        {
            DeckManager.deckManager = this;
        }
        else
        {
            if (DeckManager.deckManager != this)
            {
                Destroy(DeckManager.deckManager.gameObject);
                DeckManager.deckManager = this;
            }

        }


        DontDestroyOnLoad(this.gameObject);
       data = DataManager.dataManager;
}

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisplayCards()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
        {
            CatalogVersion = "Cards"
        },
               result => { ShowAllDeck(result); },
               error => Debug.Log(error.GenerateErrorReport())); ;
    }

    public void DisplayMyCards()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result => { ShowMyInventory(result);}, error => Debug.Log(error.GenerateErrorReport())); 
    }


    private void ShowAllDeck(GetCatalogItemsResult result)
    {

        numberLign = (int)(result.Catalog.Count / numberCardsByLign);
        Debug.Log(numberLign);

        for(int i = 0; i  < numberLign; i++)
        {
            GameObject go = Instantiate(prefabsLign, containsLign.transform);
            for(int j= 0; j < numberCardsByLign; j++)
            {
                if (result.Catalog[j + i* numberCardsByLign].ItemClass == "Cards")
                {
                    GameObject go2 = Instantiate(prefabsCards, go.transform);
                    go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().text = result.Catalog[j + i * numberCardsByLign].ItemId;
                    for(int k = 0; k < result.Catalog[j + i * numberCardsByLign].Tags.Count; k++)
                    {
                        if (result.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Commun"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color  = Color.black;
                        }
                        if (result.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Rare"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.blue;
                        }
                        if (result.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Epic"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.magenta;
                        }
                        if (result.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Legendaire"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.red;
                        }
                    }


                }
                    
            }
        }

        canvasDeck.SetActive(true);
        /*if (result.Catalog[i].ItemClass == "Cards")
            Debug.Log(result.Catalog[i].DisplayName);*/    
    }


  


    private void ShowMyInventory(GetUserInventoryResult result)
    {

        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
        {
            CatalogVersion = "Cards"
        },
               result2 => { ShowMyDeck(result2, result); },
               error => Debug.Log(error.GenerateErrorReport())); ;
    }



    private void ShowMyDeck(GetCatalogItemsResult resultCatalog, GetUserInventoryResult resultInventory)
    {

        numberLign = (int)(resultCatalog.Catalog.Count / numberCardsByLign);
        
        for (int i = 0; i < numberLign; i++)
        {
            GameObject go = Instantiate(prefabsLign, containsLign.transform);
            for (int j = 0; j < numberCardsByLign; j++)
            {
                if (resultCatalog.Catalog[j + i * numberCardsByLign].ItemClass == "Cards")
                {
                    haveThisCards = false;
                    GameObject go2 = Instantiate(prefabsCards, go.transform);
                    
                    go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().text = resultCatalog.Catalog[j + i * numberCardsByLign].ItemId;

                    for (int k = 0; k < resultCatalog.Catalog[j + i * numberCardsByLign].Tags.Count; k++)
                    {
                        if (resultCatalog.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Commun"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.black;
                        }
                        if (resultCatalog.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Rare"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.blue;
                        }
                        if (resultCatalog.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Epic"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.magenta;
                        }
                        if (resultCatalog.Catalog[j + i * numberCardsByLign].Tags[k].Equals("Legendaire"))
                        {
                            go2.transform.GetChild((int)CardsInfo.Text).GetComponent<Text>().color = Color.red;
                        }
                    }


                    for (int k = 0; k < resultInventory.Inventory.Count; k++)
                    {
                        if(resultInventory.Inventory[k].ItemId == resultCatalog.Catalog[j + i * numberCardsByLign].ItemId)
                        {
                            haveThisCards = true;
                            go2.transform.GetChild((int)CardsInfo.NumberCards).GetComponent<Text>().text = resultInventory.Inventory[k].RemainingUses.Value.ToString();
                            k = resultInventory.Inventory.Count;

                            

                        }
                    }



                
                    if (!haveThisCards)
                    {
                        Debug.Log("JetBrains rentre ici");
                        go2.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                       go2.transform.GetChild((int)CardsInfo.NumberCards).GetComponent<Text>().text = "0";
                    }
                    
      
                    
                }

            }
        }

        canvasDeck.SetActive(true);
        /*if (result.Catalog[i].ItemClass == "Cards")
            Debug.Log(result.Catalog[i].DisplayName);*/
    }




       public void ObtainBooster()
      {
        if (numberBooster > 0)
        {
            PlayFabClientAPI.UnlockContainerItem(new UnlockContainerItemRequest()
            {

                CatalogVersion = "Cards",
                ContainerItemId = "Booster"
            }, result => { RandomLootTable(result); },
            error => Debug.Log(error.GenerateErrorReport())); ;

        }

    }

    private void RandomLootTable(UnlockContainerItemResult result)
    {
        for(int i = 0; i < result.GrantedItems.Count; i++)
        {
            Debug.Log(result.GrantedItems[i].DisplayName);
            
        }
        numberBooster -= 1;
        textNumberBooster.GetComponent<Text>().text = numberBooster.ToString();
    }


    public void GetVirtualRF(int price)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
             result3 => { CheckMoney(price, result3); },
            error => { Debug.LogError(error.GenerateErrorReport()); });

    }


    private void CheckMoney(int price, GetUserInventoryResult result)
    {
        if(price > result.VirtualCurrency["RF"])
        {
            Debug.Log("PAS ASSEZ DE RF");
        }
        else
        {
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
            {
                ItemId = "Booster",
                CatalogVersion = "Cards",
                VirtualCurrency = "RF",
                Price = price
            },
            result2 => { GetNewRF(); },
            error2 => Debug.Log(error2.GenerateErrorReport())) ; ;
        }
    }


    private void GetNewRF()
    {
        Debug.Log("Booster acheté");
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
         result3 => { data.SetRuneFragment(result3.VirtualCurrency["RF"]); },
          error => { Debug.LogError(error.GenerateErrorReport()); });
    }





    public void NumberBooster()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result => { ShowNumberBooster(result); }, error => Debug.Log(error.GenerateErrorReport()));
    }


    void ShowNumberBooster(GetUserInventoryResult result)
    {

        for(int i = 0; i < result.Inventory.Count;i++)
        {
            if(result.Inventory[i].ItemClass == "Booster")
            {
                numberBooster++;
            }
        }
        canvasBooster.SetActive(true);
        textNumberBooster.GetComponent<Text>().text = numberBooster.ToString();
    
    }
}




