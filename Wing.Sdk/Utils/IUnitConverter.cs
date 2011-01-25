namespace Wing.Utils
{
    public interface IUnitConverter
    {
        float Get(float unit);
        UnitType ResultType { get; }
        UnitType SourceType { get; }
        float this[float unit] { get; }
    }
}
