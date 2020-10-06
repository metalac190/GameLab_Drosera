using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    private static ToolTip instance;

    private Text tooltipText;
    private RectTransform backgroundRectTransform;

    private void Awake()
    {
        instance = this;

        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.Find("text").GetComponent<Text>();

        ShowToolTip("Random tooltip text");
    }

    private void Update()
    {
        this.transform.position = Input.mousePosition;
    }

    private void ShowToolTip(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        float textPaddingSize = 4.0f; 
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2.0f, tooltipText.preferredHeight + textPaddingSize * 2.0f);

        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowToolTip_Static(string tooltipString)
    {
        instance.ShowToolTip(tooltipString);
    }

    public static void HideToolTip_Static()
    {
        instance.HideToolTip();
    }
}
