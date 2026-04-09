 using UnityEngine;

public abstract class ControllerBase
{
    public abstract bool isDone {  get; }
    public abstract void ControllerEnable(DroneUnitBody user);
    public abstract void ControllerDisable(DroneUnitBody user);
}
