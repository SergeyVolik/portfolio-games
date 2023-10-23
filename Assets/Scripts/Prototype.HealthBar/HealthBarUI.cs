using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField]
        Slider slider;
        [SerializeField]
        TMPro.TextMeshProUGUI healthText;

        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            var activate = !(currentHealth == 0 || currentHealth == maxHealth);
            gameObject.SetActive(activate);

            slider.minValue = 0;
            slider.maxValue = maxHealth;
            slider.value = currentHealth;

            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}