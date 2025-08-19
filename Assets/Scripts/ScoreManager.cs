using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    float startTime;

    public float[] durations;

    public static ScoreManager instance;
    TextMeshProUGUI message;
    void Start()
    {
        if(instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        startTime = Time.realtimeSinceStartup;
        message = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void EndSimulation()
    {
        message.gameObject.SetActive(true);

        float endTime = Time.realtimeSinceStartup;
        float duration = endTime - startTime;

        if (duration <= durations[0])
        {
            message.text = "Grade: A+";
        }
        else if(duration <= durations[1])
        {
            message.text = "Grade: A-";
        }
        else if (duration <= durations[2])
        {
            message.text = "Grade: B+";
        }
        else if (duration <= durations[3])
        {
            message.text = "Grade: B-";
        }
        else if (duration <= durations[4])
        {
            message.text = "Grade: C+";
        }
        else if (duration <= durations[5])
        {
            message.text = "Grade: C-";
        }
        else if (duration <= durations[6])
        {
            message.text = "Grade: D+";
        }
        else if (duration <= durations[7])
        {
            message.text = "Grade: D-";
        }
        else
        {
            message.text = "Grade: F";
        }

        Invoke(nameof(LoadMainMenu), 5.0f);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    void Update()
    {
        
    }


}
