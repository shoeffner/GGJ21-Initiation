using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public GameObject healthbar;
    public float heightAboveGround = 1.8f;
    private GameObject m_healthbar;
    private Slider slider;
    private CharacterStats characterStats;
    private RectTransform rectTransform;

    void Start()
    {
        if (characterStats == null)
        {
            characterStats = GetComponent<CharacterStats>();
        }
        if (m_healthbar == null)
        {
            m_healthbar = Instantiate(healthbar);
            m_healthbar.transform.SetParent(transform, false);
            slider = m_healthbar.GetComponentInChildren<Slider>();
            if (slider == null)
            {
                Debug.LogWarning($"Healthbar of {name} has no Slider component");
            }
            rectTransform = m_healthbar.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogWarning($"Healthbar of {name} has no RectTransform component (no canvas?)");
            }
            UpdateRectTransform();
        }
    }

    void UpdateRectTransform()
    {
        if (rectTransform != null)
        {
            rectTransform.position.Set(rectTransform.position.x, heightAboveGround, rectTransform.position.z);
        }
    }

    void Update()
    {
        if (m_healthbar != null)
        {
            m_healthbar.transform.LookAt(new Vector3(transform.position.x, 0, transform.position.z + 1));
        }
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = characterStats.maxHealth;
            slider.value = characterStats.health;
        }
    }
}
