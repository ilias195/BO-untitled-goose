using UnityEngine;

public abstract class FarmerTaskBase
{
    public abstract FarmerTask TaskType { get; }//get zorgt ervoor dat de code weet welke die is.
    
}
public enum FarmerTask
{
    None = 0,
    Watering,//geeft water
    Rake//Harken
   
}
