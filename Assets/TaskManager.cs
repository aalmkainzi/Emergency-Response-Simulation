using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public enum PlayerTask
    {
        ENTER_TRUCK,
        GO_TO_CRASH,
        EXIT_TRUCK,
        TAKE_FOAM,
        EXTINGUISH_FLAMES,
        RESCUE_DRIVERS
    };

    public PlayerTask currentTask = PlayerTask.ENTER_TRUCK;
    public TextMeshProUGUI taskText;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SetTask(PlayerTask newTask)
    {
        currentTask = newTask;
        switch(currentTask)
        {
            case PlayerTask.ENTER_TRUCK:
                taskText.text = "Task: Enter firetruck";
                break;
            case PlayerTask.GO_TO_CRASH:
                taskText.text = "Task: Go to crash site";
                break;
            case PlayerTask.EXIT_TRUCK:
                taskText.text = "Task: Exit truck";
                break;
            case PlayerTask.TAKE_FOAM:
                taskText.text = "Task: Take foam from firetruck storage";
                break;
            case PlayerTask.EXTINGUISH_FLAMES:
                taskText.text = "Task: Extinguish the flames";
                break;
            case PlayerTask.RESCUE_DRIVERS:
                taskText.text = "Task: Help the drivers";
                break;
        }
    }

    public void EnteredTruck()
    {
        if(currentTask == PlayerTask.ENTER_TRUCK)
        {
            SetTask(PlayerTask.GO_TO_CRASH);
        }
    }
}
