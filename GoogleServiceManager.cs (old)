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
    private UpgradeShopManager USM;

    public static GoogleServiceManager Instance()
    {

        if (instance == null)
        {
            instance = FindObjectOfType<GoogleServiceManager>();
            if (instance == null)
            {
                instance = new GameObject("GooglerServiceManger").AddComponent<GoogleServiceManager>();
                instance.gameObject.AddComponent<UpgradeShopManager>();
                instance.gameObject.AddComponent<AdmobVideoScript>();
            }
        }
        return instance;
    }


    private bool bIsGuest;
    public bool bIsSaving; // USM에서 참조
    public bool bIsLoading;
    private string saveData;
    public string loadData; // USM에서 참조
    private GameObject CurDialogue;

    //From Asset
    public GameObject Dialogue;
    public GameObject LoadCoverUI;
    public GameObject SaveCoverUI;
    //Form Scene
    private GameObject UpgradeShopBtn;
    private GameObject StartBtn;
    private GameObject LoginToolTip;
    public GameObject UICanvas; // Managers에서 참조
    private GameObject AchievementBtn;
    private GameObject LeaderboardBtn;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        bIsGuest = false;
        InitiatePlayGames();
    }

    public void MakeDialogue(string str)
    {
        if (CurDialogue != null)
        {
            Destroy(CurDialogue);
        }
        CurDialogue = Instantiate(Dialogue, UICanvas.transform.position, Quaternion.identity);
        CurDialogue.transform.SetParent(UICanvas.transform);
        CurDialogue.GetComponent<AlertDialogue>().Set_Contents(str);

    }

    public void TitleSceneInit(GameObject[] Initiate)
    {
        Dialogue = Initiate[0];
        LoadCoverUI = Initiate[1];
        SaveCoverUI = Initiate[2];
        StartBtn = Initiate[3];
        LoginToolTip = Initiate[4];
        UICanvas = Initiate[5];
        UpgradeShopBtn = Initiate[6];
        AchievementBtn = Initiate[7];
        LeaderboardBtn = Initiate[8];
        USM = UpgradeShopManager.Instance();
        if (CheckLogin()) // 메인 게임 종료 후 타이틀 씬으로 돌아왔을 경우 세션 체크
        {
            StartCoroutine(GetInitialData());
        }
    }


    public void SceneInit(GameObject TTP)
    {
        UICanvas = TTP;
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
        StartCoroutine(GetInitialData());

        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                MakeDialogue("로그인에 실패했습니다.");
                StopCoroutine(GetInitialData());
                LoginToolTip.SetActive(true);
                return;
            }
            else
            {
                MakeDialogue("안녕하세요. " + Social.localUser.userName + ".\n데이터를 로드합니다.");
            }
        });
    }
    IEnumerator GetInitialData()
    {
        while (CheckLogin() == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        bIsLoading = true;
        LoadFromCloud(0);
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
    public void SaveToCloud(string _data, int _dataType)
    {
        StartCoroutine(Save(_data, _dataType));
    }

    IEnumerator Save(string _data, int _dataType)
    {
        while (CheckLogin() == false)
        {
            Login();
            yield return new WaitForSeconds(2.0f);
        }
        bIsSaving = true;
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
        if (_saved == true) // Save 목적
        {
            savedClient.OpenWithAutomaticConflictResolution(_fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave);
        }
        else                // Load 목적
        {
            savedClient.OpenWithAutomaticConflictResolution(_fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead);
        }
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
            MakeDialogue("Game Save Failed");
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
            MakeDialogue("Save Fail");
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
        bIsLoading = true;
        while (CheckLogin() == false)
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
        if (_status == SavedGameRequestStatus.Success)
        {
            LoadGameData(_data);
        }
        else
        {
            bIsLoading = false;
            MakeDialogue("데이터 로드에 실패했습니다.");
        }
    }

    void LoadGameData(ISavedGameMetadata _data)
    {
        ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
        savedClient.ReadBinaryData(_data, OnSavedGameDataRead);
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
            MakeDialogue("데이터 로드에 실패했습니다.");
        }
        bIsLoading = false;
    }

    #endregion

}
