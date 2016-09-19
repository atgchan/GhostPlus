using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class GoogleAdmobStarter : MonoBehaviour {

    public string zoneId;
    public int rewardQty = 5;

    GameManager gameManager = null;

    IEnumerator Start ()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        yield return new WaitForSeconds(2.0f);

        Debug.Log("---------------------- start admob-------");

        GoogleMobileAdsManager.instance.RequestSmartBanner();
    }

    public void RewardButtonClicked()
    {
        if (string.IsNullOrEmpty(zoneId)) zoneId = null;

        if (!Advertisement.IsReady(zoneId))
        {
            return;
        }

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(zoneId, options);
        //GoogleMobileAdsManager.instance.ShowRewardVideo();
    }

    public void HandleShowResult (ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                gameManager.AddDiamonds(rewardQty);
                Debug.Log("Video completed. User rewarded " + rewardQty + " credits.");
                break;
            case ShowResult.Skipped:
                Debug.LogWarning("Video was skipped.");
                break;
            case ShowResult.Failed:
                Debug.LogError("Video failed to show.");
                break;
        }
    }
}
