﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DecisionProgressBar : MonoBehaviour {

	public RoundManager roundManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		double percentage = 0;
		if (roundManager.phase == GamePhase.Decision) {
			DateTime currentTime = DateTime.Now;
			DateTime decisionStartTime = roundManager.DecisionPhaseStartTime;
			TimeSpan decisionTimeLimit = roundManager.DecisionTimeLimit;

			percentage = (currentTime - decisionStartTime).TotalSeconds / decisionTimeLimit.TotalSeconds;
			if (percentage > 100) {
				percentage = 100;
			} else if (percentage < 0) {
				percentage = 0;
			}
		}

		gameObject.transform.localScale = new Vector3((float)percentage, 1, 1);

		if (percentage < 0.8) {
			gameObject.GetComponent<Image> ().color = new Color ((float)0.13, (float)0.82, (float)0.29);
		} else {
			gameObject.GetComponent<Image> ().color = new Color ((float)0.82, (float)0.16, (float)0.067);
		}
	}
}
