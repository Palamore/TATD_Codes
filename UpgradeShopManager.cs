using System.Collections;
using System.Collections.Generic;
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

    public bool bIsSaving; // GSM에서 참조
    public bool bIsLoading;
    public int lvPoint; //게임 클리어 후 참조

    private float[] dmgRate = new float[8];
    private float[] atkSpdRate = new float[8];
    private int[] dmgRateData = new int[8];
    private int[] atkspdRateData = new int[8];
    private GameObject CurLoadCover;
    private GameObject CurSaveCover;
    private int dmgRateDataContainer1;
    private int dmgRateDataContainer2;
    private int atkSpdRateDataContainer1;
    private int atkSpdRateDataContainer2;

    public int[] GetDmgRateData()
    {
        return dmgRateData;
    }
    public int[] GetAtkSpdRateData()
    {
        return atkspdRateData;
    }
    public int GetLevelPoint()
    {
        return lvPoint;
    }

    void Awake()
    {
        GSM = GoogleServiceManager.Instance();
        lvPoint = 0;
        for(int i = 0; i < 8; i++)
        {
            dmgRate[i] = 1.0f;
            atkSpdRate[i] = 1.0f;
            dmgRateData[i] = 0;
            atkspdRateData[i] = 0;
        }
    }



    public void LoadConfirm()
    {
        dmgRateDataContainer1 = 0;
        dmgRateDataContainer2 = 0;
        atkSpdRateDataContainer1 = 0;
        atkSpdRateDataContainer2 = 0;

        CurLoadCover = Instantiate(GSM.LoadCoverUI, GSM.UICanvas.transform.position, Quaternion.identity);
        CurLoadCover.transform.SetParent(GSM.UICanvas.transform);
        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        GSM.LoadFromCloud(0);
        GSM.bIsLoading = true;
        while (GSM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        lvPoint = int.Parse(GSM.loadData);

        GSM.LoadFromCloud(1);
        GSM.bIsLoading = true;
        while (GSM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        dmgRateDataContainer1 = int.Parse(GSM.loadData);
        DataProcessing(dmgRateDataContainer1, 1);

        GSM.LoadFromCloud(2);
        GSM.bIsLoading = true;
        while (GSM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        dmgRateDataContainer2 = int.Parse(GSM.loadData);
        DataProcessing(dmgRateDataContainer2, 2);

        GSM.LoadFromCloud(3);
        GSM.bIsLoading = true;
        while (GSM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        atkSpdRateDataContainer1 = int.Parse(GSM.loadData);
        DataProcessing(atkSpdRateDataContainer1, 3);

        GSM.LoadFromCloud(4);
        GSM.bIsLoading = true;
        while (GSM.bIsLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        atkSpdRateDataContainer2 = int.Parse(GSM.loadData);
        DataProcessing(atkSpdRateDataContainer2, 4); 
    }

    void DataProcessing(int _data, int _dataType)
    {
        int temp;
        int temp2;
        temp = _data / 1000000; // 12
        temp2 = _data % 1000000; // 345678
        switch (_dataType)
        {
            case 1:
                dmgRateData[0] = temp;
                break;
            case 2:
                dmgRateData[4] = temp;
                break;
            case 3:
                atkspdRateData[0] = temp;
                break;
            case 4:
                atkspdRateData[4] = temp;
                break;
        }
        temp = temp2 / 10000; // 34
        temp2 = temp2 % 10000; // 5678
        switch (_dataType)
        {
            case 1:
                dmgRateData[1] = temp;
                break;
            case 2:
                dmgRateData[5] = temp;
                break;
            case 3:
                atkspdRateData[1] = temp;
                break;
            case 4:
                atkspdRateData[5] = temp;
                break;
        }
        temp = temp2 / 100; // 56
        temp2 = temp2 % 100; // 78
        switch (_dataType)
        {
            case 1:
                dmgRateData[2] = temp;
                break;
            case 2:
                dmgRateData[6] = temp;
                break;
            case 3:
                atkspdRateData[2] = temp;
                break;
            case 4:
                atkspdRateData[6] = temp;
                break;
        }
        temp = temp2; // 78
        switch (_dataType)
        {
            case 1:
                dmgRateData[3] = temp;
                break;
            case 2:
                dmgRateData[7] = temp;
                break;
            case 3:
                atkspdRateData[3] = temp;
                break;
            case 4:
                atkspdRateData[7] = temp;
                
                BuffConfirm();
                Destroy(CurLoadCover);
                GSM.MakeDialogue("데이터 로드완료.");
                bIsLoading = false;
                SoundManager.Instance().Acknowledge.Play();
                break;
        }
    }

    private void BuffConfirm()
    {
        float b = 100.0f;
        for (int i = 0; i < 8; i++)
        {
            dmgRate[i] = (float)dmgRateData[i] / b + 1.0f;
            atkSpdRate[i] = (float)atkspdRateData[i] / b + 1.0f;
        }
    }

    public void GetDataFromShop(int _lvPoint, int[] _dmgRateData, int[] _atdspdRateData)
    {
        lvPoint = _lvPoint;
        dmgRateData = _dmgRateData;
        atkspdRateData = _atdspdRateData;
        //가공한 데이터 실제버프변수에 적용
        BuffConfirm();

        //가공한 데이터 저장
        SaveConfirm();
    }
    public void SaveConfirm()
    {
        dmgRateDataContainer1 = 0;
        dmgRateDataContainer2 = 0;
        atkSpdRateDataContainer1 = 0;
        atkSpdRateDataContainer2 = 0;
        //세이브 데이터 가공
        dmgRateDataContainer1 += (dmgRateData[0] * 1000000) + (dmgRateData[1] * 10000)
            + (dmgRateData[2] * 100) + dmgRateData[3];
        dmgRateDataContainer2 += (dmgRateData[4] * 1000000) + (dmgRateData[5] * 10000)
            + (dmgRateData[6] * 100) + dmgRateData[7];
        atkSpdRateDataContainer1 += (atkspdRateData[0] * 1000000) + (atkspdRateData[1] * 10000)
            + (atkspdRateData[2] * 100) + atkspdRateData[3];
        atkSpdRateDataContainer2 += (atkspdRateData[4] * 1000000) + (atkspdRateData[5] * 10000)
            + (atkspdRateData[6] * 100) + atkspdRateData[7];

        CurSaveCover = Instantiate(GSM.SaveCoverUI, GSM.UICanvas.transform.position, Quaternion.identity);
        CurSaveCover.transform.SetParent(GSM.UICanvas.transform);
        StartCoroutine(SaveData());
        
    }


    IEnumerator SaveData()
    {
        GSM.SaveToCloud(lvPoint.ToString(), 0);
        GSM.bIsSaving = true;
        while (GSM.bIsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }


        GSM.SaveToCloud(dmgRateDataContainer1.ToString(), 1);
        GSM.bIsSaving = true;
        while (GSM.bIsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }


        GSM.SaveToCloud(dmgRateDataContainer2.ToString(), 2);
        GSM.bIsSaving = true;
        while (GSM.bIsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GSM.SaveToCloud(atkSpdRateDataContainer1.ToString(), 3);
        GSM.bIsSaving = true;
        while (GSM.bIsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GSM.SaveToCloud(atkSpdRateDataContainer2.ToString(), 4);
        GSM.bIsSaving = true;
        while (GSM.bIsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(CurSaveCover);
        GSM.MakeDialogue("데이터 저장완료.");
        SoundManager.Instance().Acknowledge.Play();
        bIsSaving = false;

    }
}
