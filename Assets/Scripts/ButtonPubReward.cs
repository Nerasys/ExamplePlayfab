using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using PlayFab;
using PlayFab.ClientModels;

[RequireComponent(typeof(Button))]
public class ButtonPubReward : MonoBehaviour, IUnityAdsListener
{

#if UNITY_IOS
    private string gameId = "3461175";
#elif UNITY_ANDROID
    private string gameId = "3461174";
#endif

    Button myButton;
    public string myPlacementId = "R1";
    DataManager dataManager;
    void Start()
    {
        myButton = GetComponent<Button>();
        dataManager = DataManager.dataManager;
        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = Advertisement.IsReady(myPlacementId);

        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, true);
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        Advertisement.Show(myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId)
        {
            myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId,ShowResult result)
    {
        // Define conditional logic for each ad completion status:
        if (result == ShowResult.Finished)
        {
            PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = "RF",
                Amount = 100
            },
       result3 => {
           RequestVirtualCurrency();
       }, error => { Debug.LogError(error.GenerateErrorReport()); });
            Debug.LogWarning("Réussi");
            // Reward the user for watching the ad to completion.
        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Skip");
            // Do not reward the user for skipping the ad.
        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
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
    Debug.Log(dataManager.GetRuneFragment());
    Debug.Log(dataManager.GetIRLMonney());
}
}
