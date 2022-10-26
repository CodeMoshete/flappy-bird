using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour
{
    public EventId EventToSend;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        Service.EventManager.SendEvent(EventToSend, null);
    }
}
