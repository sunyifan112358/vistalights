﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public enum OilSpillSolution { 
	None,
	Burn,
	Dispersant,
	Skimmers
}

public class OilSpillingAction : MonoBehaviour {

	public OilSpillingController OilSpillingController;
	public MapController MapController;
	public WelfareCounter WelfareCounter;

	public OilSpillSolution solution = OilSpillSolution.None;

	private List<Connection> disconnectedMapConnections = new List<Connection>();

	public Toggle BuringToggle;
	public Toggle DispersantToggle;
	public Toggle SkimmersToggle;

	public VistaLightsLogger logger;

	public double speed;
	public double welfareImpact;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (solution == OilSpillSolution.None) return;
		
		Timer timer = GameObject.Find("Timer").GetComponent<Timer>();

		double oilCleaned = timer.TimeElapsed.TotalSeconds * speed;
		OilSpillingController.Amount -= oilCleaned; 
		if (OilSpillingController.Amount <= 0) {
			OilSpillingController.Amount = 0;
			if (solution == OilSpillSolution.Burn || solution == OilSpillSolution.Skimmers) {
				RestartTraffic();
			}
			solution = OilSpillSolution.None;
			RecoverTrafficSpeed ();

			string content = string.Format ("Oil has been cleaned up");
			GameObject.Find ("NotificationSystem").GetComponent<NotificationSystem> ().Notify (NotificationType.Success, content);
		}

		double welfareChange = welfareImpact * oilCleaned;
		WelfareCounter.ReduceWelfare(welfareChange);
			
	}

	private void SetCleaningSpeed ()
	{
		switch (solution) {
		case OilSpillSolution.Burn:
			speed = 1.0 * OilSpillingController.Amount / 10 / 3600;
			welfareImpact = 1.5 / 10000;
			break;
		case OilSpillSolution.Dispersant:
			speed = 1.0 * OilSpillingController.Amount / 48 / 3600;
			welfareImpact = 1 / 10000;
			break;
		case OilSpillSolution.Skimmers:
			speed = 1.0 * OilSpillingController.Amount / 24 / 3600;
			break;
		}
	}

	public void EnableAllToggles() {
		BuringToggle.interactable = true;
		DispersantToggle.interactable = true;
		SkimmersToggle.interactable = true;
	}

	public void DisableAllToggles() {
		BuringToggle.interactable = false;
		DispersantToggle.interactable = false;
		SkimmersToggle.interactable = false;
	}

	private bool isInOilSpill(Node node) {
		double distance = Math.Pow(Math.Pow(node.X - OilSpillingController.position.x, 2) + Math.Pow(node.Y - OilSpillingController.position.y, 2), 0.5);
		return distance <= OilSpillingController.Radius;
	}

	private void StopTraffic() {
		Map map = MapController.Map;
		foreach (Connection connection in map.connections) {
			if (isInOilSpill(connection.StartNode) || isInOilSpill(connection.EndNode)) {
				disconnectedMapConnections.Add(connection);
				connection.StartNode.RemoveConnection(connection);
				connection.EndNode.RemoveConnection(connection);
			} 
		}

		foreach (Connection connection in disconnectedMapConnections) {
			MapController.RemoveConnection(MapController.GetConnectionGO(connection));
		}

		GameObject.Find("NetworkScheduler").GetComponent<NetworkScheduler>().RequestReschedule();
	}

	private void RestartTraffic() {
		Map map = MapController.Map;
		foreach (Connection connection in disconnectedMapConnections) {
			MapController.CreateConnectionGameObject(connection);
			connection.StartNode.AddConnection(connection);
			connection.EndNode.AddConnection(connection);
			map.AddConnection(connection);
		}
		GameObject.Find("NetworkScheduler").GetComponent<NetworkScheduler>().RequestReschedule();
	}

	public void SlowDownTraffic() {
		Map map = MapController.Map;
		foreach (Connection connection in map.connections) {
			if (isInOilSpill(connection.StartNode) || isInOilSpill(connection.EndNode)) {
				connection.Speed = 2.0;
			} 
		}
		GameObject.Find("NetworkScheduler").GetComponent<NetworkScheduler>().RequestReschedule();
	}

	public void RecoverTrafficSpeed() {
		Map map = MapController.Map;
		foreach (Connection connection in map.connections) {
			if (isInOilSpill(connection.StartNode) || isInOilSpill(connection.EndNode)) {
				connection.Speed = 3.0;
			} 
		}
		GameObject.Find("NetworkScheduler").GetComponent<NetworkScheduler>().RequestReschedule();
	}

	public void Burn() {
		StopTraffic();
		solution = OilSpillSolution.Burn;

		SetCleaningSpeed ();

		DisableAllToggles();

		logger.LogOilCleaning (OilSpillSolution.Burn);
	}

	public void Dispersant() {
		solution = OilSpillSolution.Dispersant;

		SetCleaningSpeed ();

		DisableAllToggles();

		logger.LogOilCleaning (OilSpillSolution.Dispersant);
	}

	public void Skimmers() {
		StopTraffic();

		solution = OilSpillSolution.Skimmers;
		SetCleaningSpeed ();

		DisableAllToggles();

		logger.LogOilCleaning (OilSpillSolution.Skimmers);
	}
}
