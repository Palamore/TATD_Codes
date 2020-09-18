using System;
using System.Collections;
using System.Text;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SceneManagement;
public class GoogleServiceManager : MonoBehaviour
{
    private static GoogleServiceManager instance;
    public UpgradeShopManager USM;

    public static GoogleServiceManager Instance()
    {

            if(instance == null)
            {
                instance = FindObjectOfType<GoogleServiceManager>();
                if(instance == null)
                {
                    instance = new GameObject("GooglerServiceManger").AddComponent<GoogleServiceManager>();
                instance.gameObject.AddComponent<UpgradeShopManager>();
                instance.gameObject.AddComponent<AdmobVideoScript>();
                }
            }
        return instance;
    }

    const string dataName = "TATDdata";
    public bool bIsGuest;
    public bool bIsSaving;
    public bool bIsLoading;
    public string saveData;
    public string loadData;
    private GameObject CurDialogue;


    public int levelPoint = 0;
    //From Asset
    public GameObject FailDialogue;
    public GameObject LoadCoverUI;
    public GameObject SaveCoverUI;
    //Form Scene
    public GameObject UpgradeShopBtn;
    public GameObject StartBtn;
    public GameObject LoginToolTip;
    public GameObject Tooltip_Parent;
    public GameObject AchievementBtn;
    public GameObject LeaderboardBtn;


    public void MakeDialogue(string str)
    {
        if (CurDialogue != null)
        {
            Destroy(CurDialogue);
        }
        CurDialogue = Instantiate(FailDialogue, Tooltip_Parent.transform.position, Quaternion.identity);
        CurDialogue.transform.SetParent(Tooltip_Parent.transform);
        CurDialogue.GetComponent<AlertDialogue>().Set_Contents(str);
    }

    void Start()
    {
        InitiatePlayGames();
        loadData = "";
        DontDestroyOnLoad(gameObject);
        bIsGuest = false;
        USM = UpgradeShopManager.Instance();
    }

    public void TitleSceneInit(GameObject[] Initiate)
    {
        FailDialogue = Initiate[0];
        LoadCoverUI = Initiate[1];
        SaveCoverUI = Initiate[2];
        StartBtn = Initiate[3];
        LoginToolTip = Initiate[4];
        Tooltip_Parent = Initiate[5];
        UpgradeShopBtn = Initiate[6];
        AchievementBtn = Initiate[7];
        LeaderboardBtn = Initiate[8];

        if (CheckLogin())
        {
            StartCoroutine(GoogleInit());
        }
        else
        {

        }
    }


    public void SceneInit(GameObject TTP)
    {
        Tooltip_Parent = TTP;

        if (bIsGuest == false)
        {
            if (!CheckLogin())
            {
                MakeDialogue("세션이 끊겼습니다.");

                Invoke("BacktoTitle", 2.0f);
            }
        }
    }

    void BacktoTitle()
    {
        SceneManager.LoadScene("Game_title_scene_copy");
    }



    private void InitiatePlayGames()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .EnableSavedGames()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();
    }

 


    public void Login()
    {
//        USM.SaveConfirm();

        StartCoroutine(GoogleInit());

        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                MakeDialogue("로그인에 실패했습니다.");
                StopCoroutine(GoogleInit());
                LoginToolTip.SetActive(true);
                return;
            }
            else
            {
                MakeDialogue("안녕하세요. " + Social.localUser.userName + ".\n데이터를 로드합니다.");
            }
        });
    }
    IEnumerator GoogleInit()
    {
        while (CheckLogin() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        bIsLoading = true;
        LoadFromCloud();
        while (bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (loadData == "")
        {
            MakeDialogue("초기 데이터를 설정합니다.");
            USM.bIsSaving = true;
            USM.SaveConfirm();
            while (USM.bIsSaving)
            {
//                MakeDialogue("Still\nWaiting2");
                yield return new WaitForSeconds(0.1f);
            }
        }
        USM.bIsLoading = true;
        USM.LoadConfirm();
        while (USM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        AchievementBtn.SetActive(true);
        LeaderboardBtn.SetActive(true);
        UpgradeShopBtn.SetActive(true);
        StartBtn.SetActive(true);
        LoginToolTip.SetActive(false);
    }


    public bool CheckLogin()
    {
        return Social.localUser.authenticated;
    }

    public void GuestLogin()
    {
        StartBtn.SetActive(true);
        LoginToolTip.SetActive(false);
        bIsGuest = true;
    }

    #region Save
    public void SaveToCloud(string _data)
    {
        saveData = _data;
        StartCoroutine(Save());
    }

    IEnumerator Save()
    {
        while (CheckLogin() == false)
        {
            Login();
            yield return new WaitForSeconds(0.1f);
        }
        bIsSaving = true;
        string id = Social.localUser.id;
        string fileName = string.Empty;
        fileName = string.Format(dataName, id);
        ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
        savedClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave);
    }

    void OnSavedGameOpenedToSave(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    {
        if (_status == SavedGameRequestStatus.Success)
        {
            byte[] b = Encoding.UTF8.GetBytes(string.Format(saveData));
            SaveGame(_data, b, DateTime.Now.TimeOfDay);
        }
        else
        {
            MakeDialogue("Save Failed\nTo Open");
        }
    }

    void SaveGame(ISavedGameMetadata _data, byte[] _byte, TimeSpan _playTime)
    {
        ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

        builder = builder.WithUpdatedPlayedTime(_playTime).WithUpdatedDescription("Saved at " + DateTime.Now);

        SavedGameMetadataUpdate updatedData = builder.Build();
        savedClient.CommitUpdate(_data, updatedData, _byte, OnSavedGameWritten);
    }

    void OnSavedGameWritten(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    {
        bIsSaving = false;
        if (_status == SavedGameRequestStatus.Success)
        {


        }
        else
        {
            MakeDialogue("Save Failed\nTo Request");
        }
    }



    #endregion


    #region Load

    public void LoadFromCloud()
    {
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        bIsLoading = true;
        while (CheckLogin() == false)
        {
            Login();
            yield return new WaitForSeconds(1.0f);
        }
        string id = Social.localUser.id;
        string fileName = string.Empty;
        fileName = string.Format(dataName, id);

        ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
        savedClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead);
    }

    void OnSavedGameOpenedToRead(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    {
        if (_status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
            savedClient.ReadBinaryData(_data, OnSavedGameDataRead);
        }
        else
        {
            bIsLoading = false;
            MakeDialogue("Load Failed\nTo Open");
        }
    }

    void OnSavedGameDataRead(SavedGameRequestStatus _status, byte[] _byte)
    {
        if (_status == SavedGameRequestStatus.Success)
        {
            string data = Encoding.Default.GetString(_byte);
            loadData = data;
        }
        else
        {
            bIsLoading = false;
            MakeDialogue("Load Failed\nTo Read");
        }
        bIsLoading = false;
    }

    #endregion

}
