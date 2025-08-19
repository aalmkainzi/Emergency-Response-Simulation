using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public enum PlayerTask
    {
        ENTER_TRUCK,
        TURN_SIREN_ON,
        GO_TO_CRASH,
        TAKE_FOAM,
        EXTINGUISH_FLAMES,
        RESCUE_DRIVERS
    };

    bool inMissionArea = false;

    public PlayerTask currentTask = PlayerTask.ENTER_TRUCK;
    public TextMeshProUGUI taskText;

    public static TaskManager instance;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    void SetTask(PlayerTask newTask)
    {
        currentTask = newTask;
        switch(currentTask)
        {
            case PlayerTask.ENTER_TRUCK:
                taskText.text = "Task: Enter firetruck";
                break;
            case PlayerTask.TURN_SIREN_ON:
                taskText.text = "Task: Turn siren on";
                break;
            case PlayerTask.GO_TO_CRASH:
                taskText.text = "Task: Go to crash site";
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
            SetTask(PlayerTask.TURN_SIREN_ON);
        }
    }

    public void ExitedTruck()
    {
        if(inMissionArea && currentTask == PlayerTask.GO_TO_CRASH)
        {
            SetTask(PlayerTask.TAKE_FOAM);
        }
    }

    public void OnSiren()
    {
        if (currentTask == PlayerTask.TURN_SIREN_ON)
        {
            SetTask(PlayerTask.GO_TO_CRASH);
        }
    }

    public void OnEnterMissionArea()
    {
        inMissionArea = true;
    }

    public void OnTakeFoam()
    {
        if(currentTask == PlayerTask.TAKE_FOAM)
        {
            SetTask(PlayerTask.EXTINGUISH_FLAMES);
        }
    }
    
    public void OnReturnFoam()
    {
        if(currentTask == PlayerTask.EXTINGUISH_FLAMES)
        {
            SetTask(PlayerTask.TAKE_FOAM);
        }
    }

    public void OnExtinguishFire()
    {
        if(Flammable.nbOnFire == 0 && currentTask == PlayerTask.EXTINGUISH_FLAMES)
        {
            SetTask(PlayerTask.RESCUE_DRIVERS);
        }
    }
}
