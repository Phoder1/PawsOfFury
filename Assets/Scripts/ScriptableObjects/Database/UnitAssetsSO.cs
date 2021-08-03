using System;
using UnityEngine;

[CreateAssetMenu(menuName = Database.SODatabaseFol + "Units assets")]
public class UnitAssetsSO : ScriptableObject
{
    [SerializeField]
    private TierAsset[] _tierAssets = new TierAsset[3];
    public TierAsset[] TierAssets => _tierAssets;

}
    [Serializable]
    public class TierAsset
    {
        [SerializeField]
        private Sprite _crystalSprite;
        public Sprite CrystalSprite => _crystalSprite;

        [SerializeField]
        private Color _borderColor;
        public Color BorderColor => _borderColor;
        [SerializeField]
        private Color _backgroundColor;
        public Color BackgroundColor => _backgroundColor;
    }
