using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

    // 아이템 Text UI 공간
    public bool empty;
    public bool highlight;
    public bool outOfAmmo;
    public GameObject item;

    Text text;
    GameObject highlightImage;      // 현재 사용하는 아이템을 나타내주는 박스
    Color[] allColors = {
        new Color(156 / 255f, 183 / 255f, 1, 150 / 255f),           // 파랑
        new Color(135 / 255f, 213 / 255f, 155 / 255f, 150 / 255f),  // 초록
        new Color(1, 1, 1, 150 / 255f),                             // 흰색
        new Color(225/255f,186/156f,156/255f,150/255f),             // 살색
        new Color(224/255f, 113/255f, 98/255f, 150/255f)            // 빨강
    };                           
    Color currentColor;
    Color originalColor;

    void Start()
    {
        originalColor = this.GetComponent<Text>().color;
        currentColor = originalColor;
        highlightImage = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (highlight == true)
            highlightImage.SetActive(true);
        else
            highlightImage.SetActive(false);
    }    

    public void TextColor(int number)
    {
        if(number == 4|| number == 5|| number == 6)
        {
            GetComponent<Text>().color = allColors[0];
        }
        else if (number == 1 || number == 2 || number == 3)
        {
            GetComponent<Text>().color = allColors[2];
        }
        else if (number == 0 || number == 7 || number == 8)
        {
            GetComponent<Text>().color = allColors[1];
        }
    }

    public void OutOfAmmoTextColor()
    {
        currentColor = allColors[4];
        GetComponent<Text>().color = currentColor;
    }

    public void ResetTextColor()
    {
        currentColor = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a);
        GetComponent<Text>().color = currentColor;
    }
    
}
