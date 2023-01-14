using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unicgames
{

public class UICanvasAutoFit : MonoBehaviour
{
    [SerializeField] int m_DesignedWidth = 640;
    [SerializeField] int m_DesignedHeight = 1136;
    [SerializeField] bool portraitOkay = true;
    [SerializeField] bool landscapeOkay = true;

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
        // adjust orientation
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if( scaler != null) {
            Vector2 whPortrait = new Vector2(Mathf.Min(m_DesignedWidth,m_DesignedHeight), Mathf.Max(m_DesignedWidth,m_DesignedHeight));
            Vector2 whLandscape = new Vector2(Mathf.Max(m_DesignedWidth,m_DesignedHeight), Mathf.Min(m_DesignedWidth,m_DesignedHeight));
            Vector2 wh = whPortrait;
            if(portraitOkay && landscapeOkay) {
                wh = (Screen.width < Screen.height) ? whPortrait : whLandscape;
            } else if(landscapeOkay) {
                wh = whLandscape;
            }
            if( scaler.referenceResolution.x != wh.x ) {
                scaler.referenceResolution = wh;
            }
        }

        // auto set the width / height match ratio, 
        // but sometimes may not be perfect
        // still recommend mannual set
        float match = 0;
        if(Screen.width > Screen.height) { // LandSpaceLeft
            float realScale = Screen.width / Screen.height;
            float defaultScale = Mathf.Max(m_DesignedWidth,m_DesignedHeight) / Mathf.Min(m_DesignedWidth,m_DesignedHeight);
            match = (realScale > defaultScale) ? 1f : 0f;
        } else { // Portrait
            match = 0.5f;
        }
        scaler.matchWidthOrHeight = match;
    }
}

}
