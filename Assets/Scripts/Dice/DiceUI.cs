using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiceUI : Singleton<DiceUI>
{
    public RawImage image;
    public Graphic container;
    public new Camera camera;
    public int width = 256;
    public int height = 256;
    
    private readonly HashSet<DieDisplay> _displays = new();
    private readonly Dictionary<DieDisplay, Graphic> _graphics = new();
    private int _screenWidth;
    private int _screenHeight;

    [field: SerializeField, HideInInspector] public RenderTexture CameraTexture;

    protected void Awake()
    {
        if (container != null)
            container.gameObject.SetActive(false);
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
        
        foreach (DieDisplay display in GetComponentsInChildren<DieDisplay>())
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
            
            foreach (var (display, graphic) in _graphics)
                MoveContainer(display.transform, graphic);
        }
    }

    protected void OnSetDie(DieDisplay display, GameObject dieObject)
    {
        dieObject.layer = gameObject.layer;
        
        if (container == null || _graphics.ContainsKey(display)) return;

        Transform containerTransform = container.transform;
        Transform displayTransform = display.transform;
        
        Graphic containerInstance = Instantiate(container, containerTransform.position, containerTransform.rotation, containerTransform.parent);
        _graphics.Add(display, containerInstance);
        
        Transform instanceTransform = containerInstance.transform;
        instanceTransform.SetSiblingIndex(containerTransform.GetSiblingIndex());
        instanceTransform.localScale = displayTransform.localScale;

        MoveContainer(displayTransform, containerInstance);
    }

    private void MoveContainer(Transform transform, Graphic graphic)
    {
        Vector3 viewport = camera.WorldToViewportPoint(transform.position);
        Vector2 size = image.rectTransform.sizeDelta;
        
        viewport.x *= size.x;
        viewport.y *= size.y;
        
        Vector2 position =
                image.rectTransform.anchoredPosition +
                new Vector2(viewport.x, viewport.y - size.y)
            ;

        graphic.rectTransform.anchoredPosition = position;
        graphic.gameObject.SetActive(true);
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

        if (!_graphics.TryGetValue(display, out var graphic)) return;
        
        _graphics.Remove(display);
        Destroy(graphic.gameObject);
    }
}
