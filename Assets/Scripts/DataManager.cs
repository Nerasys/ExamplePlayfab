using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class DataManager : MonoBehaviour
{

    #region Variable
    public static DataManager dataManager;

    //Identifiant//
    private string username;
    private string password;
    private string email;
    public string id;
    private int playerLevel = 1;
    private int fragmentRune = 0;
    private int IRLMonney = 0;
    #endregion



    // Start is called before the first frame update
    void Awake()
    {

     
        if (DataManager.dataManager == null)
        {
            DataManager.dataManager = this;
        }
        else
        {
            if (DataManager.dataManager != this)
            {
                Destroy(DataManager.dataManager.gameObject);
                DataManager.dataManager = this;
            }

        }

    
      DontDestroyOnLoad(this.gameObject);

    }


    //SET ET GET DES VARIABLES DU DATA MANAGER POUR LES STATS
    #region SET/GET PLAYERSTATS
    public string GetUsername()
    {

        return username;

    }
    public void SetUsername(string p_Username)
    {
        username = p_Username;
    }

    public string GetPassword()
    {
        return password;
    }
    public void SetPassword(string p_Password)
    {
        password = p_Password;
    }

    public string GetEmail()
    {
        return email;
    }
    public void SetEmail(string p_Email)
    {
        email = p_Email;
    }

    public int GetPlayerLevel()
    {
        return playerLevel;
    }
    public void SetPlayerLevel(int p_level)
    {
        playerLevel = p_level;
    }

    public int GetRuneFragment() {return fragmentRune;}
    public void SetRuneFragment(int p_runeFragment) { fragmentRune = p_runeFragment; }

    public int GetIRLMonney() { return IRLMonney; }
    public void SetIRLMonney(int p_IRLMonney) { IRLMonney = p_IRLMonney; }


    #endregion
    // Update is called once per frame

    #region FUNCTION STATS REQUEST

    //requete pour envoyer les valeurs INGAME



    //requete pOUR RECUPERER LES STATS EN APPELANT ONGETSTATS
    public void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics( new GetPlayerStatisticsRequest(),OnGetStats, error => Debug.LogError(error.GenerateErrorReport())  );
    }
    //CHECK LE CLOUD et return la bonne valeur
    void OnGetStats(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            //Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "PlayerLevel":
                    dataManager.SetPlayerLevel(eachStat.Value);
                    break;
              
             
            }
        }
       
            
      //  dataManager.AfkFarmHorsLigne();
    }

    //FUNCTION DE CLOUD

    


    #endregion


    #region function LEADERBOARD

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    // RECUPERER LES DIX JOUEURS LES PLUS HAUT POUR UNE STATE
    public void GetSlimeBoarder()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "PlayerSlime", MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeadBoard, OnErrorLeaderboard);

    }

    //---------------------------------------------------

    // creer le tableau dynamiquement avec les bon joueurs
    void OnGetLeadBoard(GetLeaderboardResult result)
    {
       //extLeader.text = " Slime Point ";
       // leaderboardPanel.SetActive(true);
        // Debug.Log(result.Leaderboard[0].StatValue);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
          //  GameObject tempListing = Instantiate(listingPrefab, listingContainer);
          //  LeaderBoardListing LL = tempListing.GetComponent<LeaderBoardListing>();
          //  LL.playerNameText.text = player.DisplayName;
         //   LL.playerScoreText.text = player.StatValue.ToString();
         //   Debug.Log(player.DisplayName + ": " + player.StatValue);
        }

    }
   
   


    #endregion

    #region UI BUTTON




    public void CloseLeaderboardPanel()
    {

      //  leaderboardPanel.SetActive(false);
      //  for (int i = listingContainer.childCount - 1; i >= 0; i--)
        {
      //      Destroy(listingContainer.GetChild(i).gameObject);
        }
    }

    
    void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    #endregion


}
