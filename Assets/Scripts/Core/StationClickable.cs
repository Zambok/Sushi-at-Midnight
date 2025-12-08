using UnityEngine;

public class StationClickable : MonoBehaviour
{
    [SerializeField] private StationType _targetStation;

    // 마우스가 올라갔을 때 하이라이트 효과 등을 줄 수 있음
    private void OnMouseEnter()
    {
        // 커서 변경 로직 등
    }

    private void OnMouseDown()
    {
        // 클릭하면 해당 스테이션으로 줌인!
        KitchenStationController.Instance.EnterDetailView(_targetStation);
    }
}