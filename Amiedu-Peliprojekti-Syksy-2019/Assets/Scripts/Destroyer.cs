using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Destroyer : MonoBehaviour
{
    private int size;
    private int maxSize;
    private Color[] clear;
    private Color[] block;
    private Texture2D _texture;
    private int fpsUpdate;
    private SpriteRenderer tempSprite;
    private float pX, pY, pointX, pointY = 0f;


    void Start()
    {
        maxSize = 120;
        int radius = maxSize * maxSize - 10;
        block = new Color[maxSize * maxSize];
        clear = new Color[maxSize * maxSize];
        int cX = maxSize / 2, cY = maxSize / 2; 
        int y = 0;
        int x = 0;
        for (int i = 0; i < clear.Length; i++)
        {
            if (i > 0 && i % maxSize == 0) y++;
            x = i - (y * maxSize);
            int xIn = Math.Abs((x - cX));
            int yIn = Math.Abs((y - cY));
            int tempValue = xIn * xIn + yIn * yIn;
            int inRadius = (int)Mathf.Sqrt(tempValue);
            if (inRadius == cX - 10)
                clear[i] = new Color(0f, 0f, 0f, 0f);
            else if (inRadius > cX - 7)
            {
                clear[i] = new Color(0f, 0f, 0f, 0.5f);
            }
            else if (inRadius > cX - 10)
                clear[i] = new Color(0f, 0f, 0f, 0.4f);
            
         
        

         
        }
        tempSprite = GetComponent<SpriteRenderer>();
        tempSprite.sprite = CreateSprite(GetComponent<SpriteRenderer>().sprite.texture);
        

    }


    public void ClearFog(Vector2 pos)
    {
        SetPixels(tempSprite.sprite, pos.x, pos.y, clear);
    }

    public static Sprite CreateSprite(Texture2D _texture)
    {
        Texture2D clone = Instantiate(_texture);
        Sprite sprite = Sprite.Create(clone, new Rect(0, 0, clone.width, clone.height), new Vector2(0.5f, 0.5f), 128);
        return sprite;
    }

    public Color[] GetPixels(Sprite _sprite, float pointX, float pointY, int size, bool pixels = false)
    {
        Color[] _data = new Color[size * size];
        float x = pointX;
        float y = pointY;
        if (!pixels)
        {
            x = (pointX + (_sprite.bounds.size.x / 2f)) * _sprite.pixelsPerUnit - size / 2;
            y = (pointY + (_sprite.bounds.size.y / 2f)) * _sprite.pixelsPerUnit - size / 2;
            float maxX = _sprite.rect.size.x - x;
            float maxY = _sprite.rect.size.y - y;
            if (maxX < size) size = (int)maxX;
            if (maxY < size) size = (int)maxY;
        }
        try
        { _data = _sprite.texture.GetPixels((int)x, (int)y, size, size); }
        catch (Exception e)
        {
            Debug.LogException(e);
        }



        return _data;
    }

    public void SetPixels(Sprite _sprite, float pointX, float pointY, Color[] clr)
    {
        int size = Mathf.RoundToInt(Mathf.Sqrt(clr.Length));
        float x = (pointX + (_sprite.bounds.size.x / 2f)) * _sprite.pixelsPerUnit;
        float y = (pointY + (_sprite.bounds.size.y / 2f)) * _sprite.pixelsPerUnit;

        x = (x + _sprite.bounds.size.x * _sprite.pixelsPerUnit / 2f * (transform.localScale.x - 1)) / transform.localScale.x - size / 2;
        y = (y + _sprite.bounds.size.y * _sprite.pixelsPerUnit / 2f * (transform.localScale.y - 1)) / transform.localScale.y - size / 2;
        //float maxX = _sprite.rect.size.x - x;
        //float maxY = _sprite.rect.size.y - y;
        //if (maxX < size) size = (int)maxX;
        //if (maxY < size) size = (int)maxY;
        var test = (x + _sprite.bounds.size.x  * _sprite.pixelsPerUnit / 2f * (transform.localScale.x - 1)) / transform.localScale.x;
        _sprite.texture.SetPixels((int)x, (int)y, size, size, clr);
        _sprite.texture.Apply();

    }
    public void Explode(GameObject obj, float pointX, float pointY, int size, Vector2 pos)
    {
        Sprite _sprite = obj.GetComponent<SpriteRenderer>().sprite;
        Collider2D _col = obj.GetComponent<Collider2D>();
        float scaleX = obj.transform.localScale.x;
        float scaleY = obj.transform.localScale.y;
        pointX /= scaleX;
        pointY /= scaleY;

        if (obj.transform.eulerAngles.z != 0f) AdjustRotation(obj.transform.eulerAngles.z, ref pointX, ref pointY);
        //Debug.Log("AFTER: " + pointX + "   " + pointY);
        pX = Mathf.Clamp(((pointX + (_sprite.bounds.size.x / 2f)) * _sprite.pixelsPerUnit - size / 2), 0f, Mathf.Infinity);
        pY = Mathf.Clamp(((pointY + (_sprite.bounds.size.y / 2f)) * _sprite.pixelsPerUnit - size / 2), 0f, Mathf.Infinity);

        float maxX = _sprite.rect.size.x - pX;
        float maxY = _sprite.rect.size.y - pY;


        if (maxX < size) size = (int)maxX;
        if (maxY < size) size = (int)maxY;

        //size = Mathf.RoundToInt((float)size / scaleX);
        clear = new Color[size * size];


        block = _sprite.texture.GetPixels((int)pX, (int)pY, size, size);
        float offset = 0f - ((size / 100f) / 2f);
        for (int y = 0; y < size; y += 2)
        {
            for (int x = 0; x < size; x += 3)
            {
                Color pixel = block[x + (y * size)];
                if (pixel.a > 0f)
                {
                    //GameObject _block = Pooler.p.Spawn("Particle", new Vector2(pointX + (offset + (x / 100f)), pointY + (offset + (y / 100f))), pixel);
                    //GameObject _block = Instantiate(particle);
                    ////_block.transform.position = new Vector2(pointX + (offset + (x / 100f)), pointY + (offset + (y / 100f)));
                    //_block.transform.position = pos;
                    //_block.GetComponent<SpriteRenderer>().color = pixel;
                    //_block.transform.localScale = new Vector2(0.03f, 0.03f);
                    //float yForce = UnityEngine.Random.Range(-150f, 150f);
                    //float xForce = UnityEngine.Random.Range(-150f, 150f);
                    ////_block.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                    //_block.GetComponent<Rigidbody2D>().AddForce(Vector2.up * yForce + Vector2.right * xForce);
                    //Destroy(_block, 2f);
                }
            }
        }
        _sprite.texture.SetPixels((int)pX, (int)pY, size, size, clear);
        _sprite.texture.Apply();
        //Destroy(obj.GetComponent<PolygonCollider2D>());
        //AddCollider(obj);


    }
    void AdjustRotation(float angle, ref float pointX, ref float pointY)
    {
        float newY = pointY * Mathf.Cos(angle * Mathf.Deg2Rad) - pointX * Mathf.Sin(angle * Mathf.Deg2Rad);
        pointX = pointY * Mathf.Sin(angle * Mathf.Deg2Rad) + pointX * Mathf.Cos(angle * Mathf.Deg2Rad);
        pointY = newY;


    }
}
