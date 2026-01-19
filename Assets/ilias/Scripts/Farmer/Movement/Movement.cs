using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private WayPoints wayPoints;

    [SerializeField] private float _speed = 1f;

    private Transform target;
     private int wavePointIndex = 0;

    private FarmerTaskBase currentTask;
    private bool isWorking = false;

    private FarmerAnimation farmerAnimation;
    private Vector3 lastPosition;



    private void Start()
        {
        farmerAnimation = GetComponent<FarmerAnimation>();

        if (farmerAnimation == null)
        {
            Debug.LogError("FarmerAnimation NOT found on this object!");
        }

        if (wayPoints == null || wayPoints.Points.Count == 0)
        {
            Debug.LogError("No WayPoints assigned!");
            return;
        }

        target = wayPoints.Points[wavePointIndex];

    }



    private void Update()
    {
        if (isWorking)
        {
            farmerAnimation.SetMoving(false);
            return;
        }

        Vector3 direction = target.position - transform.position;

        // Zolang we niet bij het waypoint zijn  lopen
        if (direction.magnitude > 0.4f)
        {
            farmerAnimation.SetMoving(true);

            transform.Translate(direction.normalized * _speed * Time.deltaTime, Space.World);

            // draaien
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    5f * Time.deltaTime
                );
            }
        }
        else
        {
            farmerAnimation.SetMoving(false);
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

        if (waypointTask.taskType == FarmerTask.Digging)
        {
            currentTask = new DiggingTask();
        }
        else if (waypointTask.taskType == FarmerTask.Rake)
        {
            currentTask = new RakeTask();
        }

        farmerAnimation.PlayTaskAnimation(currentTask.TaskType);


        isWorking = true; // farmer niet lopen
        Invoke(nameof(FinishTask), 5f); // zegt roep FinishTask op na 2sec is de taak klaar
    }

    void FinishTask()//taak klaar volgende WayPoint
    {
        farmerAnimation.StopTaskAnimation();

        isWorking = false;
        currentTask = null;

        GetNextWayPoint();
    }

    void GetNextWayPoint()
    {
        wavePointIndex++;

        if (wavePointIndex >= wayPoints.Points.Count)
        {
            wavePointIndex = 0;
        }

        target = wayPoints.Points[wavePointIndex];
    }

}
