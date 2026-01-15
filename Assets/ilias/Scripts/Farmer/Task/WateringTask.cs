using UnityEngine;

public class WateringTask : FarmerTaskBase
{
    public override FarmerTask TaskType
    {
        get
        {
            return FarmerTask.Watering;
        }
    }
}
