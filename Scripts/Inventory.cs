using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    
    public GameObject slotHolder;       // 전체 슬롯
    public Transform weaponHold;        // 무기를 드는 위치
    public GameObject[] allProducts;    // 모든 제작 아이템
    public Sprite[] allItemSprites;     // 아이템 이미지
    public Image shootImage;            // 공격 이미지 UI

    private int allSlots;       // 모든 슬롯 갯수
    [SerializeField]
    private int highlightSlot;  // 현재 사용중인 or 제작할 수 있는 아이템들을 나타냄
    [SerializeField]
    bool composeMode = false;   // 아이템 제작 모드
    [SerializeField]
    bool enoughMaterial = false;
    private GameObject[] slot;
    [SerializeField]
    public GameObject highlightedItem;

    Animator playerAnim;

    bool trigger = false;

    Dictionary<int, Item> items = new Dictionary<int, Item>();  // ID별 아이템
    Dictionary<int, int> slotItem = new Dictionary<int, int>(); // ID별 슬롯 번호
    
    private void Start()
    {
        playerAnim = transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Animator>();
        playerAnim.SetBool("EmptyWalk", true);

        allSlots = 9;
        slot = new GameObject[allSlots];

        for(int i= 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;

            if (slot[i].GetComponent<Slot>().item == null)
                slot[i].GetComponent<Slot>().empty = true;
        }
    }

    private void Update()
    {
        if (trigger == true)
            Shoot();
        else
            return;
    }

    private void OnTriggerEnter(Collider collider)  // item 줍기
    {
        if(collider.tag == "Item")
        {
            GameObject itemPickedUp = collider.gameObject;
            Item item = itemPickedUp.GetComponent<Item>();
            for (int i = 0; i < allSlots; i++)
            {
                if (slot[i].GetComponent<Slot>().item != null && item.ID == slot[i].GetComponent<Slot>().item.GetComponent<Item>().ID)
                {
                    // 가지고 있는 아이템인 경우 갯수만 바꾼다
                    ChangeCount(i, item);
                    ChangeText(i, slot[i].GetComponent<Slot>().item.GetComponent<Item>().description, slot[i].GetComponent<Slot>().item.GetComponent<Item>().currentCount);
                    if (slot[i].GetComponent<Slot>().highlight == true)
                        slot[i].GetComponent<Slot>().item.SetActive(true);
                    Destroy(collider.gameObject);
                    break;
                }
                else if (slot[i].GetComponent<Slot>().empty == true)
                {
                    // 새로운 아이템인 경우
                    item.currentCount = item.earnCount;
                    AddItem(itemPickedUp, item.ID, item.description, item.currentCount);
                    if (items.Count == 0)       // 가장 처음 아이템을 줍는 경우
                    {
                        shootImage.GetComponent<Image>().sprite = allItemSprites[itemPickedUp.GetComponent<Item>().ID];
                        Destroy(weaponHold.GetChild(0).gameObject);
                        if (itemPickedUp.GetComponent<Item>().type == "Gun")
                            playerAnim.SetBool("EmptyHand", false);
                        else
                            playerAnim.SetBool("EmptyHand", true);
                    }
                    if (item.ID == 4) {
                        items[4] = itemPickedUp.GetComponent<Item>();
                    }
                    else
                    {
                        items.Add(item.ID, item);
                    }
                    break;
                }
            }
        }
    }

    void AddItem(GameObject itemObject, int itemID, string itemName, int itemCount)  // slot에 아이템 추가
    {
        for (int i = 0; i < allSlots; i++)
        {            
            if (slot[i].GetComponent<Slot>().empty) // 비어있는 slot에 아이템 추가
            {
                itemObject.GetComponent<Item>().pickedUp = true;
                slot[i].GetComponent<Slot>().item = itemObject;
                slotItem.Remove(itemObject.GetComponent<Item>().ID);    // pistol을 autoPistol로 바꾸는 과정에서 딕셔너리의 같은 키를 가진 value값을 제거
                slotItem.Add(itemObject.GetComponent<Item>().ID, i);
                if (itemObject.GetComponent<Item>().ID == 4 && itemObject.GetComponent<Item>().composable == false)
                    TurnOnHighlight(itemObject);
                ChangeText(i, itemName, itemCount);
                itemObject.transform.parent = weaponHold;
                itemObject.transform.position = weaponHold.position;
                itemObject.transform.rotation = weaponHold.rotation;
                itemObject.GetComponent<Collider>().enabled = false;

                if (slot[i].GetComponent<Slot>().highlight == true)
                {
                    itemObject.gameObject.SetActive(true);
                    highlightedItem = itemObject;
                    highlightSlot = i;
                }
                else
                {
                    itemObject.gameObject.SetActive(false);
                }

                slot[i].GetComponent<Slot>().empty = false;
                break;
            }         
        }
    }

    void ChangeCount(int number, Item item) // 기존에 있는 아이템을 획득한 경우
    {
        slot[number].GetComponent<Slot>().item.GetComponent<Item>().currentCount += item.earnCount;
        slot[number].GetComponent<Slot>().outOfAmmo = false;
        slot[number].GetComponent<Slot>().ResetTextColor();
    }

    public void ChangeText(int number, string itemName, int itemCount) // 아이템 text 수정
    {                     // 몇번째슬롯?, 아이템명칭?, 아이템잔여개수?
        slot[number].gameObject.GetComponent<Text>().text =  itemName.ToString() + " x " + itemCount.ToString();
        slot[number].GetComponent<Slot>().TextColor(slot[number].GetComponent<Slot>().item.GetComponent<Item>().ID);
    }

    public void Switch()   // 장착중인 아이템 변경, 조합 모드 OFF
    {
        if (composeMode)
        {
            composeMode = false;
            TurnOnHighlight(tempObject);
        }
        else
        {
            for (int i = 0; i < slot.Length; i++)
            {
                if (slot[i].GetComponent<Slot>().highlight == true && slot[i].GetComponent<Slot>().item != null)
                {
                    slot[i].GetComponent<Slot>().highlight = false;
                    slot[i].GetComponent<Slot>().item.gameObject.SetActive(false);
                    i++;
                    if (i > weaponHold.childCount - 1)
                        i = 0;
                    slot[i].GetComponent<Slot>().highlight = true;
                    if (slot[i].GetComponent<Slot>().item.gameObject.GetComponent<Item>().currentCount != 0)
                        slot[i].GetComponent<Slot>().item.gameObject.SetActive(true);
                    highlightedItem = slot[i].GetComponent<Slot>().item.gameObject;
                    shootImage.GetComponent<Image>().sprite = allItemSprites[highlightedItem.GetComponent<Item>().ID];
                    if (highlightedItem.GetComponent<Item>().type == "Gun")
                        playerAnim.SetBool("EmptyHand", false);
                    else
                        playerAnim.SetBool("EmptyHand", true);
                    highlightSlot = i;
                    break;
                }
            }
        }
    }

    public void Shoot()    // 아이템 사용
    {
        if (composeMode == false)
        {
            if (highlightedItem.GetComponent<Item>().usable == true)
            {
                if (highlightedItem.GetComponent<Item>().type == "Gun") // 총 아이템의 사용
                {
                    highlightedItem.GetComponent<Gun>().Shoot();
                    ChangeText(highlightSlot, highlightedItem.GetComponent<Item>().description, highlightedItem.GetComponent<Gun>().currentCount);
                    if (highlightedItem.GetComponent<Gun>().currentCount == 0)
                    {
                        slot[highlightSlot].GetComponent<Slot>().OutOfAmmoTextColor();
                    }
                }
                else if (highlightedItem.GetComponent<Item>().type == "Exp")    // 투척 아이템 사용
                {
                    if (highlightedItem.GetComponent<Item>().ID == 8)
                    {
                        // 2개를 던지지 못하도록 만듬
                        highlightedItem.GetComponent<Item>().usable = false;

                    }
                    highlightedItem.GetComponent<Throw>().Shoot();
                    ChangeText(highlightSlot, highlightedItem.GetComponent<Item>().description, highlightedItem.GetComponent<Throw>().currentCount);
                    if (highlightedItem.GetComponent<Throw>().currentCount == 0)
                    {
                        slot[highlightSlot].GetComponent<Slot>().item.SetActive(false);
                        slot[highlightSlot].GetComponent<Slot>().OutOfAmmoTextColor();
                    }
                }
                return;
            }
            else
            {
                if (highlightedItem.GetComponent<Item>().ID == 8)
                {
                    // 한번 더 눌렀을시 C4 격발
                    highlightedItem.GetComponent<Throw>().RemoteTrigger();
                    highlightedItem.GetComponent<Item>().usable = true;
                }
                return;
            }
        }
    }

    int currentComposeId = 0;
    GameObject tempObject;

    public void CheckComposableItem()       // 조합 가능한 아이템 개수 체크, 조합 모드 ON
    {
        tempObject = slot[highlightSlot].GetComponent<Slot>().item;
        composeMode = true;
        for (int i = 0; i < slot.Length; i++)
        {
            slot[i].GetComponent<Slot>().highlight = false;
            if(slot[i].GetComponent<Slot>().item!=null)
                slot[i].GetComponent<Slot>().item.gameObject.SetActive(false);
        }
        switch (currentComposeId)
        {
            case 0:
                if (items.ContainsKey(0) && items.ContainsKey(1) && items.ContainsKey(3) &&
                    items[0].currentCount >= 1 && items[1].currentCount >= 1 && items[3].currentCount >= 1)
                {
                    enoughMaterial = true;
                    ShowComposableItem(7);
                }
                else
                {
                    enoughMaterial = false;
                }
                break;
            case 1:
                if (items.ContainsKey(0) && items.ContainsKey(1) && items.ContainsKey(2) && items.ContainsKey(3) &&
                    items[0].currentCount >= 2 && items[1].currentCount >= 1 && items[2].currentCount >= 1 && items[3].currentCount >= 1)
                {
                    enoughMaterial = true;
                    ShowComposableItem(8);
                }
                else
                {
                    enoughMaterial = false;
                }
                break;
            case 2:
                if (items.ContainsKey(1) && items.ContainsKey(2) && items.ContainsKey(4) &&
                    items[1].currentCount >= 3 && items[2].currentCount >= 2 && items[4].currentCount >= 1 && items[4].composable == true)
                {
                    enoughMaterial = true;
                    ShowComposableItem(4);
                }
                else
                {
                    enoughMaterial = false;
                }
                break;
        }
        currentComposeId += 1;
        if (currentComposeId > 2)
            currentComposeId = 0;
    }

    public void ShowComposableItem(int id)      // 조합 가능한 아이템 슬롯 박스 활성화
    {
        TurnOffHighlight();
        switch (id)
        {
            case 7:
                slot[slotItem[0]].GetComponent<Slot>().highlight = true;
                slot[slotItem[1]].GetComponent<Slot>().highlight = true;
                slot[slotItem[3]].GetComponent<Slot>().highlight = true;
                shootImage.GetComponent<Image>().sprite = allItemSprites[7];
                break;
            case 8:
                slot[slotItem[0]].GetComponent<Slot>().highlight = true;
                slot[slotItem[1]].GetComponent<Slot>().highlight = true;
                slot[slotItem[2]].GetComponent<Slot>().highlight = true;
                slot[slotItem[3]].GetComponent<Slot>().highlight = true;
                shootImage.GetComponent<Image>().sprite = allItemSprites[8];
                break;
            case 4:
                slot[slotItem[1]].GetComponent<Slot>().highlight = true;
                slot[slotItem[2]].GetComponent<Slot>().highlight = true;
                slot[slotItem[4]].GetComponent<Slot>().highlight = true;
                shootImage.GetComponent<Image>().sprite = allItemSprites[9];
                break;
        }
    }

    public void ComposeUI()     // 하나의 UI가 두가지 기능을 하기 때문에 이를 구분하기 위한 메서드
    {
        if (composeMode)
            ComposeItem();
    }

    public void TriggerDown()
    {
        if(highlightedItem != null)
            trigger = true;
    }

    public void TriggerUp()
    {
        trigger = false;
    }
    
    public void ComposeItem()  // 아이템 조합
    {
        AudioManager.instance.PlaySound("Reload", Camera.main.transform.position);
        if (composeMode == true && enoughMaterial == true)
        {
            switch (currentComposeId-1)
            {
                case -1:     // Auto Pistol 제작
                    for (int i = 0; i < slot.Length; i++)
                    {
                        if (slot[i].GetComponent<Slot>().item.GetComponent<Item>().ID == 4)
                        {
                            int pistolCount = slot[i].GetComponent<Slot>().item.GetComponent<Item>().currentCount;
                            allProducts[0].GetComponent<Item>().earnCount = pistolCount;
                            Destroy(slot[i].GetComponent<Slot>().item.gameObject);
                            slot[i].GetComponent<Slot>().empty = true;
                            GameObject newPistol = Instantiate(allProducts[0], transform.position, allProducts[0].transform.rotation);
                            newPistol.GetComponent<Item>().ID = 4;
                            TurnOnHighlight(allProducts[0]);
                            playerAnim.SetBool("EmptyHand", false);
                            allItemSprites[4] = allItemSprites[9];
                            break;
                        }
                        items[1].currentCount -= 3;
                        ChangeText(slotItem[1], "metalscrap", items[1].currentCount);
                        items[2].currentCount -= 2;
                        ChangeText(slotItem[2], "metalpipe", items[2].currentCount);
                    }
                    break;
                case 0:     // Mine 제작
                    Instantiate(allProducts[1], transform.position, allProducts[1].transform.rotation);
                    items[0].currentCount -= 1;
                    ChangeText(slotItem[0], "explosive", items[0].currentCount);
                    items[1].currentCount -= 1;
                    ChangeText(slotItem[1], "metalscrap", items[1].currentCount);
                    items[3].currentCount -= 1;
                    ChangeText(slotItem[3], "sensor", items[3].currentCount);
                    TurnOnHighlight(tempObject);
                    break;

                case 1:     // C4 제작
                    Instantiate(allProducts[2], transform.position, allProducts[2].transform.rotation);
                    items[0].currentCount -= 2;
                    ChangeText(slotItem[0], "explosive", items[0].currentCount);
                    items[1].currentCount -= 1;
                    ChangeText(slotItem[1], "metalscrap", items[1].currentCount);
                    items[2].currentCount -= 1;
                    ChangeText(slotItem[2], "metalpipe", items[2].currentCount);
                    items[3].currentCount -= 1;
                    ChangeText(slotItem[3], "sensor", items[3].currentCount);
                    TurnOnHighlight(tempObject);
                    break;
            }
        }
    }

    void TurnOffHighlight()
    {
        for (int i = 0; i < slot.Length; i++)
            slot[i].GetComponent<Slot>().highlight = false;
    }

    void TurnOnHighlight(GameObject gameObject)
    {
        TurnOffHighlight();

        for (int i = 0; i < slot.Length; i++) {
            if (slot[i].GetComponent<Slot>().item != null)
            {
                if (gameObject.GetComponent<Item>().ID == slot[i].GetComponent<Slot>().item.gameObject.GetComponent<Item>().ID)
                {
                    slot[i].GetComponent<Slot>().highlight = true;
                    slot[i].GetComponent<Slot>().item.SetActive(true);
                    highlightSlot = i;
                    highlightedItem = gameObject;
                    shootImage.GetComponent<Image>().sprite = allItemSprites[highlightedItem.GetComponent<Item>().ID];
                    composeMode = false;
                }
            }
        }
    }
}
