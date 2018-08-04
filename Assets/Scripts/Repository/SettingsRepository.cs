using UnityEngine;
using World;

namespace Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        private const string WorldTypeKey = "WorldTypeKey";

        public void SaveWorldType(WorldType worldType)
        {
            PlayerPrefs.SetInt(WorldTypeKey, (int) worldType);
        }

        public WorldType GetWorldType(WorldType defaulWorldType = WorldType.Flat)
        {
            return (WorldType) PlayerPrefs.GetInt(WorldTypeKey, (int) defaulWorldType);
        }
    }
}
