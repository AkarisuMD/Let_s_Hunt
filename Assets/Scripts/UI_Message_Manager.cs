using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using FMODUnity;
using DG.Tweening;

public class UI_Message_Manager : Singleton<UI_Message_Manager>
{
    [SerializeField] private GameObject messageParent;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private RectTransform DownPoint;
    [SerializeField] private RectTransform LeftDownPoint;
    [SerializeField] private RectTransform LeftUpPoint;
    public async void ShowMessage(Color color, string text, string fmodEventName = null, MessagePosition messagePosition = MessagePosition.Down)
    {
        GameObject newMessage = Instantiate(messagePrefab, messageParent.transform);

        switch(messagePosition)
        {
            case MessagePosition.Down:
                newMessage.transform.position = DownPoint.position;
                //newMessage.GetComponent<RectTransform>().anchoredPosition = DownPoint.anchoredPosition;
                break;
            case MessagePosition.LeftDown:
                newMessage.transform.position = LeftDownPoint.position;
                //newMessage.GetComponent<RectTransform>().anchoredPosition = LeftDownPoint.anchoredPosition;
                break;
            case MessagePosition.LeftUp:
                newMessage.transform.position = LeftUpPoint.position;
                //newMessage.GetComponent<RectTransform>().anchoredPosition = LeftUpPoint.anchoredPosition;
                break;
        }
        TextMeshProUGUI messageTMPro = newMessage.GetComponentInChildren<TextMeshProUGUI>();

        messageTMPro.color = color;
        messageTMPro.text = text;

        await messageTMPro.DOColor(new Color( messageTMPro.color.r, messageTMPro.color.g, messageTMPro.color.b, 0), 5).AsyncWaitForCompletion();
        Destroy(newMessage);
    }
}
public enum MessagePosition
{
    Down, LeftDown, LeftUp
}
