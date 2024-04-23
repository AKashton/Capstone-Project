using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] SpriteRenderer borderImage;

    TileManager tileManager;
    Vector3 startLocation;

    void Awake()
    {
        tileManager = GameObject.FindWithTag("TileManager").GetComponent<TileManager>();
    }

    public void ClickTile()
    {
        tileManager.CallTileSwap(gameObject);
    }

    public void AssignStartLocation()
    {
        startLocation = transform.localPosition;
    }

    public float GetDistanceFromStart()
    {
        return Vector3.Distance(transform.localPosition, startLocation);
    }

    public void FadeBorder()
    {
        StartCoroutine("FadeBorderImage", 255);
    }

    IEnumerator FadeBorderImage(int imageAlpha)
    {
        imageAlpha -= 11;

        Color tempColor = borderImage.color;
        tempColor.a = (float)imageAlpha / 255;
        borderImage.color = tempColor;

        yield return new WaitForSeconds(0.1f);

        if (imageAlpha > 0)
            StartCoroutine("FadeBorderImage", imageAlpha);
    }

    public void FadeWhite()
    {
        StartCoroutine("FadeWhiteImage", 0.1f);
    }

    IEnumerator FadeWhiteImage(float fadeTimer)
    {
        yield return new WaitForSeconds(0.232f);
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.black, Color.white, fadeTimer);

        if (fadeTimer < 1)
            StartCoroutine("FadeWhiteImage", 0.1f + fadeTimer);
    }
}
