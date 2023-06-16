using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SyncChatWithText : MonoBehaviour
{
    //========
    //VARIABLES
    //========

    [Header("Component")]
    [SerializeField] private TextMeshProUGUI chatText;

    [Header("Limit the message")]
    [SerializeField] private int userLenghtLimit = 4;
    [SerializeField] private int messageLenghtLimit = 12;

    [SerializeField] private int messageLimit = 12;

    [SerializeField] private bool popUp;
    [SerializeField] private MessagePosition popUpPosition;
    //========
    //FONCTIONS
    //========
    public void AddingChatMessageToUI(string user, string message)
    {
        //We Limit the message Lenght
        int messageLength = Mathf.Clamp(message.Length, 0, messageLenghtLimit);
        int userLength = Mathf.Clamp(user.Length, 0, userLenghtLimit);

        //If the message limit is exeded, erase the first line
        if(GetNumberOfLine(chatText.text.ToString()) >= messageLimit)
        chatText.text = RemoveTheFirstLineOfString(chatText.text.ToString());

        chatText.text += "\r\n" + user.Substring(0, userLength) + ": " + message.Substring(0, messageLength);

        if(popUp)
        {
            UI_Message_Manager.Instance.ShowMessage(Color.green, message.Substring(0, messageLength), null, popUpPosition);
        }
    }
    private string RemoveTheFirstLineOfString(string text)
    {
        int index = text.IndexOf(System.Environment.NewLine);
        string newText = text.Substring(index + System.Environment.NewLine.Length);

        return newText;
    }
    private int GetNumberOfLine(string text)
    {
        string[] newText = text.Split(System.Environment.NewLine);
        return newText.Length;
    }
}
