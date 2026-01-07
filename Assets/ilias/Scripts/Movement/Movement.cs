using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private Transform target;
     private int wavePointIndex = 0;

    private FarmerTaskBase currentTask;
    private bool isWorking = false;

    private void Start()
    {
        if (WayPoints.points == null || WayPoints.points.Length == 0) //checken of mijn waypint bestaat
        {
            Debug.Log("no waypoints found");
                return;
        }
        target = WayPoints.points[wavePointIndex]; //target kiezen
    }

    private void Update()
    {

        if (isWorking)
            return;

        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * _speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            StartTaskWayPoint();
        }
    }

    void StartTaskWayPoint()
    {
        WaypointTask waypointTask = target.GetComponent<WaypointTask>();//

        //als er geen task is ga naar de volgende Task point en kijk opnieuw;
        if (waypointTask == null || waypointTask.taskType == FarmerTask.None)
        {
            GetNextWayPoint();
            return;
        }

        if (waypointTask.taskType == FarmerTask.Watering)
        {
            currentTask = new WateringTask();
        }
        else if (waypointTask.taskType == FarmerTask.Rake)
        {
            currentTask = new RakeTask();
        }

        isWorking = true; // farmer niet lopen
        Invoke(nameof(FinishTask), 2f); // zegt roep FinishTask op na 2sec is de taak klaar
    }

    void FinishTask()//taak klaar volgende WayPoint
    {
        isWorking = false;
        currentTask = null;

        GetNextWayPoint();
    }

    void GetNextWayPoint()
    {
        wavePointIndex++;

        if (wavePointIndex >= WayPoints.points.Length)
        {
            wavePointIndex = 0;
        }

        target = WayPoints.points[wavePointIndex];
    }

}
