using UnityEngine;

public class DiggingTask : FarmerTaskBase
{
    public override FarmerTask TaskType
    {
        get
        {
            return FarmerTask.Digging;
        }
    }
}
