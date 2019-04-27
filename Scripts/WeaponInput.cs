using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
    //private Button mainWeapon;   // 현재 사용하는 무기
    private bool trigger = false;
    public GunController gunController;
    
	// Use this for initialization
	void Start () {
    }

    void Update()
    {
        if (trigger == true) {
            gunController.Shoot();
        }
    }

    public void OnPointerDown (PointerEventData eventData)
    {
        trigger = true;
        print("동작하나?");
        print(gunController.weaponHold.transform.GetChild(0).GetComponent<Gun>().weaponNum == 4);
            //AudioManager.instance.PlaySound("Drill", Camera.main.transform.position);
        // 드릴 사운드만 따로 넣어야 함
        //Debug.Log("Trigger has been pulled!");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        trigger = false;
        //Debug.Log("Trigger has reset!");
    }
}
