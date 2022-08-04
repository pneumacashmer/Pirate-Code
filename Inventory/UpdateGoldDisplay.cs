using UnityEngine;
using UnityEngine.UI;

public class UpdateGoldDisplay : MonoBehaviour {
    private Text _goldDisplay;

    /// <summary>
    /// Gets the display text and tells it to update to the current amount of gold
    /// </summary>
    private void OnEnable()
    {
        _goldDisplay = GetComponent<Text>();
        UpdateGold();
    }

    /// <summary>
    /// Updates the text to the current amount of gold
    /// </summary>
    public void UpdateGold()
    {
        _goldDisplay.text = Currency.gold.ToString();
        ;
    }
}
