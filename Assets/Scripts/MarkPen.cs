using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkPen : MonoBehaviour
{
    private Texture texture;    //原图
    public RenderTexture cachedTexture;//缓存图
    RenderTexture currentRenderTexture;//当前渲染纹理

    public float brushSize = 0.01f;
    public float brushMaxSize;   //画笔最大尺寸
    public Color brushColor = Color.red;

    private Material effectMaterial;    //特效材质
    private Material renderMaterial;    //原始材质

    public Transform penTip;    //笔尖
    public Transform board;    //画板
    public float rayDistance = 0.05f;    //射线距离

    private Vector2 lastUV;
    private bool isDrawing = false;
        // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(penTip.position, transform.forward);
        //Debug
        Debug.DrawRay(penTip.position, transform.forward * rayDistance, Color.red);

        //检测画板
        if(Physics.Raycast(ray, out RaycastHit hit, rayDistance, LayerMask.GetMask("Board")))
        {
            if(hit.distance < 0.05f && hit.collider.gameObject == board.gameObject)
            {
                Debug.Log("Is Drawing");
                //如果画笔在画板上，且没有在画，则开始画
                if(!isDrawing)
                {
                    isDrawing = true;
                    lastUV = hit.textureCoord2;
                }
                brushSize = Mathf.Clamp((0.05f / hit.distance) * 0.001f, 0, brushMaxSize);
                RenderBurshToBoard(hit);
                lastUV = hit.textureCoord2;

            }
            else
            {
                isDrawing = false;
                // return;
            }
        }
        else
        {
            isDrawing = false;
        }
    }

    private void RenderBurshToBoard(RaycastHit hit)
    {
        Vector2 dir = hit.textureCoord2 - lastUV;

        if(Vector3.SqrMagnitude(dir) > brushSize * brushSize)
        {
            int len = Mathf.CeilToInt(dir.magnitude / brushSize);
            for(int i = 0; i < len; i++)
            {
                RenderToMatTex(lastUV + dir.normalized * brushSize * i);
                // RenderToMatTex(Vector2.Lerp(lastUV, hit.textureCoord2, i / len));
            }
        }

        RenderToMatTex(hit.textureCoord2);
    }
    private void RenderToMatTex(Vector2 uv)
    {
        //将当前渲染纹理赋值给缓存纹理
        effectMaterial.SetVector("_BrushPos", new Vector4(uv.x, uv.y, lastUV.x, lastUV.y));
        effectMaterial.SetColor("_BrushColor", brushColor);
        effectMaterial.SetFloat("_BrushSize", brushSize);
        Graphics.Blit(cachedTexture, currentRenderTexture, effectMaterial);
        //将当前渲染纹理赋值给原始纹理
        renderMaterial.SetTexture("_MainTex", currentRenderTexture);
        // renderMaterial.mainTexture = currentRenderTexture;
        //将当前渲染纹理赋值给缓存纹理
        Graphics.Blit(currentRenderTexture, cachedTexture);
    }

    private void Init()
    {
        // brushMaxSize = brushSize;
        effectMaterial = new Material(Shader.Find("Brush/MarkPenEffect"));
        Debug.Log("effectMaterial" + effectMaterial);
        Material boardMat = board.GetComponent<MeshRenderer>().material;
        texture = boardMat.mainTexture; 

        renderMaterial = boardMat;

        cachedTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
        //将原图拷贝到缓存图
        Graphics.Blit(texture, cachedTexture);
        //将缓存图赋值给原图
        renderMaterial.SetTexture("_MainTex", cachedTexture);
        // renderMaterial.mainTexture = cachedTexture;

        currentRenderTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
    }
}
