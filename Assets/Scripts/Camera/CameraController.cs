using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineCamera _vCamCounter; // CM vCam_Counter 연결
    [SerializeField] private CinemachineCamera _vCamKitchen; // CM vCam_Kitchen 연결

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 주방으로 이동
    public void MoveToKitchen()
    {
        // 주방 카메라의 우선순위를 높임
        _vCamKitchen.Priority = 10;
        _vCamCounter.Priority = 0;
    }

    // 홀로 복귀
    public void MoveToCounter()
    {
        // 홀 카메라의 우선순위를 높임
        _vCamCounter.Priority = 10;
        _vCamKitchen.Priority = 0;
    }

    // (선택) 특정 손님 줌인 효과 - 스토리 모드용
    public void FocusOnTarget(Transform target)
    {
        // 별도의 vCam_Zoom 하나를 더 만들어서
        // Follow 대상을 target으로 바꾸고 Priority를 높이는 방식으로 구현 가능
    }
}