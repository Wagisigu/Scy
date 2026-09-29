using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _healthBarPrefab;
    [SerializeField] private Canvas _canvas;
    private Image _healthBarFill;
    private GameObject _healthBarInstance;

    [Header("Positioning")]
    [SerializeField] private Vector3 _worldOffset = new Vector3(0, 2, 0);
    [SerializeField] private bool _useWorldSpace = false;

    [Header("Behavior")]
    [SerializeField] private bool _hideAtFullHealth = true;
    [SerializeField] private bool _hideOnDeath = true;
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("Visual Feedback")]
    [SerializeField] private bool _useColorGradient = true;
    [SerializeField] private Gradient _healthGradient;

    private Health _health;
    private Camera _mainCamera;
    private float _targetFillAmount;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _mainCamera = Camera.main;
        InitializeDefaultGradient();
    }

    private void Start()
    {
        InstantiateHealthBar();
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnHealthPercentChanged.AddListener(OnHealthPercentChanged);
            _health.OnDeath.AddListener(OnDeath);
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnHealthPercentChanged.RemoveListener(OnHealthPercentChanged);
            _health.OnDeath.RemoveListener(OnDeath);
        }
    }

    private void OnDestroy()
    {
        if (_healthBarInstance != null)
        {
            Destroy(_healthBarInstance);
        }
    }

    private void InitializeDefaultGradient()
    {
        // Always create a proper gradient if one doesn't exist or is invalid
        if (_healthGradient == null || _healthGradient.colorKeys.Length == 0)
        {
            _healthGradient = new Gradient();
        }

        // Set default health gradient: red -> yellow -> green
        _healthGradient.SetKeys(
            new GradientColorKey[] 
            { 
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.yellow, 0.5f),
                new GradientColorKey(Color.green, 1f)
            },
            new GradientAlphaKey[] 
            { 
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
    }

    private void InstantiateHealthBar()
    {
        if (_healthBarPrefab == null)
        {
            Debug.LogWarning($"HealthBarUI on {gameObject.name}: No health bar prefab assigned.", this);
            return;
        }

        // Determine parent canvas
        Canvas targetCanvas = _canvas != null ? _canvas : FindObjectOfType<Canvas>();
        if (targetCanvas == null)
        {
            Debug.LogError($"HealthBarUI on {gameObject.name}: No canvas found. Cannot create health bar.", this);
            return;
        }

        // Instantiate
        _healthBarInstance = Instantiate(_healthBarPrefab.gameObject, targetCanvas.transform);
        _healthBarInstance.name = $"{gameObject.name}_HealthBar";

        // Find fill image - look for "Fill" child specifically
        Transform fillTransform = _healthBarInstance.transform.Find("Fill");
        if (fillTransform != null)
        {
            _healthBarFill = fillTransform.GetComponent<Image>();
        }

        // If not found, search all children and pick the one that's NOT named "Background"
        if (_healthBarFill == null)
        {
            Image[] allImages = _healthBarInstance.GetComponentsInChildren<Image>();

            foreach (Image img in allImages)
            {
                if (!img.gameObject.name.Contains("Background") && !img.gameObject.name.Contains("BG"))
                {
                    _healthBarFill = img;
                    break;
                }
            }
        }

        if (_healthBarFill == null)
        {
            Debug.LogError($"HealthBarUI on {gameObject.name}: No suitable fill Image found in health bar prefab.", this);
            Destroy(_healthBarInstance);
            return;
        }

        // Setup fill type for smooth bar
        _healthBarFill.type = Image.Type.Filled;
        _healthBarFill.fillMethod = Image.FillMethod.Horizontal;
        _healthBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;

        // Get or add CanvasGroup for fading
        _canvasGroup = _healthBarInstance.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = _healthBarInstance.AddComponent<CanvasGroup>();
        }

        // Initialize
        _targetFillAmount = _health.HealthPercent;
        _healthBarFill.fillAmount = _targetFillAmount;
        UpdateHealthBarColor();
        UpdateVisibility();
    }

    private void LateUpdate()
    {
        if (_healthBarFill == null) return;

        UpdateHealthBarPosition();
        UpdateHealthBarFill();
    }

    private void UpdateHealthBarPosition()
    {
        if (_healthBarInstance == null || _mainCamera == null) return;

        Vector3 worldPosition = transform.position + _worldOffset;

        if (_useWorldSpace)
        {
            _healthBarInstance.transform.position = worldPosition;
        }
        else
        {
            // Convert world position to screen point for UI overlay
            Vector3 screenPoint = _mainCamera.WorldToScreenPoint(worldPosition);

            // Hide if behind camera
            if (screenPoint.z < 0)
            {
                _canvasGroup.alpha = 0;
                return;
            }

            _healthBarInstance.transform.position = screenPoint;
            _canvasGroup.alpha = 1;
        }
    }

    private void UpdateHealthBarFill()
    {
        if (_healthBarFill == null) return;

        // Smooth lerp to target
        float currentFill = _healthBarFill.fillAmount;
        if (Mathf.Abs(currentFill - _targetFillAmount) > 0.001f)
        {
            _healthBarFill.fillAmount = Mathf.Lerp(currentFill, _targetFillAmount, Time.deltaTime * _smoothSpeed);
        }
        else
        {
            _healthBarFill.fillAmount = _targetFillAmount;
        }
    }

    private void OnHealthPercentChanged(float percent)
    {
        _targetFillAmount = percent;
        UpdateHealthBarColor();
        UpdateVisibility();
    }

    private void OnDeath()
    {
        if (_hideOnDeath && _healthBarInstance != null)
        {
            _healthBarInstance.SetActive(false);
        }
    }

    private void UpdateHealthBarColor()
    {
        if (_healthBarFill == null || !_useColorGradient) return;

        _healthBarFill.color = _healthGradient.Evaluate(_targetFillAmount);
    }

    private void UpdateVisibility()
    {
        if (_healthBarInstance == null) return;

        bool isFullHealth = _health.HealthPercent >= 1f;
        bool shouldShow = !(_hideAtFullHealth && isFullHealth) && _health.IsAlive;

        _healthBarInstance.SetActive(shouldShow);
    }

    public void SetHealthBarActive(bool active)
    {
        if (_healthBarInstance != null)
        {
            _healthBarInstance.SetActive(active);
        }
    }

    public void ForceUpdatePosition()
    {
        UpdateHealthBarPosition();
    }
}
