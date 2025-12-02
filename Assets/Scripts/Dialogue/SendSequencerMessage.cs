using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class SendSequencerMessage : MonoBehaviour
{
    public string message = "Clicked"; // 보낼 메시지 이름

    private void Start()
    {
        // 버튼 컴포넌트를 찾아서 클릭 리스너 자동 등록
        GetComponent<Button>().onClick.AddListener(SendMessageToSequencer);
    }

    private void SendMessageToSequencer()
    {
        // 시퀀서에게 "Clicked"라는 메시지를 전송!
        Sequencer.Message(message);
    }
}