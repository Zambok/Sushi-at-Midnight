using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    // 사용법: SetEmotion(Happy) 또는 SetEmotion(Angry)
    public class SequencerCommandSetEmotion : SequencerCommand
    {
        public void Awake()
        {
            // 1. 대화 상대(Conversant = 손님) 찾기
            // GetSubject(1)은 시퀀서의 두 번째 주체, 즉 대화 상대를 가져옴
            Transform subject = GetSubject(1);

            if (subject == null)
            {
                Stop();
                return;
            }

            Customer customer = subject.GetComponent<Customer>();
            if (customer == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("SetEmotion: 대상에게 Customer 컴포넌트가 없습니다.");
                Stop();
                return;
            }

            // 2. 파라미터(감정 이름) 읽기
            // GetParameter(0)은 명령어 뒤의 첫 번째 인자 (예: Happy)
            string emotionString = GetParameter(0);

            // 3. String -> Enum 변환 및 적용
            try
            {
                CustomerEmotion emotion = (CustomerEmotion)System.Enum.Parse(typeof(CustomerEmotion), emotionString, true);
                customer.SetEmotion(emotion);
            }
            catch
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning($"SetEmotion: '{emotionString}'은(는) 유효한 CustomerEmotion이 아닙니다.");
            }

            // 4. 명령 종료
            Stop();
        }
    }
}