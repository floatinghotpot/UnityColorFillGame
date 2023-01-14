using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloodFill_Demo : MonoBehaviour
{
    //计算鼠标点击位置 对应的像素位置，一个是image的左下角，一个是图片的右上角
    public Transform textureOrigin;
    public Transform textureUPEnd;

    [SerializeField] Image m_image;
    [SerializeField] Image[] m_colors;
    [SerializeField] Image m_pen;

    Color m_selectedColor = Color.green;

    private void Start()
    {
    }
    private void Update()
    {
    }

	void OnEnable(){
		EasyTouch.On_SimpleTap += On_SimpleTap;
	}
    void OnDisable(){
		EasyTouch.On_SimpleTap -= On_SimpleTap;
    }

	void On_SimpleTap(Gesture gesture){
        FloodFill(gesture.position);
    }

    Vector2 ScreenToTexturePos(Image img, Vector3 tapPos) {
        Texture2D texture = img.sprite.texture;

        var rt = img.GetComponent<RectTransform>();
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        Vector3 delta = v[2] - v[0];
        Vector3 offset = tapPos - v[0];

        Vector2 colorPos;
        colorPos.x = offset.x / delta.x * texture.width;
        colorPos.y = offset.y / delta.y * texture.height;

        //print($"{v[0]}, {v[2]}, {tapPos}, {delta}, {offset}");
        //print($"colorPos 1: {colorPos}");
        return colorPos;
    }

    Vector2 ScreenToTexturePos2(Image img, Vector3 tapPos) {
        Texture2D texture = img.sprite.texture;

        Vector3 delta = textureUPEnd.position - textureOrigin.position;
        Vector3 offset = tapPos - textureOrigin.position;

        Vector2 colorPos;
        colorPos.x = offset.x / delta.x * texture.width;
        colorPos.y = offset.y / delta.y * texture.height;

        print($"colorPos 2: {colorPos}");
        return colorPos;
    }

    private void FloodFill(Vector3 tapPos)
    {
        Stack<int> stackX = new Stack<int>();
        Stack<int> stackY = new Stack<int>();

        Vector2 colorPos =  ScreenToTexturePos(m_image, tapPos);
        //Vector2 colorPos =  ScreenToTexturePos2(m_image, tapPos);

        if(colorPos.x < 0 || colorPos.x >= m_image.sprite.texture.width) return;
        if(colorPos.y < 0 || colorPos.y >= m_image.sprite.texture.height) return;

        Texture2D texture = m_image.sprite.texture;
        Color startColor = texture.GetPixel((int)colorPos.x, (int)colorPos.y);
        if(startColor == Color.black) {
            print("Fill on black is not allowed to avoid losing border line");
            return;
        }

        int[] dx = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };

        stackX.Push((int)colorPos.x);
        stackY.Push((int)colorPos.y);
        int x, y, xx, yy;
        int w = texture.width, h = texture.height;
        //print("texture size: " + texture.width + " x " + texture.height);
        int nMax = 0;
        while (stackX.Count > 0) {
            x = stackX.Pop();
            y = stackY.Pop();
            texture.SetPixel(x, y, m_selectedColor);
            for (int i = 0; i < 8; i++) {
                xx = x + dx[i];
                yy = y + dy[i];
                if (xx > 0 && xx < w && yy > 0 && yy < h) {
                    if(texture.GetPixel(xx, yy) == startColor) {
                        stackX.Push(xx);
                        stackY.Push(yy);
                    }
                }
            }
            if(nMax < stackX.Count) nMax = stackX.Count;
            if(nMax > 1000000) break;
        }
        print($"stack max depth: {nMax}");
        texture.Apply();
    }

    public void On_ClickColor0() { On_ClickColor(0); }
    public void On_ClickColor1() { On_ClickColor(1); }
    public void On_ClickColor2() { On_ClickColor(2); }
    public void On_ClickColor3() { On_ClickColor(3); }
    public void On_ClickColor4() { On_ClickColor(4); }
    public void On_ClickColor5() { On_ClickColor(5); }
    public void On_ClickColor6() { On_ClickColor(6); }
    public void On_ClickColor7() { On_ClickColor(7); }

    void On_ClickColor(int index) {
        SelectColor(index);
    }

    void SelectColor(int index) {
        Image img = m_colors[ index % m_colors.Length ];
        m_selectedColor = img.color;
        m_pen.transform.position = img.transform.position + new Vector3(10,80,0);
        print($"{m_selectedColor}");
    }
}

