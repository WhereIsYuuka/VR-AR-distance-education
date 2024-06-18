using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WhiteBoardMarker : MonoBehaviour
{
    [SerializeField] private Transform markerTip;
    [SerializeField] private int _penSize = 10;

    private Renderer _renderer;
    private Color[] _penColors; //= new Color[5] { Color.black, Color.red, Color.green, Color.blue, Color.yellow };
    private float _tipHeight;
    private WhiteBoard _whiteBoard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _isDrawing;
    private Quaternion _lastTouchRot;
    //private RaycastHit _hit;
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = markerTip.GetComponent<Renderer>();
        _penColors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = markerTip.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        // Physics.Raycast() 方法用于检测射线是否与物体碰撞，如果碰撞则返回 true，否则返回 false。
        if(Physics.Raycast(markerTip.position, transform.up, out var hit, _tipHeight))
        {
            if(hit.transform.CompareTag("WhiteBoard"))
            {
                if(_whiteBoard == null)
                {
                    _whiteBoard = hit.transform.GetComponent<WhiteBoard>();
                }

                // hit.textureCoord 是一个 Vector2 类型的变量，它表示射线在碰撞物体上的坐标。
                _touchPos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);

                // 如果触摸屏幕
                var x = (int)(_touchPos.x * _whiteBoard.textureSize.x - (_penSize / 2));
                var y = (int)(_touchPos.y * _whiteBoard.textureSize.y - (_penSize / 2));

                if(x < 0 || y < 0 || x > _whiteBoard.textureSize.x || y > _whiteBoard.textureSize.y)
                {
                    return;
                }

                if(_isDrawing)
                {
                    // 在两个点之间画线，这样可以使线条更加平滑。
                    _whiteBoard.texture.SetPixels(x, y, _penSize, _penSize, _penColors);

                    // 通过 Lerp() 方法在两个点之间画线
                    for(float f = 0.01f; f < 1.0f; f += 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);

                        _whiteBoard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _penColors);
                    }

                    transform.rotation = _lastTouchRot;
                    
                    _whiteBoard.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _isDrawing = true;
                return;
            }
            
            // var texture = _renderer.material.mainTexture as Texture2D;
            // var pixelUV = hit.textureCoord;
            // pixelUV.x *= texture.width;
            // pixelUV.y *= texture.height;

            // for (int x = 0; x < _penSize; x++)
            // {
            //     for (int y = 0; y < _penSize; y++)
            //     {
            //         texture.SetPixel((int)pixelUV.x + x, (int)pixelUV.y + y, _penColors[x]);
            //     }
            // }
            // texture.Apply();
        }

        _whiteBoard = null;
        _isDrawing = false;
    }
}
