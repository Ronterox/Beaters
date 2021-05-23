using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogPopUp : MonoBehaviour
{
    [SerializeField] Button dialogButton;
    [SerializeField] TextMeshPro dialogButtonText;
    [SerializeField] TextMeshPro popUpText;

    public void Init(Transform canvas, string popupMessage, string buttonText, Action action)
    {
        popUpText.text = popupMessage;
        dialogButtonText.text = buttonText;

        transform.SetParent(canvas);
        transform.localScale = Vector3.one;

        dialogButton.onClick.AddListener(() =>
        {
            action();
        });

    }
}
