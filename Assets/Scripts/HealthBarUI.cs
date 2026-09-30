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

    [Header("Behavior")]
    [SerializeField] private bool _hideAtFullHealth = true;
    [SerializeField] private bool _hideOnDeath = true;
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("Visual Feedback")]
    [SerializeField] private bool _useColorGradient = true;
    [SerializeField] private Gradient _healthGradient;

    private Health _health;
    private float _targetFillAmount;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _health = GetComponent<Health>();
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
            _health.OnHealthChanged.AddListener(OnHealthChanged);
            _health.OnDeath.AddListener(OnDeath);
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnHealthChanged.RemoveListener(OnHealthChanged);
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
        if (_canvas == null)
        {
            Debug.LogError($"HealthBarUI on {gameObject.name}: No canvas found. Cannot create health bar.", this);
            return;
        }

        // Instantiate
        _healthBarInstance = Instantiate(_healthBarPrefab.gameObject, _canvas.transform);
        _healthBarInstance.name = $"{gameObject.name}_HealthBar";

        // Find fill image - look for "Fill" child specifically
        Transform fillTransform = _healthBarInstance.transform.Find("Fill");
        if (fillTransform != null)
        {
            _healthBarFill = fillTransform.GetComponent<Image>();
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
        _targetFillAmount = _health.CurrentHealth / (float)_health.MaxHealth;
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
        if (_healthBarInstance == null) return;

        Vector3 worldPosition = transform.position + _worldOffset;
        _healthBarInstance.transform.position = worldPosition;
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

    private void OnHealthChanged(int current, int max)
    {
        _targetFillAmount = (float)current / max;
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

        bool isFullHealth = _health.CurrentHealth >= _health.MaxHealth;
        bool isDead = _health.CurrentHealth <= 0;
        bool shouldShow = !(_hideAtFullHealth && isFullHealth) && !isDead;

        _healthBarInstance.SetActive(shouldShow);
    }
}
