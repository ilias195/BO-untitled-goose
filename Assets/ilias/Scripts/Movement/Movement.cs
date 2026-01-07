using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private Transform target;
     private int wavePointIndex = 0;

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
        Vector3 direction = target.position - transform.position; //als je van A naar B wilt gaan.
        transform.Translate(direction.normalized *_speed * Time.deltaTime,Space.World);


        if(Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWayPoint();
        }
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
