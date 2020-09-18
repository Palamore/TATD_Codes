using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretFactory : MonoBehaviour
{
    private static TurretFactory instance;
    public static TurretFactory Instance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<TurretFactory>();
        }
        return instance;
    }



    TowerCounter TC;
    SynergyManager SM;
    GoogleServiceManager GSM;
    SoundManager SDM;
    [Header("Turret Prefab Field")]
    public GameObject BaseTurret;
    public GameObject BuffTurret;
    public GameObject GrenadeTurret;
    public GameObject LaserTurret;
    public GameObject RailgunTurret;
    public GameObject MachinegunTurret;
    public GameObject TankTurret;
    public GameObject MissileTurret;

    int[] turretShopIndex = new int[8];

    public int slotCount;

    [Header("ShopUI Field")]
    public Text RerollCost;
    public Text UnlockCost;
    public Button[] ShopBtns = new Button[8];
    public Sprite[] StarSprites = new Sprite[5];
    public Image[] StarImages = new Image[8];
    public Sprite[] ShopSprites = new Sprite[13];
    //인덱스0~7 Turrets
    //인덱스8 TurretSlot BackGround
    //인덱스9 Lock with white circle
    //인덱스10 none
    //인덱스11 Sold Out
    //인덱스 12 LockedSlot BackGround
    public Image[] SlotImages = new Image[8];
    public Sprite[] SlotSprites = new Sprite[8];
    public Sprite SynergySlotSprite;
    public Image[] SynergyIconSlotImages1 = new Image[8];
    public Image[] SynergyIconImages1 = new Image[8];
    public Image[] SynergyIconSlotImages2 = new Image[8];
    public Image[] SynergyIconImages2 = new Image[8];

    [Header("Node Field")]
    private Node TargetNode;
    public GameObject buildEffect;

    void Start()
    {
        if (GameObject.FindObjectOfType<TutorialManager>() != null)
        {
            slotCount = 8;
            for (int i = 0; i < 8; i++)
            {
                turretShopIndex[i] = i;
            }
            PlayerStats.Money = 10000;
            UpdateSprites();
        }
    }
    public void InitTutorial(int turretIndex)
    {
        slotCount = 8;
        for (int i = 0; i < 8; i++)
        {
            this.turretShopIndex[i] = turretIndex;
        }
        UpdateSprites();
    }

    void Awake()
    {
        TC = TowerCounter.Instance();
        SM = SynergyManager.Instance();
        GSM = GoogleServiceManager.Instance();
        SDM = SoundManager.Instance();
        slotCount = 3;
        for(int i = slotCount; i < 8; i++)
        {
            turretShopIndex[i] = -1;
        }

        PlayerStats.Money += 50;
        RerollTurretShop();
        UnlockCost.text = "300";
        RerollCost.text = "50";
        

    }                       



    public void UpdateSprites()
    {
        UpdateStarSprites();
        for (int i = 0; i < slotCount; i++)
        {
            ShopBtns[i] = ShopBtns[i].GetComponent<Button>();
            ShopBtns[i].enabled = true;

            if (turretShopIndex[i] == -1)
            {
                ShopBtns[i].image.sprite = ShopSprites[10];
                SlotImages[i].sprite = ShopSprites[12];
                SynergyIconImages1[i].sprite = ShopSprites[10];
                SynergyIconImages2[i].sprite = ShopSprites[10];
                SynergyIconSlotImages1[i].sprite = ShopSprites[10];
                SynergyIconSlotImages2[i].sprite = ShopSprites[10];
            }
            else
            {
                if (!TC.bIsSoldOut[turretShopIndex[i]])
                {//해당 칸에 나타난 터렛이 다 팔리지 않았다면.
                    ShopBtns[i].image.sprite = ShopSprites[turretShopIndex[i]];
                    SlotImages[i].sprite = SlotSprites[turretShopIndex[i]];
                    SynergyIconImages1[i].sprite = SynergyIconTracker(turretShopIndex[i],1);
                    SynergyIconImages2[i].sprite = SynergyIconTracker(turretShopIndex[i],2);
                    SynergyIconSlotImages1[i].sprite = SynergySlotSprite;
                    SynergyIconSlotImages2[i].sprite = SynergySlotSprite;
                }
                else
                {
                    // Sold Out Field
                    ShopBtns[i].enabled = false;
                    ShopBtns[i].image.sprite = ShopSprites[10];
                    SlotImages[i].sprite = ShopSprites[11];
                    SynergyIconImages1[i].sprite = ShopSprites[10];
                    SynergyIconImages2[i].sprite = ShopSprites[10];
                    SynergyIconSlotImages2[i].sprite = ShopSprites[10];
                    SynergyIconSlotImages1[i].sprite = ShopSprites[10];
                }
            }
        }

       for (int i = slotCount; i < 8; i++)
        {
            ShopBtns[i] = ShopBtns[i].GetComponent<Button>();
            ShopBtns[i].image.sprite = ShopSprites[9];
            StarImages[i].sprite = ShopSprites[10];
            SlotImages[i].sprite = ShopSprites[12];
            SynergyIconImages1[i].sprite = ShopSprites[10];
            SynergyIconImages2[i].sprite = ShopSprites[10];
            SynergyIconSlotImages2[i].sprite = ShopSprites[10];
            SynergyIconSlotImages1[i].sprite = ShopSprites[10];
        }
    }
    
    private void UpdateStarSprites()
    {
        for(int i = 0; i < slotCount; i++)
        {
            //Whole_Tower_Counter의 0번인덱스부터
            //4번인덱스까지 최초로 false인 값이 있다면
            //그 false인 값의 인덱스가 다음에 지어질 터렛의 티어가 되는 구조.
            if (turretShopIndex[i] != -1)
            {
                if (!TC.bIsSoldOut[turretShopIndex[i]]) //해당 칸에 나타난 터렛이 다팔리지 않았다면.
                {
                    for (int j = 0; j < 5; j++)
                    {

                        if (TC.bWholeTowerCounter[turretShopIndex[i], j] == false)
                        {
                            StarImages[i].sprite = StarSprites[j];
                            j = 10; // for문을 종료한다.
                        }

                    }
                }
                else
                {
                    StarImages[i].sprite = ShopSprites[10];
                }
            }
        }
    }

    Sprite SynergyIconTracker(int turretIndex, int dif)
    {

        switch (turretIndex)
        {
            case 0:
                if (dif == 1)
                    return SM.Sp[BaseTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[BaseTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 1:
                if (dif == 1)
                    return SM.Sp[BuffTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[BuffTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 2:
                if (dif == 1)
                    return SM.Sp[GrenadeTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[GrenadeTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 3:
                if (dif == 1)
                    return SM.Sp[LaserTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[LaserTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 4:
                if (dif == 1)
                    return SM.Sp[RailgunTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[RailgunTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 5:
                if (dif == 1)
                    return SM.Sp[MachinegunTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[MachinegunTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 6:
                if (dif == 1)
                    return SM.Sp[TankTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[TankTurret.GetComponent<Turret_Parent>().synergyCode2];
            case 7:
                if (dif == 1)
                    return SM.Sp[MissileTurret.GetComponent<Turret_Parent>().synergyCode1];
                else
                    return SM.Sp[MissileTurret.GetComponent<Turret_Parent>().synergyCode2];
            default: break;
        }
        return null;
    }

    public void PayForAutoReroll()
    { // 웨이브 종료 시 자동 리롤될 때 사용되는 골드를 보충.
        int tmp;
        tmp = 0;
        for (int i = 0; i < slotCount; i++)
        {
            if (turretShopIndex[i] == -1)
            {
                tmp += 30;
            }
        }
        PlayerStats.Money += 50 + tmp;
    }

    public void RerollTurretShop()
    {
        SDM.UIClicked.Play();
        int tmp;
        tmp = 0;
        for(int i = 0; i < slotCount; i++)
        {
            if(turretShopIndex[i] == -1)
            {
                tmp+= 30;
            }
        }
        
        if(PlayerStats.Money < 50 + tmp)
        {
            GSM.MakeDialogue("골드가 부족합니다.     ( " + (50 + tmp) + "G )");
            SDM.Denied.Play();
            return;
        }
        PlayerStats.Money -= 50 + tmp;
        Random rand = new Random();
        int temp;
        for (int i = 0; i < slotCount; i++)
        {
            temp = Random.Range(0, 8);
            turretShopIndex[i] = temp;
        }
        RerollCost.text = ("50");
        UpdateSprites();
    }

    public void UnlockSlot()
    {
        SDM.UIClicked.Play();
        if (slotCount < 8)
        {
            if (PlayerStats.Money >= 300 * (slotCount - 2))
            {
                //           Bt[Slot_numb] = Bt[Slot_numb].GetComponent<Button>();
                ShopBtns[slotCount].image.sprite = ShopSprites[10];
                
                PlayerStats.Money -= 300 * (slotCount - 2);
                slotCount++;
                UnlockCost.text = (300 * (slotCount - 2)).ToString();
            }
            else
            {
                GSM.MakeDialogue("골드가 부족합니다.");
                SDM.Denied.Play();
                return;
            }
        }
    }


    public void UpgradeTurret(GameObject turret, int turretIndex, int turretLv) //타워카운터에서넘겨준캐넌을 업그레이드
    {
        GameObject nextTurret = (GameObject)Instantiate(turret.GetComponent<Turret_Parent>().TurretNextLV,turret.transform.position,Quaternion.identity);
        TC.UpdateTurretStatus(nextTurret, turretIndex, turretLv + 1);
        GameObject effect = (GameObject)Instantiate(buildEffect, nextTurret.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(effect, 1.0f);
        turret.GetComponent<Turret_Parent>().GetBaseNode().UpdateStatus(turretIndex);

        UpdateSprites();
        SM.UpdateSynergy(turretIndex);
    }

    public void SetTargetNode(Node _node)
    {
        TargetNode = _node;
    }

    public void ClickBtn1()
    {
        ExecuteBtn(0);
    }
    public void ClickBtn2()
    {
        ExecuteBtn(1);
    }
    public void ClickBtn3()
    {
        ExecuteBtn(2);
    }
    public void ClickBtn4()
    {
        ExecuteBtn(3);
    }
    public void ClickBtn5()
    {
        ExecuteBtn(4);
    }
    public void ClickBtn6()
    {
        ExecuteBtn(5);
    }
    public void ClickBtn7()
    {
        ExecuteBtn(6);
    }
    public void ClickBtn8()
    {
        ExecuteBtn(7);
    }

    private void ExecuteBtn(int index)
    {
        SDM.UIClicked.Play();
        if (slotCount < index + 1)
        {
            return;
        }
        if (!CheckMoney())
        {
            GSM.MakeDialogue("골드가 부족합니다.     ( 100G )");
            SDM.Denied.Play();
            return;
        }
        if (!TargetNode)
        {
            GSM.MakeDialogue("터렛을 설치할 위치를 지정하지 않았습니다.");
            SDM.Denied.Play();
            return;
        }

        BuildTurret(turretShopIndex[index]);
        turretShopIndex[index] = -1;
        ShopBtns[index].enabled = false;
        ShopBtns[index].image.sprite = ShopSprites[10];
        StarImages[index].sprite = ShopSprites[10];
        SlotImages[index].sprite = ShopSprites[12];
        SynergyIconSlotImages1[index].sprite = ShopSprites[10];
        SynergyIconSlotImages2[index].sprite = ShopSprites[10];
        SynergyIconImages1[index].sprite = ShopSprites[10];
        SynergyIconImages2[index].sprite = ShopSprites[10];
        PlayerStats.Money -= 100;
        int tmp = 0;
        for(int i = 0; i < slotCount; i++)
        {
            if(turretShopIndex[i] == -1)
            {
                tmp += 30;
            }
        }
        RerollCost.text = (50 + tmp).ToString();

    }

    private bool CheckMoney()
    {
        if(PlayerStats.Money >= 100)
        {
            return true;
        }
        return false;
    }

    public void BuildTurret(int turretIndex)
    {
        if(!TargetNode)
        {
            GSM.MakeDialogue("터렛을 설치할 위치를 지정하지 않았습니다.");
            SDM.Denied.Play();
            return;
        }
        switch (turretIndex)
        {
            case 0:
                GameObject turret1 = (GameObject)Instantiate(BaseTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret1.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret1;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret1, 0 , 0);
                break;   //터렛 생성, 골드 차감, 해당 노드의 터렛정보 갱신
            case 1:
                GameObject turret2 = (GameObject)Instantiate(BuffTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret2.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret2;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret2, 1, 0); 
                break;//인자로넘겨주는 숫자는 터렛 종류인덱스 (1) , 터렛레벨 (0) 순
            case 2:
                GameObject turret3 = (GameObject)Instantiate(GrenadeTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret3.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret3;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret3, 2, 0);
                break;
            case 3:
                GameObject turret4 = (GameObject)Instantiate(LaserTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret4.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret4;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret4, 3, 0);
                break;
            case 4:
                GameObject turret5 = (GameObject)Instantiate(RailgunTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret5.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret5;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret5, 4, 0);
                break;
            case 5:
                GameObject turret6 = (GameObject)Instantiate(MachinegunTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret6.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret6;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret6, 5, 0);
                break;
            case 6:
                GameObject turret7 = (GameObject)Instantiate(TankTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret7.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret7;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret7, 6, 0);
                break;
            case 7:
                GameObject turret8 = (GameObject)Instantiate(MissileTurret, TargetNode.GetBuildPosition() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                turret8.GetComponent<Turret_Parent>().SetBaseNode(TargetNode);
                TargetNode.turret = turret8;
                TargetNode.UpdateStatus(turretIndex);
                TC.UpdateTurretStatus(turret8, 7, 0);
                break;
            default:
                break;
        }
        TargetNode = null; // 터렛이 지어진 이후 해당노드위의 빌드타겟팅 제거
    }

}
