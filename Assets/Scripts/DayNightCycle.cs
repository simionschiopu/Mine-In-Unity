using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float TimeSpeed = 0.6f;

    private void Update()
    {
        transform.Rotate(0f, 0f, Time.deltaTime * Time.timeScale * TimeSpeed);
    }

    public float HourOfDay
    {
        get { return (transform.rotation.eulerAngles.z / 360f) * 24f; }
        set
        {
            var rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, value / 24f * 360f);
            transform.rotation = rotation;
        }
    }
}
