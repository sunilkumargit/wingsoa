
namespace Wing.Soa
{
    public interface ISoaServiceHostBuildPolicy
    {
        void Apply(SoaServiceHostBuildContext context);
        void PostApply(SoaServiceHostBuildContext context);
    }
}
