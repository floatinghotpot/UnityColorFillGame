using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unicgames
{

public class UISafeArea : MonoBehaviour
{
    [SerializeField] RectTransform m_canvasRect;

    // Start is called before the first frame update
    void Awake()
    {
        AutoFit();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        AutoFit();
    }

    void AutoFit() {
        // adjust UI to fit safe area
        Rect area = Screen.safeArea;
        float w = Screen.width, h = Screen.height;
        Vector2 sz = m_canvasRect.rect.size;
        Vector2 offsetMin = new Vector2(sz.x * area.xMin / w, sz.y * area.yMin / h);
        Vector2 offsetMax = new Vector2(sz.x * (area.xMax - w) / w, sz.y * (area.yMax - h) / h);

        RectTransform rect = GetComponent<RectTransform>();
        if((offsetMin - rect.offsetMin).magnitude > 1) rect.offsetMin = offsetMin;
        if((offsetMax - rect.offsetMax).magnitude > 1) rect.offsetMax = offsetMax;
    }
}

}
