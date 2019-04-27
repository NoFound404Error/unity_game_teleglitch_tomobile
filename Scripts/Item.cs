using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [Header("Common property")]
    public int ID;                  // 순번
    public string type;             // 타입(총, 던지는 폭발물, 단순 재료, 총알)
    public string description;      // 이름

    public int currentCount;        // 현재 갯수
    public int earnCount;           // 한번에 얻는 갯수

    public bool composable;         // 조합 가능한 재료 또는 무기인지
    public bool usable;             // 사용 가능한 아이템인지(총, 폭발물, 지뢰 C4)
    public bool equipped;           // 현재 장착중인 아이템인가

    public bool pickedUp;           // 주워진 이후 행동을 위한 bool 변수
    
}
