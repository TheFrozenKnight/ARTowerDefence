using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

	public Color hoverColor;
	public Color notEnoughMoneyColor;
    public Vector3 positionOffset;
    public GameObject gameContainer;
	private Canvas canvas;

	[HideInInspector]
	public GameObject turret;
	[HideInInspector]
	public TurretBlueprint turretBlueprint;
	[HideInInspector]
	public bool isUpgraded = false;
	
	private Renderer rend;
	private Color startColor;
	
	BuildManager buildManager;

	void Start ()
	{
		canvas = GetComponentInChildren<Canvas>();
		canvas.worldCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
		rend = GetComponent<Renderer>();
		startColor = rend.material.color;
		gameContainer = GameObject.FindGameObjectWithTag("GameContainer");
		buildManager = BuildManager.instance;
		buildManager.nodeUI.gameObject.GetComponentInChildren<Canvas>().worldCamera = canvas.worldCamera;
	}

	public Vector3 GetBuildPosition ()
	{
		return transform.position + positionOffset;
	}
    private void Update()
    {

	}
    
    void OnMouseDown ()
	{
		
	}
    public void clicked()
    {
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (turret != null)
		{
			buildManager.SelectNode(this);
			return;
		}

		if (!buildManager.CanBuild)
			return;

		BuildTurret(buildManager.GetTurretToBuild());
	}

    void BuildTurret (TurretBlueprint blueprint)
	{
		if (PlayerStats.Money < blueprint.cost)
		{
			Debug.Log("Not enough money to build that!");
			return;
		}

		PlayerStats.Money -= blueprint.cost;

		GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity,gameContainer.transform);
		turret = _turret;

		turretBlueprint = blueprint;

		GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity, gameContainer.transform);
		Destroy(effect, 5f);

		Debug.Log("Turret built!");
	}

	public void UpgradeTurret ()
	{
		if (PlayerStats.Money < turretBlueprint.upgradeCost)
		{
			Debug.Log("Not enough money to upgrade that!");
			return;
		}

		PlayerStats.Money -= turretBlueprint.upgradeCost;

		//Get rid of the old turret
		Destroy(turret);

		//Build a new one
		GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity, gameContainer.transform);
		turret = _turret;

		GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity, gameContainer.transform);
		Destroy(effect, 5f);

		isUpgraded = true;

		Debug.Log("Turret upgraded!");
	}

	public void SellTurret ()
	{
		PlayerStats.Money += turretBlueprint.GetSellAmount();

		GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity, gameContainer.transform);
		Destroy(effect, 5f);

		Destroy(turret);
		turretBlueprint = null;
	}
	/*
	void OnMouseEnter ()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (!buildManager.CanBuild)
			return;

		if (buildManager.HasMoney)
		{
			rend.material.color = hoverColor;
		} else
		{
			rend.material.color = notEnoughMoneyColor;
		}

	}

	void OnMouseExit ()
	{
		rend.material.color = startColor;
    }
	*/
}
