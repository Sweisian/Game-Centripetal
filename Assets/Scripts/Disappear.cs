using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour {
	public float timeUntilDestroy = 2.0f;
	float m_timePassed = 0.0f;

	void Update () {
		m_timePassed += Time.deltaTime;
		if (m_timePassed > timeUntilDestroy) {
			Destroy (gameObject);
		}
	}
}
