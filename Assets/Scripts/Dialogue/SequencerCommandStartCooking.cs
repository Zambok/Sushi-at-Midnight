using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandStartCooking : SequencerCommand
    {
        public void Awake()
        {
            Debug.Log(" [StartCooking] 커맨드 시작");

            Customer customer = null;

            // 1. 대화 참여자들(화자, 청자) 중에서 Customer 컴포넌트를 가진 사람 찾기
            Transform speaker = GetSubject(0);
            Transform listener = GetSubject(1);

            // 화자(Subject 0)가 손님인지 체크
            if (speaker != null)
            {
                customer = speaker.GetComponent<Customer>();
            }

            // 화자가 손님이 아니면, 청자(Subject 1)가 손님인지 체크
            if (customer == null && listener != null)
            {
                customer = listener.GetComponent<Customer>();
            }

            // [수정됨] 그래도 못 찾았으면, 현재 대화의 주인공(Conversant)을 직접 조회
            // DialogueManager.currentConversant는 현재 대화의 상대방(NPC) Transform을 반환합니다.
            if (customer == null)
            {
                Transform conversant = DialogueManager.currentConversant;
                if (conversant != null)
                {
                    customer = conversant.GetComponent<Customer>();
                }
            }

            // 2. 결과 확인
            if (customer == null)
            {
                Debug.LogError($" [StartCooking] Customer를 찾을 수 없습니다! \nSpeaker: {speaker?.name}, Listener: {listener?.name}");
                Stop();
                return;
            }

            Debug.Log($" [StartCooking] 손님 찾음: {customer.name}");

            // 3. 파라미터 읽기 (공백 제거 포함)
            string recipeName = GetParameter(0)?.Trim();
            string requestType = GetParameter(1)?.Trim();

            // 4. 주문 생성 로직
            Order order = null;
            if (!string.IsNullOrEmpty(recipeName))
            {
                // FindObjectOfType으로 안전하게 찾기
                var orderManager = Object.FindFirstObjectByType<OrderManager>();
                if (orderManager != null)
                {
                    try
                    {
                        order = orderManager.CreateSpecificOrder(customer, recipeName, requestType);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($" [StartCooking] 주문 생성 중 에러: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError(" [StartCooking] OrderManager가 없습니다.");
                }
            }
            else
            {
                order = customer.CurrentOrder;
            }

            // 5. 주방 이동
            if (order != null)
            {
                if (GameManager.Instance != null)
                {
                    Debug.Log($" 주방으로 이동! (메뉴: {order.BaseRecipe?.name})");
                    GameManager.Instance.GoToKitchen(order);
                }
            }
            else
            {
                Debug.LogError(" [StartCooking] 주문 객체(Order) 생성 실패.");
            }

            Stop();
        }
    }
}