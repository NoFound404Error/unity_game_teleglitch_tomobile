using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RPBManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform ReloadingBar;
    //public Transform TextIndicator;
    //public Transform TextLoading;
    [SerializeField] private float currentAmount;
    [SerializeField] private float speed;

    private bool trigger = false;
    public Inventory gunController;

    void Update () {
        GaugeFill();
    }

    void GaugeFill()
    {
        if(currentAmount > 0 && trigger == true)
        {
            currentAmount -= (speed / 2) * Time.deltaTime;
            //TextIndicator.GetComponent<Text>().text = ((int)currentAmount).ToString() + "%";
            //TextLoading.gameObject.SetActive(true);
            gunController.Shoot();
        }
        else if(currentAmount < 100 && trigger == false)
        {
            currentAmount += speed * Time.deltaTime;
            
            //TextLoading.gameObject.SetActive(false);
            //TextIndicator.GetComponent<Text>().text = "DONE!";
        }
        ReloadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        trigger = true;
        if(gunController.weaponHold.transform.GetChild(0).gameObject.activeSelf)
            AudioManager.instance.PlaySound("Drill", Camera.main.transform.position);
        //Debug.Log("Trigger has been pulled!");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        trigger = false;
        AudioManager.instance.PlaySound("Reload", Camera.main.transform.position); 
        
        //Debug.Log("Trigger has reset!");
    }
}
