using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Gündüz süresi dakika cinsinden (15 dk = 1 gün)")]
    public float dayLengthInMinutes = 15f;  
    [Range(0, 24)]
    public float currentTime = 8f; 

    [Header("Skyboxes")]
    public Material morningSkybox;
    public Material noonSkybox;
    public Material afternoonSkybox;
    public Material sunsetSkybox;
    public Material nightSkybox;

    [Header("Sun Light")]
    public Light sunLight;
    public Color morningColor = new Color(1f, 0.85f, 0.6f);
    public Color noonColor = Color.white;
    public Color afternoonColor = new Color(1f, 0.85f, 0.7f);
    public Color sunsetColor = new Color(1f, 0.5f, 0.3f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);

    [Header("Sun Intensity")]
    public float minIntensity = 0.05f;
    public float maxIntensity = 1f;

    void Update()
    {
        // Zaman ilerlesin
        float deltaTimeHours = (Time.deltaTime / 60f) * (24f / dayLengthInMinutes);
        currentTime += deltaTimeHours;
        if (currentTime >= 24f) currentTime -= 24f;

        UpdateSun();
        UpdateSkybox();
    }

    void UpdateSun()
    {
        if (sunLight == null) return;

        float t = currentTime / 24f;
        sunLight.transform.rotation = Quaternion.Euler(new Vector3(t * 360f - 90f, 170f, 0));

        if (currentTime >= 6 && currentTime < 12)
        {
            sunLight.color = morningColor;
            sunLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, (currentTime - 6) / 6f);
        }
        else if (currentTime >= 12 && currentTime < 15)
        {
            sunLight.color = noonColor;
            sunLight.intensity = maxIntensity;
        }
        else if (currentTime >= 15 && currentTime < 18)
        {
            sunLight.color = afternoonColor;
            sunLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, (currentTime - 15) / 3f);
        }
        else if (currentTime >= 18 && currentTime < 20)
        {
            sunLight.color = sunsetColor;
            sunLight.intensity = Mathf.Lerp(minIntensity, minIntensity * 0.5f, (currentTime - 18) / 2f);
        }
        else
        {
            sunLight.color = nightColor;
            sunLight.intensity = minIntensity;
        }
    }

    void UpdateSkybox()
    {
        if (currentTime >= 6 && currentTime < 12) RenderSettings.skybox = morningSkybox;
        else if (currentTime >= 12 && currentTime < 15) RenderSettings.skybox = noonSkybox;
        else if (currentTime >= 15 && currentTime < 18) RenderSettings.skybox = afternoonSkybox;
        else if (currentTime >= 18 && currentTime < 20) RenderSettings.skybox = sunsetSkybox;
        else RenderSettings.skybox = nightSkybox;
    }

    
    public void SleepToMorning()
    {
        currentTime = 6f;
        UpdateSun();
        UpdateSkybox();
    }
}
