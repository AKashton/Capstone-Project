using MixedReality.Toolkit.SpatialManipulation;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject seaWolf, goldRing, seaWolfEye, winScreen, winSlate, startSlate, seaWolfGameObject, rootMenu;
    [SerializeField] Transform playerTransform;
    [SerializeField] DirectionalIndicator directionalIndicator;
    [SerializeField] float spawnRadius = 3; // 2-5 is probably good value
    [SerializeField] float radiusVariation = 1;
    [SerializeField] float maxLatitudeAngle = 60; // Must be less than 90
    [SerializeField] float maxLongitudeAngle = 180; // Must be less than 180
    [SerializeField] int seaWolvesRequired = 10;
    [SerializeField] float initialLookTime = 2;
    [SerializeField] float timeDecrement = 0.2f;
    [SerializeField] float initialGrowSpeed = 1.1f;
    [SerializeField] float growIncrement = 0.2f;
    [SerializeField] float initialLifeSpan = 5;
    [SerializeField] float lifeSpanDecrease = 0.4f;
    [SerializeField] float lifeSpanIncrease = 0.1f;

    int seaWolvesCaught = 0;
    int numFails = 0;

    void OnEnable()
    {
        startSlate.SetActive(true);
        winSlate.SetActive(false);
    }

    public void StartGame()
    {
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        //seaWolfEye.SetActive(true);
#endif
        SpawnSeawolf();
    }

    public void SpawnSeawolf()
    {
        if (seaWolvesCaught >= seaWolvesRequired)
        {
            Celebrate();
            return;
        }

        float radiusSelection = Random.Range(spawnRadius - radiusVariation, spawnRadius + radiusVariation);
        float latitudeAngle = Random.Range(-maxLatitudeAngle, maxLatitudeAngle);
        float longitudeAngle = Random.Range(-maxLongitudeAngle, maxLongitudeAngle);
        Vector3 spawnLocation = GetSphereSurfaceVector(radiusSelection, latitudeAngle, longitudeAngle);
        spawnLocation += playerTransform.position;

        GameObject newWolf = Instantiate(seaWolf, spawnLocation, Quaternion.identity);
        SeaWolfScript wolfScript = newWolf.GetComponent<SeaWolfScript>();
        directionalIndicator.DirectionalTarget = newWolf.transform;
        newWolf.transform.LookAt(playerTransform);

        float newLookTime = initialLookTime - timeDecrement * seaWolvesCaught;
        float newGrowSpeed = initialGrowSpeed + growIncrement * seaWolvesCaught;
        wolfScript.SetParameters(newLookTime, newGrowSpeed);

        GameObject newRing = Instantiate(goldRing, spawnLocation, Quaternion.identity);
        newRing.transform.localScale = (1 + newLookTime * newGrowSpeed) * Vector3.one;
        newRing.transform.LookAt(playerTransform);
        wolfScript.SetRing(newRing);

        float lifeSpan = initialLifeSpan - lifeSpanDecrease * seaWolvesCaught + lifeSpanIncrease * numFails;
        wolfScript.StartDestructTimer(lifeSpan);
    }

    Vector3 GetSphereSurfaceVector(float inputRadius, float angleB, float angleA) // angleB from y, angleA from x in degrees
    {
        angleB = Mathf.Clamp(angleB, -90, 90);
        angleA = Mathf.Clamp(angleA, -180, 180);

        angleB *= Mathf.PI / 180;
        angleA *= Mathf.PI / 180;

        float xZComponent = inputRadius * Mathf.Cos(angleB);
        float height = Mathf.Sqrt(inputRadius * inputRadius - xZComponent * xZComponent);

        return new Vector3(xZComponent * Mathf.Cos(angleA), Mathf.Sign(angleB) * height, xZComponent * Mathf.Sin(angleA));
    }

    public void IncrementWolvesSeen()
    {
        seaWolvesCaught += 1;
    }

    public void IncrementFails()
    {
        numFails += 1;
    }

    public void Celebrate()
    {
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
        //seaWolfEye.SetActive(false);
#endif
        winSlate.SetActive(true);
        seaWolvesCaught = 0;
        numFails = 0;
    }

    public void RestartGame()
    {
        //seaWolvesCaught = 0;
        //numFails = 0;
        StartGame();
    }

    public void QuitGame()
    {
        rootMenu.SetActive(true);
        seaWolfGameObject.SetActive(false);
        /*
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        */
    }
}
