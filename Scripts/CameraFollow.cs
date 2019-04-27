using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;    // 카메라가 쫓는 타겟
    public float smoothing = 5f;    // 카메라 움직임 부드럽게 하기 위한 정도

    Vector3 offset; //플레이어와 카메라 오프셋 저장

    private void Start()
    {
        offset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetCamPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}
