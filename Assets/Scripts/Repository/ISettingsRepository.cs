using World;

namespace Repository
{
    public interface ISettingsRepository
    {
        void SaveWorldType(WorldType worldType);
        WorldType GetWorldType(WorldType defaulWorldType = WorldType.Flat);
    }
}
