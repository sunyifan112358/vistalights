﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipListController : MonoBehaviour {

	public GameObject entryPrefab;
	public List<ShipListEntryController> entries = new List<ShipListEntryController>();
	public GameObject listHeader;
	public GameObject headerOldPriority;
	public RoundManager roundManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject AddShip(ShipController shipController) {
		GameObject entryGO = GameObject.Instantiate(entryPrefab) as GameObject;
		ShipListEntryController entryController = entryGO.GetComponent<ShipListEntryController>();
		entryController.shipController = shipController;
		entryController.shipListController = this;
		entryGO.transform.SetParent(this.transform);

		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
			rectTransform.sizeDelta.y + 30);
        int sameWaitingShipCount = 0;
        int sameShipCount = 0;
        foreach (ShipListEntryController s in entries)
        {
            if (entryController.type.text == s.type.text)
            {
                sameShipCount++;
                if (s.status.text == ShipStatus.Waiting.ToString())
                {
                    sameWaitingShipCount++;
                }
            }
        }
        if (sameShipCount > 4 && sameWaitingShipCount >= sameShipCount/2) {
            roundManager.notificationSystem.Notify(NotificationType.Information, "Prioritize industries that are unloading significantly less");
        }
        entryController.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        entries.Add (entryController);
        /*
		if (roundManager.phase == GamePhase.Decision) {
			entryController.ShowNewPriority ();
		}
		*/

        return entryGO;
	}

	public void RemoveShip(ShipController shipController) {
		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
			rectTransform.sizeDelta.y - 30);
		
		foreach (ShipListEntryController entry in entries) {
			if (entry.shipController == shipController) {
				Destroy(entry.gameObject);
				entries.Remove (entry);
				return;
			} 
		}
	}

	public void ShowNewPriority() {
		foreach (ShipListEntryController entry in entries) {
			entry.ShowNewPriority();
		}

		RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
		Vector2 sizeDelta = rectTransform.sizeDelta;
		rectTransform.sizeDelta = new Vector2 (sizeDelta.x + 40, sizeDelta.y);

		ShowOldPriorityInHeader ();

	}

	void ShowOldPriorityInHeader ()
	{
		headerOldPriority.SetActive (true);
		listHeader.transform.Find ("Status").Translate (new Vector3 (40, 0, 0));
		listHeader.transform.Find ("Name").Translate (new Vector3 (40, 0, 0));
		listHeader.transform.Find ("Type").Translate (new Vector3 (40, 0, 0));
		listHeader.transform.Find ("Amount").Translate (new Vector3 (40, 0, 0));
		listHeader.transform.Find ("Value").Translate (new Vector3 (40, 0, 0));
		listHeader.transform.Find ("Due Time").Translate (new Vector3 (40, 0, 0));
		listHeader.transform.Find ("ETA").Translate (new Vector3 (40, 0, 0));
		RectTransform rectTransform = listHeader.GetComponent<RectTransform> ();
		Vector2 sizeDelta = rectTransform.sizeDelta;
		rectTransform.sizeDelta = new Vector2 (sizeDelta.x + 40, sizeDelta.y);
	}

	public void HideNewPriority() {
		foreach (ShipListEntryController entry in entries) {
			entry.UpdatePriority ();
			entry.HideNewPriority();
		}

		if (headerOldPriority.activeSelf) {
			RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
			Vector2 sizeDelta = rectTransform.sizeDelta;
			rectTransform.sizeDelta = new Vector2 (sizeDelta.x - 40, sizeDelta.y);
		}

		HideOldPriorityInHeader ();

	}

	void HideOldPriorityInHeader ()
	{
		if (headerOldPriority.activeSelf) {
			headerOldPriority.SetActive (false);
			listHeader.transform.Find ("Status").Translate (new Vector3 (-40, 0, 0));
			listHeader.transform.Find ("Name").Translate (new Vector3 (-40, 0, 0));
			listHeader.transform.Find ("Type").Translate (new Vector3 (-40, 0, 0));
			listHeader.transform.Find ("Amount").Translate (new Vector3 (-40, 0, 0));
			listHeader.transform.Find ("Value").Translate (new Vector3 (-40, 0, 0));
			listHeader.transform.Find ("Due Time").Translate (new Vector3 (-40, 0, 0));
			listHeader.transform.Find ("ETA").Translate (new Vector3 (-40, 0, 0));
			RectTransform rectTransform = listHeader.GetComponent<RectTransform> ();
			Vector2 sizeDelta = rectTransform.sizeDelta;
			rectTransform.sizeDelta = new Vector2 (sizeDelta.x - 40, sizeDelta.y);
		}
	}

	public void UpdateAllPriorityInput() {
		foreach (ShipListEntryController entry in entries) {
			entry.UpdatePriorityInput();
		}
	}
}
