using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickInput : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;                // 조이스틱의 배경
    private Image joystickImg;          // 조이스틱(움직이는 부분)
    private Vector3 inputVector;
    private Vector3 joystickFirstPos;   // 조이스틱의 처음 위치를 저장

    public bool trigger;

    void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
        joystickFirstPos = joystickImg.transform.position;
    }

    void Update()
    {
        
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        //Debug.Log("Joystick >>> OnDrag()");

        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2 , pos.y * 2 , 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // 조이스틱 이미지 이동
            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 2.5f),
                                                            inputVector.y * (bgImg.rectTransform.sizeDelta.y / 2.5f));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
        trigger = true;
    }
    

    // 터치가 끝나면 조이스틱의 위치를 원래대로 수정 
    public virtual void OnPointerUp(PointerEventData ped)
    {        
        joystickImg.transform.position = joystickFirstPos;
        inputVector.x = 0;
        inputVector.y = 0;
        trigger = false;
    }

    public float GetHorizontalValue()
    {
        if (inputVector.x != 0)
            return inputVector.x;
        else
            return Input.GetAxis("Horizontal");
    }
    public float GetVerticalValue()
    {
        if (inputVector.y != 0)
            return inputVector.y;
        else
            return Input.GetAxis("Vertical");
    }
}
