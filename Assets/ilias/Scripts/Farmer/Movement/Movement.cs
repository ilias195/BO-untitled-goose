using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
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

        if (WayPoints.points == null || WayPoints.points.Length == 0)
            return;

        target = WayPoints.points[wavePointIndex];
        lastPosition = transform.position;

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
                    8f * Time.deltaTime
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

        if (waypointTask.taskType == FarmerTask.Watering)
        {
            currentTask = new WateringTask();
        }
        else if (waypointTask.taskType == FarmerTask.Rake)
        {
            currentTask = new RakeTask();
        }

        farmerAnimation.PlayTaskAnimation(currentTask.TaskType);


        isWorking = true; // farmer niet lopen
        Invoke(nameof(FinishTask), 2f); // zegt roep FinishTask op na 2sec is de taak klaar
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

        if (wavePointIndex >= WayPoints.points.Length)
        {
            wavePointIndex = 0;
        }

        target = WayPoints.points[wavePointIndex];
    }

}
