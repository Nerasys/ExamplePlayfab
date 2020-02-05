using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayFab_Login : MonoBehaviour
{
    // Start is called before the first frame update

    #region variable
    DataManager dataManager;

    private string userEmail;
    private string userPassword;
    private string username;
    private string userConfirmPassword;
    private string userConfirmEmail;

    public Text errorMessage;
    public InputField IFRegisterUsername;
    public InputField IFRegisterPassword;
    public InputField IFRegisterEmail;
    public InputField IFRegisterConfirmPassword;
    public InputField IFRegisterConfirmEmail;

    public InputField IFLoginPassword;
    public InputField IFLoginEmail;

    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject gamePanel;
    public GameObject game;
    public GameObject launchGame;


    


    #endregion

    void Start()
    {
        dataManager = DataManager.dataManager;
        PlayerPrefs.DeleteAll();
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "7B285";

        }
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest
            {
                Email = dataManager.GetEmail(),
                Password = dataManager.GetPassword()
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
     

    }



    #region LOGIN/Register

    public void OnCancelButton()
    {

        loginPanel.SetActive(true);
        registerPanel.SetActive(false);

    }


    // BUTTON REGISTER
    public void OnRegisterButton()
    {

        loginPanel.SetActive(false);
        registerPanel.SetActive(true);

    }

    //MESSAGE D' ERREUR ET REUSSITE FUTUR EVENT POUR ACTIVER DES TEXTS 

    private void OnLoginSuccess(LoginResult log)
    {
        Debug.Log("Congratulations, tu es connecté via login");
        PlayerPrefs.SetString("EMAIL", dataManager.GetEmail());
        PlayerPrefs.SetString("PASSWORD", dataManager.GetPassword());
        dataManager.id = log.PlayFabId;
      
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = log.PlayFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
         result => dataManager.SetUsername(result.PlayerProfile.DisplayName), 
         error => Debug.LogError(error.GenerateErrorReport())) ;
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnGetStats, error => Debug.LogError(error.GenerateErrorReport()));

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
           result3 => { SetCurrencyAccount(result3); },
            error => { Debug.LogError(error.GenerateErrorReport()); });

    }

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








    private void OnLoginFailure(PlayFabError error)
    {
        errorMessage.text = "ERROR : Identifiant incorrect !";
        errorMessage.gameObject.SetActive(true);
        errorMessage.gameObject.transform.SetParent(loginPanel.transform);
        errorMessage.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        Debug.Log("Je ne te connais pas");
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {

        Debug.Log("Congratulations, tu es connecté via register");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        PlayerPrefs.SetString("USERNAME", username);
        loginPanel.SetActive(false);

        registerPanel.SetActive(false);
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = dataManager.GetUsername() }, OnDisplayName, OnLoginAndroidFailure);
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
        Statistics = new List<StatisticUpdate> {
        new StatisticUpdate { StatisticName = "PlayerLevel", Value = dataManager.GetPlayerLevel() }
         }
        },
       result2 => { Debug.Log("User statistics updated"); },
       error => { Debug.LogError(error.GenerateErrorReport()); });
        //  Debug.Log(dataManager.GetEmail());
        //  AddOrUpdateContactEmail(dataManager.GetEmail());

        // ADD BIENVENUE FRAGMENT
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "RF",
            Amount = 50
        },
        result3 => { RequestVirtualCurrency();
        },error => { Debug.LogError(error.GenerateErrorReport()); });


    }


    private void RequestVirtualCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
           result3 => { SetCurrencyAccount(result3); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }



    private void SetCurrencyAccount(GetUserInventoryResult result)
    {
        dataManager.SetRuneFragment(result.VirtualCurrency["RF"]);
        dataManager.SetIRLMonney(result.VirtualCurrency["MI"]);
        gamePanel.SetActive(true);
        Debug.Log("Monnaie Load");
        Debug.Log(dataManager.GetRuneFragment());
        Debug.Log(dataManager.GetIRLMonney());
    }



    private void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + "is your new display name");
    }
       
   

    private void OnRegisterFailure(PlayFabError error)
    {

        Debug.Log(error.HttpCode);
        Debug.Log(error.GenerateErrorReport());
        if (error.HttpCode == 400)
        {
            errorMessage.gameObject.transform.SetParent(registerPanel.transform);
            errorMessage.text = "ERROR : L'adresse email est déja utilisé ou invalide !";
            errorMessage.gameObject.SetActive(true);
            errorMessage.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        };

    }


    private void OnLoginAndroidFailure(PlayFabError error)
    {
        errorMessage.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        errorMessage.gameObject.transform.SetParent(registerPanel.transform);
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = "ERROR : Identifiant incorrect !";
        Debug.Log(error.GenerateErrorReport());

    }

    private void OnLoginAndroidSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, j'ai réussi mon premier appel");
        loginPanel.SetActive(false);
    }

    //REQUETE DE CONNEXION

    public void OnClickLogin()
    {

        dataManager.SetEmail(IFLoginEmail.text);
        dataManager.SetPassword(IFLoginPassword.text);

        var request = new LoginWithEmailAddressRequest
        {
            Email = dataManager.GetEmail(),
            Password = dataManager.GetPassword()
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

    }


    public void OnClickRegister()
    {

        dataManager.SetEmail(IFRegisterEmail.text);
        dataManager.SetPassword(IFRegisterPassword.text);
        dataManager.SetUsername(IFRegisterUsername.text);
        Debug.Log(dataManager.GetUsername());
        userConfirmEmail = IFRegisterConfirmEmail.text;
        userConfirmPassword = IFRegisterConfirmPassword.text;

        if (dataManager.GetEmail().Equals(userConfirmEmail))
        {
            if (dataManager.GetPassword().Equals(userConfirmPassword))
            {

                errorMessage.gameObject.SetActive(false);
                var registerRequest = new RegisterPlayFabUserRequest();
                registerRequest.Email = dataManager.GetEmail();
                registerRequest.Password = dataManager.GetPassword();
                registerRequest.Username = dataManager.GetUsername();

                PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);


            }
            else
            {
                errorMessage.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
                errorMessage.gameObject.transform.SetParent(registerPanel.transform);
                errorMessage.text = "ERROR : Les deux mots de passes sont différents !";
                errorMessage.gameObject.SetActive(true);

            }
        }
        else
        {
            errorMessage.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            errorMessage.gameObject.transform.SetParent(registerPanel.transform);
            errorMessage.text = "ERROR : Les deux adresses mails sont différents !";
            errorMessage.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame

    


    void AddOrUpdateContactEmail(string emailAddress)
    {
        var request = new AddOrUpdateContactEmailRequest
        {
            EmailAddress = emailAddress
        };
        PlayFabClientAPI.AddOrUpdateContactEmail(request, result =>
        {
            Debug.Log("The player's account has been updated with a contact email");
        }, FailureCallback);
    }

    void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    #endregion



    #region Player_Stats


   




    #endregion


}
