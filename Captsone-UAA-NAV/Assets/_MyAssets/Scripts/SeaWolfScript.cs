using System.Collections;
using UnityEngine;

public class SeaWolfScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color regular, warning;
    

    GameManager gameManager;
    GameObject goldRing;
    Color startColor;

    float maxLookTime = 2;
    float growSpeed = 1.1f;
    float selfDestructTime = 5;

    bool isLooking = false;
    float lookTimeElapsed = 0;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        if (isLooking)
        {
            lookTimeElapsed += Time.deltaTime;
            transform.localScale = Vector3.one * (1 + growSpeed * lookTimeElapsed);
        }

        if (lookTimeElapsed > maxLookTime)
            SeaWolfCaught();
    }

    public void SetParameters(float lookTime, float speed)
    {
        maxLookTime = lookTime;
        growSpeed = speed;
    }

    public void SetRing(GameObject inputObject)
    {
        goldRing = inputObject;
    }

    public void StartDestructTimer(float wolfLifeSpan)
    {
        selfDestructTime = wolfLifeSpan;
        StartCoroutine(StartSelfDestruct2(selfDestructTime, 0));
    }

    public void LookAtAction()
    {
        StopAllCoroutines();
        startColor = spriteRenderer.color;
        StartCoroutine(ReturnToNormal(selfDestructTime / 4, 0));
        isLooking = true;
    }

    public void LookAwayAction()
    {
        StopAllCoroutines();
        isLooking = false;
        StartCoroutine(StartSelfDestruct2(selfDestructTime, 0));
    }

    void SeaWolfCaught()
    {
        StopAllCoroutines();
        gameManager.IncrementWolvesSeen();
        gameManager.SpawnSeawolf();
        Destroy(goldRing);
        Destroy(gameObject);
    }

    IEnumerator StartSelfDestruct(float destructTime)
    {
        yield return new WaitForSeconds(0.6f * destructTime);
        spriteRenderer.color = warning;
        yield return new WaitForSeconds(0.1f * destructTime);
        spriteRenderer.color = regular;
        yield return new WaitForSeconds(.15f * destructTime);
        spriteRenderer.color = warning;
        yield return new WaitForSeconds(0.075f * destructTime);
        spriteRenderer.color = regular;
        yield return new WaitForSeconds(0.025f * destructTime);
        spriteRenderer.color = warning;
        yield return new WaitForSeconds(0.025f * destructTime);
        spriteRenderer.color = regular;
        yield return new WaitForSeconds(0.025f * destructTime);

        SelfDestruct();
    }

    IEnumerator StartSelfDestruct2(float destructTime, float elapsedColorIncrement)
    {
        elapsedColorIncrement += 0.05f;
        spriteRenderer.color = Color.Lerp(Color.white, Color.red, elapsedColorIncrement);
        yield return new WaitForSeconds(0.05f * destructTime);

        if (elapsedColorIncrement < 1)
            StartCoroutine(StartSelfDestruct2(selfDestructTime, elapsedColorIncrement));
        else
            SelfDestruct();
    }

    IEnumerator ReturnToNormal(float recoverTime, float elapsedColorIncrement)
    {
        if (spriteRenderer.color == Color.white)
            yield break;

        elapsedColorIncrement += 0.05f;
        spriteRenderer.color = Color.Lerp(startColor, Color.white, elapsedColorIncrement);
        yield return new WaitForSeconds(0.05f * recoverTime);

        StartCoroutine(ReturnToNormal(recoverTime, elapsedColorIncrement));
    }

    void SelfDestruct()
    {
        gameManager.IncrementFails();
        gameManager.SpawnSeawolf();
        Destroy(goldRing);
        Destroy(gameObject);
    }
}
