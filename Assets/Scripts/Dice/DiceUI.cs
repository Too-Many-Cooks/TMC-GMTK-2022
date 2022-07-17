using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceUI : Singleton<DiceUI>
{
    public RawImage image;
    public DiceContainerGraphic container;
    public new Camera camera;
    public int width = 256;
    public int height = 256;
    public ShooterController shooterController;

    public DieDisplay[] displays;
    
    private readonly HashSet<DieDisplay> _displays = new();
    private readonly Dictionary<DieDisplay, DiceContainerGraphic> _containers = new();
    private int _screenWidth;
    private int _screenHeight;

    [field: SerializeField, HideInInspector] public RenderTexture CameraTexture;

    protected void Awake()
    {
        if (container != null)
            container.gameObject.SetActive(false);

        if(shooterController == null)
        {
            shooterController = FindObjectOfType<PlayerMovement>().GetComponent<ShooterController>();
        }

        if (shooterController != null)
        {
            shooterController.OnReloadDieChanged.AddListener(OnReloadDieChanged);
            shooterController.OnReloadDieRolled.AddListener(OnReloadDieRolled);
        }
    }

    protected void Start()
    {
        CameraTexture = new(width, height, 24);
        camera.targetTexture = CameraTexture;
        
        image.texture = CameraTexture;
        image.color = Color.white;
    }

    protected override bool OnEnable()
    {
        if (!base.OnEnable())
            return false;

        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        
        foreach (DieDisplay display in displays)
            AddDisplay(display);
        
        return true;
    }

    protected override bool OnDisable()
    {
        if (!base.OnDisable())
            return false;
        
        while (_displays.Count > 0)
            RemoveDisplay(_displays.First());

        return true;
    }

    protected void Update()
    {
        if (_screenWidth != Screen.width || _screenHeight != Screen.height)
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            
            foreach (var (display, container) in _containers)
                MoveContainer(display.transform, container);
        }

        foreach (var (display, container) in _containers)
        {
            DieFace face = display.FindFace(camera.transform.localPosition.normalized);
            if (face != null)
                container.label.text = face.name;
        }
    }

    protected void OnSetDie(DieDisplay display, GameObject dieObject)
    {
        dieObject.layer = gameObject.layer;
        
        if (container == null || _containers.ContainsKey(display)) return;

        Transform containerTransform = container.transform;
        Transform displayTransform = display.transform;
        
        DiceContainerGraphic containerInstance = Instantiate(container, containerTransform.position, containerTransform.rotation, containerTransform.parent);
        _containers.Add(display, containerInstance);
        
        Transform instanceTransform = containerInstance.transform;
        instanceTransform.SetSiblingIndex(containerTransform.GetSiblingIndex());
        instanceTransform.localScale = displayTransform.localScale;

        MoveContainer(displayTransform, containerInstance);
    }

    private void OnReloadDieChanged(Die currentDie, int currentDieIndex)
    {
        for(int i = 0; i < displays.Length; i++)
        {
            if(i < shooterController.ReloadDice.Length)
            {
                displays[i].gameObject.SetActive(true);
                AddDisplay(displays[i]);
                int dieIndex = (currentDieIndex + i) % shooterController.ReloadDice.Length;
                displays[i].Die = shooterController.ReloadDice[dieIndex];
            } else
            {
                RemoveDisplay(displays[i]);
                displays[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnReloadDieRolled(ShooterController.ReloadDieRoll roll)
    {
        displays[0].RollToFace(roll.die.GetNormal(roll.dieFaceIndex), roll.weapon.reloadSpeed);
    }

    private void MoveContainer(Transform transform, DiceContainerGraphic container)
    {
        Vector3 viewport = camera.WorldToViewportPoint(transform.position);
        Vector2 size = image.rectTransform.sizeDelta;
        
        viewport.x *= size.x;
        viewport.y *= size.y;
        
        Vector2 position =
                image.rectTransform.anchoredPosition +
                new Vector2(viewport.x, viewport.y - size.y)
            ;

        container.RectTransform.anchoredPosition = position;
        container.gameObject.SetActive(true);
    }

    public void AddDisplay(DieDisplay display)
    {
        if (_displays.Add(display))
            display.OnSetDie += OnSetDie;
    }

    public void RemoveDisplay(DieDisplay display)
    {
        if (_displays.Remove(display))
            display.OnSetDie -= OnSetDie;

        if (!_containers.TryGetValue(display, out var container)) return;
        
        _containers.Remove(display);
        Destroy(container.gameObject);
    }
}
