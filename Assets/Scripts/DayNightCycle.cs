using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {
	
	public float timeSpeed = 0.6f;

	void Update() {
		transform.Rotate(0f, 0f, Time.deltaTime * Time.timeScale * timeSpeed);
	}

	public float HourOfDay {
		get {
			return (transform.rotation.eulerAngles.z / 360f) * 24f;
		}
		set {
			Quaternion q = transform.rotation;
			q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, (value / 24f) * 360f);
			transform.rotation = q;
		}
	}

}
