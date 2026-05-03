using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
public class UiCard : MonoBehaviour
{
    public Card card;

    [Header("LastStand Shake")]
    [SerializeField] private bool shakeIfLastStand = true;
    [SerializeField] private float shakeAmplitude = 1.8f;   // movement
    [SerializeField] private float shakeFrequency = 20f;  // speed 
    [SerializeField] private bool createVisualChild = true;

    [Header("Usage")]
    [SerializeField] private int maxUses = 3; // number of times this card can be used before removed
    private int _usesRemaining;

    private RectTransform _rectTransform;
    private RectTransform _visualRect;     
    private Image _parentImage;
    private Animator _parentAnimator;
    private Coroutine _shakeCoroutine;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentImage = GetComponent<Image>();
        _parentAnimator = GetComponent<Animator>();

        if (createVisualChild)
            EnsureVisualChildExists();
    }

    void Start()
    {
        // initialize uses for this UiCard instance
        _usesRemaining = maxUses;

        ApplyVisuals();
        ApplyCardEffect();
        StartShakeIfNeeded();
    }

    private void OnEnable()
    {
        ApplyCardEffect();
        StartShakeIfNeeded();
    }

    private void ApplyVisuals()
    {
        
        if (_visualRect != null)
        {
            var visualImage = _visualRect.GetComponent<Image>();
            var visualAnimator = _visualRect.GetComponent<Animator>();
            if (card != null)
            {
                visualImage.sprite = card.sprite;
                visualAnimator.runtimeAnimatorController = card.animatorController;
            }

            
            if (_parentImage != null) _parentImage.enabled = false;
            if (_parentAnimator != null) _parentAnimator.enabled = false;
        }
        else
        {
            if (card != null)
            {
                if (_parentImage != null) _parentImage.sprite = card.sprite;
                if (_parentAnimator != null) _parentAnimator.runtimeAnimatorController = card.animatorController;
            }
        }
    }

    private void EnsureVisualChildExists()
    {
        
        var child = transform.Find("Visual");
        if (child != null)
        {
            _visualRect = child as RectTransform;
            // ensure required components exist
            if (_visualRect.GetComponent<Image>() == null) _visualRect.gameObject.AddComponent<Image>();
            if (_visualRect.GetComponent<Animator>() == null) _visualRect.gameObject.AddComponent<Animator>();
            return;
        }

        GameObject visual = new GameObject("Visual", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Animator));
        _visualRect = visual.GetComponent<RectTransform>();

        
        _visualRect.SetParent(transform, false);
        _visualRect.anchorMin = Vector2.zero;
        _visualRect.anchorMax = Vector2.one;
        _visualRect.anchoredPosition = Vector2.zero;
        _visualRect.sizeDelta = Vector2.zero;
        _visualRect.localScale = Vector3.one;
    }

    private void ApplyCardEffect()
    {
        if (card?.statusEffect != null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null && playerObj.TryGetComponent<Health>(out var health))
            {
                health.ApplyEffect(card.statusEffect);
                
            }
        }
    }

    private void OnDestroy()
    {
        StopShakeIfRunning();

        if (card?.statusEffect != null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null && playerObj.TryGetComponent<Health>(out var health))
            {
                health.RemoveEffect();
               
            }
        }
    }

    private void Update()
    {
        
        var h = FindFirstObjectByType<KillCountTracker>();
        if (h != null)
        {
            if (_visualRect != null)
            {
                var anim = _visualRect.GetComponent<Animator>();
                if (anim != null) anim.SetFloat("killCount", h.killCount);
            }
            else if (_parentAnimator != null)
            {
                _parentAnimator.SetFloat("killCount", h.killCount);
            }
        }
    }

    public void Assign()
    {
        // reset uses when assigned
        _usesRemaining = maxUses;

        ApplyVisuals();
        ApplyCardEffect();
        StartShakeIfNeeded();
    }

    // Returns true if the card is depleted and should be removed
    public bool Use(GameObject user)
    {
        if (card != null)
            card.Use(user);

        _usesRemaining = Mathf.Max(0, _usesRemaining - 1);

        return _usesRemaining <= 0;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        Start();
    }

    private void StartShakeIfNeeded()
    {
        if (!shakeIfLastStand || card == null) return;

        var cards = FindFirstObjectByType<Cards>();
        if (cards != null && cards.LastStand != null && card == cards.LastStand)
        {
            if (_shakeCoroutine == null)
            {
                _shakeCoroutine = StartCoroutine(ShakeCoroutine());
            }
        }
    }

    private void StopShakeIfRunning()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }

        if (_visualRect != null)
            _visualRect.anchoredPosition = Vector2.zero;
        else if (_rectTransform != null)
            _rectTransform.anchoredPosition = Vector2.zero;
    }

    private System.Collections.IEnumerator ShakeCoroutine()
    {
        
        RectTransform targetRect = _visualRect != null ? _visualRect : _rectTransform;
        if (targetRect == null) yield break;

        while (true)
        {
            float offsetX = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude;
            
            float jitter = Mathf.Sin(Time.time * (shakeFrequency * 1.37f)) * (shakeAmplitude * 0.25f);
            targetRect.anchoredPosition = new Vector2(offsetX + jitter, 0f);
            yield return null;
        }
    }
}
