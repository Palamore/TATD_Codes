using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShopConfirm : MonoBehaviour
{
    private static UpgradeShopConfirm instance;
    GoogleServiceManager GSM;
    public static UpgradeShopConfirm Instance()
    {

        if (instance == null)
        {
            instance = FindObjectOfType<UpgradeShopConfirm>();
        }
        return instance;
    }
    public int LevelPoint;
    public float[] DmgRate = new float[8];
    public float[] AtkSpdRate = new float[8];
    private int[] DmgRateHash = new int[8];
    private int[] AtkSpdRateHash = new int[8];
    public int Data_Loaded;
    public bool is_Saving;
    public bool is_Loading;
    private GameObject Load_Cover_Now;
    private GameObject Save_Cover_Now;
    private int DmgRate_Data1_Container;
    private int DmgRate_Data2_Container;
    private int AtkSpdRate_Data1_Container;
    private int AtkSpdRate_Data2_Container;

    public int[] Get_DmgRate_Data()
    {
        return DmgRateHash;
    }
    public int[] Get_AtkSpdRate_Data()
    {
        return AtkSpdRateHash;
    }
    public int Get_LevelPoint()
    {
        return LevelPoint;
    }

    void Awake()
    {
        GSM = GoogleServiceManager.Instance();
        LevelPoint = 0;
        for(int i = 0; i < 8; i++)
        {
            DmgRate[i] = 1.0f;
            AtkSpdRate[i] = 1.0f;
            DmgRateHash[i] = 0;
            AtkSpdRateHash[i] = 0;
        }
    }



    public void LoadData()
    {
        DmgRate_Data1_Container = 0;
        DmgRate_Data2_Container = 0;
        AtkSpdRate_Data1_Container = 0;
        AtkSpdRate_Data2_Container = 0;

        StartCoroutine(DataLoading());
        
    }

    IEnumerator DataLoading()
    {
        Load_Cover_Now = Instantiate(GSM.LoadCoverUI, GSM.Tooltip_Parent.transform.position, Quaternion.identity);
        Load_Cover_Now.transform.SetParent(GSM.Tooltip_Parent.transform);

        GSM.LoadFromCloud(0);
        GSM.onLoading = true;
        while (GSM.onLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        LevelPoint = int.Parse(GSM.LoadedData);

        GSM.LoadFromCloud(1);
        GSM.onLoading = true;
        while (GSM.onLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        DmgRate_Data1_Container = int.Parse(GSM.LoadedData);
        DataProcessing(DmgRate_Data1_Container, 1);

        GSM.LoadFromCloud(2);
        GSM.onLoading = true;
        while (GSM.onLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        DmgRate_Data2_Container = int.Parse(GSM.LoadedData);
        DataProcessing(DmgRate_Data2_Container, 2);

        GSM.LoadFromCloud(3);
        GSM.onLoading = true;
        while (GSM.onLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        AtkSpdRate_Data1_Container = int.Parse(GSM.LoadedData);
        DataProcessing(AtkSpdRate_Data1_Container, 3);

        GSM.LoadFromCloud(4);
        GSM.onLoading = true;
        while (GSM.onLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        AtkSpdRate_Data2_Container = int.Parse(GSM.LoadedData);
        DataProcessing(AtkSpdRate_Data2_Container, 4); 
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
                DmgRateHash[0] = temp;
                break;
            case 2:
                DmgRateHash[4] = temp;
                break;
            case 3:
                AtkSpdRateHash[0] = temp;
                break;
            case 4:
                AtkSpdRateHash[4] = temp;
                break;
        }
        temp = temp2 / 10000; // 34
        temp2 = temp2 % 10000; // 5678
        switch (_dataType)
        {
            case 1:
                DmgRateHash[1] = temp;
                break;
            case 2:
                DmgRateHash[5] = temp;
                break;
            case 3:
                AtkSpdRateHash[1] = temp;
                break;
            case 4:
                AtkSpdRateHash[5] = temp;
                break;
        }
        temp = temp2 / 100; // 56
        temp2 = temp2 % 100; // 78
        switch (_dataType)
        {
            case 1:
                DmgRateHash[2] = temp;
                break;
            case 2:
                DmgRateHash[6] = temp;
                break;
            case 3:
                AtkSpdRateHash[2] = temp;
                break;
            case 4:
                AtkSpdRateHash[6] = temp;
                break;
        }
        temp = temp2;
        switch (_dataType)
        {
            case 1:
                DmgRateHash[3] = temp;
                break;
            case 2:
                DmgRateHash[7] = temp;
                break;
            case 3:
                AtkSpdRateHash[3] = temp;
                break;
            case 4:
                AtkSpdRateHash[7] = temp;
                
                BuffConfirm();
                Destroy(Load_Cover_Now);
                GSM.Make_Dialouge("데이터 로드완료.");
                is_Loading = false;
                SoundManager.Instance().Acknowledge.Play();
                break;
        }
    }

    private void BuffConfirm()
    {
        float b = 100.0f;
        for (int i = 0; i < 8; i++)
        {
            DmgRate[i] = (float)DmgRateHash[i] / b + 1.0f;
            AtkSpdRate[i] = (float)AtkSpdRateHash[i] / b + 1.0f;
        }
    }

    public void GetDataFromShop(int _LevelPoint, int[] _DmgRateHash, int[] _AtkSpdRateHash)
    {
        LevelPoint = _LevelPoint;
        DmgRateHash = _DmgRateHash;
        AtkSpdRateHash = _AtkSpdRateHash;


        //가공한 데이터 실제버프변수에 적용
        BuffConfirm();


        //가공한 데이터 저장
        Save_Data();
    }
    public void Save_Data()
    {
        //세이브 전처리
        DmgRate_Data1_Container = 0;
        DmgRate_Data2_Container = 0;
        AtkSpdRate_Data1_Container = 0;
        AtkSpdRate_Data2_Container = 0;

        DmgRate_Data1_Container += (DmgRateHash[0] * 1000000) + (DmgRateHash[1] * 10000)
            + (DmgRateHash[2] * 100) + DmgRateHash[3];
        DmgRate_Data2_Container += (DmgRateHash[4] * 1000000) + (DmgRateHash[5] * 10000)
            + (DmgRateHash[6] * 100) + DmgRateHash[7];
        AtkSpdRate_Data1_Container += (AtkSpdRateHash[0] * 1000000) + (AtkSpdRateHash[1] * 10000)
            + (AtkSpdRateHash[2] * 100) + AtkSpdRateHash[3];
        AtkSpdRate_Data2_Container += (AtkSpdRateHash[4] * 1000000) + (AtkSpdRateHash[5] * 10000)
            + (AtkSpdRateHash[6] * 100) + AtkSpdRateHash[7];


        //     GSM.Make_Dialouge("It's Saving");
        Save_Cover_Now = Instantiate(GSM.SaveCoverUI, GSM.Tooltip_Parent.transform.position, Quaternion.identity);
        Save_Cover_Now.transform.SetParent(GSM.Tooltip_Parent.transform);
        StartCoroutine(Status_Confirm());
        
    }


    IEnumerator Status_Confirm()
    {
        //

  //      GSM.Make_Dialouge("I'm gonna save" + LevelPoint.ToString());
        GSM.SaveToCloud(LevelPoint.ToString(), 0);
  //      GSM.Make_Dialouge("This Works 22");
        GSM.onSaving = true;
        while (GSM.onSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }


        GSM.SaveToCloud(DmgRate_Data1_Container.ToString(), 1);
        GSM.onSaving = true;
        while (GSM.onSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }


        GSM.SaveToCloud(DmgRate_Data2_Container.ToString(), 2);
        GSM.onSaving = true;
        while (GSM.onSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GSM.SaveToCloud(AtkSpdRate_Data1_Container.ToString(), 3);
        GSM.onSaving = true;
        while (GSM.onSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GSM.SaveToCloud(AtkSpdRate_Data2_Container.ToString(), 4);
        GSM.onSaving = true;
        while (GSM.onSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(Save_Cover_Now);
        GSM.Make_Dialouge("데이터 저장완료.");
        SoundManager.Instance().Acknowledge.Play();
        is_Saving = false;

    }
}
