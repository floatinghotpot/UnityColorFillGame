using UnityEngine;

public class FPS : MonoBehaviour
{
    private float m_lastUpdateShowTime = 0f;
    private readonly float m_updateTime = 0.05f;
    private int m_frames = 0;
    private float m_frameDeltaTime = 0;
    private float m_FPS = 0;
    private Rect m_fps, m_dtime;
    private GUIStyle m_style = new GUIStyle();

    void Awake()  {
        Application.targetFrameRate = 60; //-1;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_lastUpdateShowTime = Time.realtimeSinceStartup;
        m_fps = new Rect(0, 0, 100, 100);
        m_dtime = new Rect(0, 300, 100, 100);
        m_style.fontSize = 28;
        m_style.normal.textColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        m_frames ++;
        float deltaTime = Time.realtimeSinceStartup - m_lastUpdateShowTime;
        if(deltaTime>= m_updateTime) {
            m_FPS = m_frames / deltaTime;
            m_frameDeltaTime = deltaTime / m_frames;
            m_frames = 0;
            m_lastUpdateShowTime = Time.realtimeSinceStartup;
        }
    }

    void OnGUI() {
        bool showFps = true; // (BuildSystemConfig.BUILD_PHASE == "alpha" || BuildSystemConfig.BUILD_PHASE == "beta");
        if(showFps) GUI.Label(m_fps, "FPS: " + (Mathf.RoundToInt(m_FPS*10)/10f), m_style);
    }
}
