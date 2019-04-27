using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    public Transform weaponHold;    // 플레이어의 총을 잡을 손 위치
    public Gun[] allGuns;           // 모든 총들
    public Sprite[] allGunUI;
    public Image shootingImage;

    Dictionary<int, Gun> gunDic = new Dictionary<int, Gun>();
    public Dictionary<int, Text> txtDic = new Dictionary<int, Text>();

    private int gunNum;
    public int GunNum
    {
        get { return gunNum; }
        set
        {
            gunNum = value;
            //print("6");
            EquipGun(allGuns[gunNum]);
        }
    }
    Gun equippedGun;        // 현재 장착중인 총을 저장할 변수

    public Text textPrefab;
    public GameObject canvas;

    private void Awake()
    {
        //print("awake 메서드 실행");
        equippedGun = null;
    }

    private void Start()
    {
        EquipDrill();
    }

    private void Update()
    {
        shootingImage.GetComponent<Image>().sprite = allGunUI[equippedGun.weaponNum];
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)     // 현재 장착중인 무기가 있는지 확인
        {
            //for (int i = 0; i < gunDic.Count; i++)
            foreach (KeyValuePair<int, Gun> row in gunDic)
            {
                if (gunDic.ContainsKey(gunToEquip.weaponNum) == true)
                {   // 이미 획득한 무기인지 확인하고 획득한 무기라면 총알만 충전시킨다
                    //weaponHold.transform.GetChild(i).GetComponent<Gun>().CurrentAmmo +=
                    //                        weaponHold.transform.GetChild(i).GetComponent<Gun>().beginAmmo;
                    //print("이미 장착한 무기 총알만 보충");
                    //gunDic[gunToEquip.weaponNum].CurrentAmmo += gunDic[gunToEquip.weaponNum].beginAmmo;
                    
                }
                else
                { // 이미 획득한 무기가 아닌경우 무기를 Instantiate 한다
                    //print("새로운 무기 스폰");
                    equippedGun.gameObject.SetActive(false);
                    equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun; // as Gun으로 확장
                    // Instantiate는 Object를 반환하는데 우리가 사용하는 equippedGun은 Gun 타입이므로 Object를 Gun으로 형변환 해준다
                    Destroy(equippedGun.GetComponent<SphereCollider>());
                    // 장착된 총의 Collider로 인해 Trigger이벤트가 계속 발생하므로 SphereCollider를 없애준다.
                    equippedGun.transform.parent = weaponHold;
                    // 총 오브젝트가 플레이어를 따라 움직이도록 weaponHold의 자식 오브젝트로 넣어줌
                    if(gunToEquip.weaponNum != 1)
                        equippedGun.transform.GetChild(0).GetComponent<LineRenderer>().enabled = true;
                    gunDic.Add(gunToEquip.weaponNum, equippedGun);
                    SpawnTextUI(equippedGun.weaponNum);
                    break;  // Instantiate 한 무기를 이미 획득한 무기로 착각하는걸 방지하기 위해 for문 탈출
                }
            }
        }
        //else if(equippedGun == null)
        //{ // 처음 시작할때 장착된 총이 없으므로 Pistol 장착
        //    equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        //    Destroy(equippedGun.GetComponent<SphereCollider>());
        //    equippedGun.transform.parent = weaponHold;
        //    //guns.Add(equippedGun);
        //    gunDic.Add(gunToEquip.weaponNum, equippedGun);
        //    SpawnTextUI(4);
        //}
    }

    void EquipDrill()
    {
        //print("드릴 스폰");
        equippedGun = Instantiate(allGuns[4], weaponHold.position, weaponHold.rotation) as Gun;
        Destroy(equippedGun.GetComponent<SphereCollider>());
        equippedGun.transform.parent = weaponHold; 
        gunDic.Add(allGuns[4].weaponNum, equippedGun);
        SpawnTextUI(4);
    }
    
    public void Shoot()
    {
        if (equippedGun != null)
        {
            equippedGun.Shoot();
            
        }
    }

    public void SelectWeapon()  // 무기 스위칭, weaponHold의 자식에 있는 차례대로 SetActive를 true로 만든다
    {
        //print("무기 스위칭");
        for (int i = 0; i < gunDic.Count; i++)
        {
            if (weaponHold.transform.GetChild(i).gameObject.activeSelf)
            {
                weaponHold.transform.GetChild(i).gameObject.SetActive(false);
                i++;
                if (i > gunDic.Count - 1)
                    i = 0;
                weaponHold.transform.GetChild(i).gameObject.SetActive(true);
                
                equippedGun = weaponHold.transform.GetChild(i).GetComponent<Gun>();
                break;
            }
        }
    }

    void SpawnTextUI(int gunNum)
    {
        if (gunNum != 4)
        {
            //print("무기 텍스트 스폰");
            Text newText = Instantiate(textPrefab);
            newText.transform.SetParent(canvas.transform);
            newText.transform.position = new Vector2(Screen.width - 140, Screen.height / 2.2f + (gunDic.Count - 1) * 45);
            //newText.GetComponent<Text>().text = gunDic[gunNum].weaponName.ToString() + " x <color=ff0000>" + gunDic[gunNum].beginAmmo.ToString() + "</color>";
            txtDic.Add(gunNum, newText);
        }
        else
        {
            //print("드릴 텍스트 스폰");
            Text newText = Instantiate(textPrefab);
            newText.transform.SetParent(canvas.transform);
            newText.transform.position = new Vector2(Screen.width - 140, Screen.height / 2.2f + (gunDic.Count - 1) * 45);
            newText.GetComponent<Text>().text = "<color=ff0000>" + gunDic[gunNum].weaponName.ToString() + "</color>";
            txtDic.Add(gunNum, newText);
        }
    }
}
