using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Firebase;
using Firebase.Analytics;
using UnityEngine.SceneManagement;
public class GoogleServiceManager : MonoBehaviour
{
    private static GoogleServiceManager instance;
    public UpgradeShopConfirm USC;

    public static GoogleServiceManager Instance()
    {

            if(instance == null)
            {
                instance = FindObjectOfType<GoogleServiceManager>();
                if(instance == null)
                {
                    instance = new GameObject("GooglerServiceManger").AddComponent<GoogleServiceManager>();
                instance.gameObject.AddComponent<UpgradeShopConfirm>();
                instance.gameObject.AddComponent<AdmobVideoScript>();
                }
            }
        return instance;
    }


    public bool is_Guest;
    public bool onSaving;
    public bool onLoading;
    public string saveData;
    public string LoadedData;
    private GameObject Dialogue_Now;

    public void Make_Dialouge(string st)
    {
        if(Dialogue_Now != null)
        {
            Destroy(Dialogue_Now);
        }
        Dialogue_Now = Instantiate(FailDialogue, Tooltip_Parent.transform.position, Quaternion.identity);
        Dialogue_Now.transform.SetParent(Tooltip_Parent.transform);
        temp_st = st;
        Dialogue_Now.GetComponent<AlertDialogue>().Set_Contents(temp_st);
        
    }


    public int Level_Point = 0;
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
    

    string temp_st;



    void Start()
    {
        InitiatePlayGames();
        FirebaseInit();
    }


    private void FirebaseInit()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
       {
           FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
       });
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        is_Guest = false;

    }
    public void First_Init(GameObject[] Initiate)
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
        USC = UpgradeShopConfirm.Instance();

        if (Check_Login())
        {
            StartCoroutine(Google_Init());
        }
        else
        {

        }
    }


    public void Init(GameObject TTP)
    {
        Tooltip_Parent = TTP;

        if (is_Guest == false)
        {
            if (!Check_Login())
            {
                Make_Dialouge("세션이 끊겼습니다.");

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
        StartCoroutine(Google_Init());

        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                Make_Dialouge("로그인에 실패했습니다.");
                StopCoroutine(Google_Init());
                LoginToolTip.SetActive(true);
                return;
            }
            else
            {
                Make_Dialouge("안녕하세요. " + Social.localUser.userName + ".\n데이터를 로드합니다.");
            }
        });
    }
    IEnumerator Google_Init()
    {
        while (Check_Login() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        onLoading = true;
        LoadFromCloud(0);
        while (onLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (LoadedData == "")
        {
            Make_Dialouge("초기 데이터를 설정합니다.");
            USC.is_Saving = true;
            USC.Save_Data();
            while (USC.is_Saving)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        USC.is_Loading = true;
        USC.LoadData();
        while (USC.is_Loading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        AchievementBtn.SetActive(true);
        LeaderboardBtn.SetActive(true);
        UpgradeShopBtn.SetActive(true);
        StartBtn.SetActive(true);
        LoginToolTip.SetActive(false);
    }


    public bool Check_Login()
    {
        return Social.localUser.authenticated;
    }

    public void Guest_Login()
    {
        StartBtn.SetActive(true);
        LoginToolTip.SetActive(false);
        is_Guest = true;
    }

    #region Save
    public void SaveToCloud(string _data,int _dataType)
    {
        StartCoroutine(Save(_data, _dataType));
    }

    IEnumerator Save(string _data , int _dataType)
    {
        while(Check_Login() == false)
        {
            Login();
            yield return new WaitForSeconds(2.0f);
        }
        onSaving = true;
        string id = Social.localUser.id;
        string fileName = string.Empty;
        if (_dataType == 0)
            fileName = string.Format("LevelPoint", id);
        else if (_dataType == 1)
            fileName = string.Format("DmgPoint1", id);
        else if (_dataType == 2)
            fileName = string.Format("DmgPoint2", id);
        else if (_dataType == 3)
            fileName = string.Format("AtkSpdPoint1", id);
        else if (_dataType == 4)
            fileName = string.Format("AtkSpdPoint2", id);

        saveData = _data;

        OpenSavedGame(fileName, true);
    }
    void OpenSavedGame(string _fileName, bool _saved)
    {
        ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
        if (_saved == true)
        {
            savedClient.OpenWithAutomaticConflictResolution(_fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave);
        }
        else
        {
            savedClient.OpenWithAutomaticConflictResolution(_fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead);
        }
    }

    void OnSavedGameOpenedToSave(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    {
        if(_status == SavedGameRequestStatus.Success)
        {
            byte[] b = Encoding.UTF8.GetBytes(string.Format(saveData));
            SaveGame(_data, b, DateTime.Now.TimeOfDay);
        }
        else
        {
            Make_Dialouge("Game Save Failed");
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
        onSaving = false;
        if(_status == SavedGameRequestStatus.Success)
        {

        }
        else
        {
            Make_Dialouge("Save Fail");
        }
    }



    #endregion


    #region Load

    public void LoadFromCloud(int _dataType)
    {
        StartCoroutine(Load(_dataType));
    }
    IEnumerator Load(int _dataType)
    {
        onLoading = true;
        while(Check_Login() == false)
        {
            Login();
            yield return new WaitForSeconds(1.0f);
        }
        string id = Social.localUser.id;
        string fileName = string.Empty;
        if (_dataType == 0)
            fileName = string.Format("LevelPoint", id);
        else if (_dataType == 1)
            fileName = string.Format("DmgPoint1", id);
        else if (_dataType == 2)
            fileName = string.Format("DmgPoint2", id);
        else if (_dataType == 3)
            fileName = string.Format("AtkSpdPoint1", id);
        else if (_dataType == 4)
            fileName = string.Format("AtkSpdPoint2", id);

        OpenSavedGame(fileName, false);
    }

    void OnSavedGameOpenedToRead(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    {
        if(_status == SavedGameRequestStatus.Success)
        {
            LoadGameData(_data);
        }
        else
        {
            onLoading = false;
            Make_Dialouge("데이터 로드에 실패했습니다.");
        }
    }

    void LoadGameData(ISavedGameMetadata _data)
    {
        ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
        savedClient.ReadBinaryData(_data, OnSavedGameDataRead);
    }

    void OnSavedGameDataRead(SavedGameRequestStatus _status, byte[] _byte)
    {
        
        if(_status == SavedGameRequestStatus.Success)
        {
            string data = Encoding.Default.GetString(_byte);
            LoadedData = data;
        }
        else
        {
            onLoading = false;
            Make_Dialouge("데이터 로드에 실패했습니다.");
        }
        onLoading = false;
    }

    #endregion

}
