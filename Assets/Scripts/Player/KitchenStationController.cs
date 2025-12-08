using DG.Tweening; // Dotween 필수
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public enum StationType
{
    None,       // 아무것도 아닌 상태 (메인 뷰)
    Rice,       // 밥 쥐기 (클로즈업)
    Cutting,    // 도마 칼질 (클로즈업)
    Assembly    // 조립/접시 (클로즈업 - 추후 사용)
}

public class KitchenStationController : MonoBehaviour
{
    public static KitchenStationController Instance { get; private set; }

    [Header("Main System")]
    [SerializeField] private CinemachineBrain _brain; // 메인 카메라의 Brain

    [Header("Virtual Cameras")]
    [Tooltip("홀(카운터)을 비추는 카메라")]
    [SerializeField] private CinemachineCamera _camCounter;

    [Tooltip("주방 전체를 비추는 카메라")]
    [SerializeField] private CinemachineCamera _camMainKitchen;

    [Tooltip("밥통 클로즈업 카메라")]
    [SerializeField] private CinemachineCamera _camRice;

    [Tooltip("도마 클로즈업 카메라")]
    [SerializeField] private CinemachineCamera _camCutting;

    [Header("UI & Effect")]
    [SerializeField] private CanvasGroup _transitionPanel; // 검은색 페이드 패널
    [SerializeField] private float _fadeDuration = 0.5f;   // 페이드 시간
    [SerializeField] private GameObject _backButton;       // 뒤로가기 버튼 (클로즈업 탈출용)

    private StationType _currentStation = StationType.None;

    private void Awake()
    {
        Instance = this;
        if (_backButton) _backButton.SetActive(false);
        if (_transitionPanel) _transitionPanel.alpha = 0f;
    }

    // =================================================================================
    // 1. [Fade] 홀 <-> 주방 메인 (공간 이동)
    //    GameManager에서 호출할 함수들
    // =================================================================================

    // 주방으로 입장 (홀 -> 주방)
    public void EnterKitchenMode()
    {
        StartCoroutine(FadeTransitionRoutine(true));
    }

    // 홀로 복귀 (주방 -> 홀)
    public void ExitKitchenMode()
    {
        StartCoroutine(FadeTransitionRoutine(false));
    }

    private IEnumerator FadeTransitionRoutine(bool isEnteringKitchen)
    {
        // 1. 페이드 아웃 (화면 암전)
        if (_transitionPanel) _transitionPanel.DOFade(1f, _fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);

        // 2. 이동 중 빈 공간 안 보이게 'Cut'으로 설정
        SetCameraBlend(CinemachineBlendDefinition.Styles.Cut, 0f);

        // 3. 카메라 우선순위 변경
        ResetPriorities();

        if (isEnteringKitchen)
        {
            _camMainKitchen.Priority = 20; // 주방 메인 ON
        }
        else
        {
            _camCounter.Priority = 20;     // 홀(카운터) ON
        }

        // 안정감을 위해 살짝 대기
        yield return new WaitForSeconds(0.1f);

        // 4. 페이드 인 (화면 밝아짐)
        if (_transitionPanel) _transitionPanel.DOFade(0f, _fadeDuration);
    }

    // =================================================================================
    // 2. [Cut] 주방 메인 <-> 클로즈업 (빠른 작업 전환)
    //    클릭 이벤트에서 호출할 함수들
    // =================================================================================

    public void EnterDetailView(StationType type)
    {
        if (_currentStation != StationType.None) return;

        // Cut 전환
        SetCameraBlend(CinemachineBlendDefinition.Styles.Cut, 0f);

        _currentStation = type;
        ResetPriorities();

        switch (type)
        {
            case StationType.Rice: _camRice.Priority = 20; break;
            case StationType.Cutting: _camCutting.Priority = 20; break;
        }

        if (_backButton) _backButton.SetActive(true);
    }

    public void ExitDetailView()
    {
        // Cut 전환
        SetCameraBlend(CinemachineBlendDefinition.Styles.Cut, 0f);

        _currentStation = StationType.None;
        ResetPriorities();
        _camMainKitchen.Priority = 20; // 주방 메인으로 복귀

        if (_backButton) _backButton.SetActive(false);
    }

    // =================================================================================
    // 유틸리티
    // =================================================================================

    private void SetCameraBlend(CinemachineBlendDefinition.Styles style, float time)
    {
        if (_brain != null)
        {
            _brain.DefaultBlend.Style= style;
            _brain.DefaultBlend.Time = time;
        }
    }

    private void ResetPriorities()
    {
        _camCounter.Priority = 0;
        _camMainKitchen.Priority = 0;
        _camRice.Priority = 0;
        _camCutting.Priority = 0;
    }
}