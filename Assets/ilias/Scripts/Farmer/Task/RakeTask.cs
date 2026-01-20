using UnityEngine;

public class RakeTask : FarmerTaskBase
{
    public override FarmerTask TaskType
    {
        get
        {
            return FarmerTask.Rake;
        }
    }


}
