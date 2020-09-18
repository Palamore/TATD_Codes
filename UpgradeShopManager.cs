using System.Collections;
using UnityEngine;

public class UpgradeShopManager : MonoBehaviour
{
    private static UpgradeShopManager instance;
    GoogleServiceManager GSM;
    public static UpgradeShopManager Instance()
    {

        if (instance == null)
        {
            instance = FindObjectOfType<UpgradeShopManager>();
        }
        return instance;
    }
    public int levelPoint;
    public float[] dmgRate = new float[8];
    public float[] atkSpdRate = new float[8];
    public int[] dmgRateInt = new int[8];
    public int[] atkSpdRateInt = new int[8];
    private string[] dataContainer = new string[16];
    public string saveData;
    public int loadData;
    public bool bIsSaving;
    public bool bIsLoading;
    private GameObject CurLoadCover;
    private GameObject CurSaveCover;


    public int[] GetDmgData()
    {
        return dmgRateInt;
    }
    public int[] GetAtkSpdData()
    {
        return atkSpdRateInt;
    }
    public int GetLevelPoint()
    {
        return levelPoint;
    }

    void Awake()
    {
        saveData = "000000000000000000000000000000000";
        GSM = GoogleServiceManager.Instance();
        levelPoint = 0;
        for(int i = 0; i < 8; i++)
        {
            dmgRate[i] = 1.0f;
            atkSpdRate[i] = 1.0f;
            dmgRateInt[i] = 0;
            atkSpdRateInt[i] = 0;
        }
    }

    public void LoadConfirm()
    {
        CurLoadCover = Instantiate(GSM.LoadCoverUI, GSM.Tooltip_Parent.transform.position, Quaternion.identity);
        CurLoadCover.transform.SetParent(GSM.Tooltip_Parent.transform);
        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        GSM.LoadFromCloud();
        GSM.bIsLoading = true;
        while (GSM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        string tmpData = GSM.loadData;
        DataProcessing(tmpData);
    }

    void DataProcessing(string _data)
    {
        int k = 0;
        string tmp;
        for (int i = 0; i < 8; i++)
        {
            tmp = "" + _data[k] + _data[k + 1];
            dmgRateInt[i] = int.Parse(tmp);
            k += 2;
        }
        for (int i = 0; i < 8; i++)
        {
            tmp = "" + _data[k] + _data[k + 1];
            atkSpdRateInt[i] = int.Parse(tmp);
            k += 2;
        }
        tmp = _data.Remove(0, 32);

        levelPoint = int.Parse(tmp);

        BuffConfirm();
        Destroy(CurLoadCover);
        bIsLoading = false;
        SoundManager.Instance().Acknowledge.Play();
    }

    private void BuffConfirm()
    {
        float b = 100.0f;
        for (int i = 0; i < 8; i++)
        {
            dmgRate[i] = (float)dmgRateInt[i] / b + 1.0f;
            atkSpdRate[i] = (float)atkSpdRateInt[i] / b + 1.0f;
        }
    }

    public void GetDataFromShop(int _LevelPoint, int[] _DmgRateInt, int[] _AtkSpdRateInt)
    {
        levelPoint = _LevelPoint;
        dmgRateInt = _DmgRateInt;
        atkSpdRateInt = _AtkSpdRateInt;

        for(int i = 0; i < 8; i++)
        {
            if(dmgRateInt[i] < 10)
            dataContainer[i] = "0" + dmgRateInt[i].ToString();
            else
            dataContainer[i] = dmgRateInt[i].ToString();
        }
        for(int i = 8; i < 16; i++)
        {
            if(atkSpdRateInt[i-8] < 10)
            dataContainer[i] = "0" + atkSpdRateInt[i - 8].ToString();
            else
            dataContainer[i] = atkSpdRateInt[i - 8].ToString();
        }
        saveData = dataContainer[0] + dataContainer[1] + dataContainer[2]
            + dataContainer[3] + dataContainer[4] + dataContainer[5]
            + dataContainer[6] + dataContainer[7] + dataContainer[8]
            + dataContainer[9] + dataContainer[10] + dataContainer[11]
            + dataContainer[12] + dataContainer[11] + dataContainer[14]
            + dataContainer[15] + levelPoint.ToString();
        

        BuffConfirm();

        SaveConfirm();
    }
    public void SaveConfirm()
    {
 //       GSM.MakeDialogue("Start \nSave Coroutine");
        CurSaveCover = Instantiate(GSM.SaveCoverUI, GSM.Tooltip_Parent.transform.position, Quaternion.identity);
        CurSaveCover.transform.SetParent(GSM.Tooltip_Parent.transform);
        StartCoroutine(SaveData());
    }


    IEnumerator SaveData()
    {
        GSM.SaveToCloud(saveData);
        GSM.bIsSaving = true;
        while (GSM.bIsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(CurSaveCover);
        SoundManager.Instance().Acknowledge.Play();
        bIsSaving = false;

    }
}
