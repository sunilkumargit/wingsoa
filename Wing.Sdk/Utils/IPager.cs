
namespace Wing.Utils
{
    public interface IPager
    {
        int ItemsCount { get; }
        int ItemsPerPage { get; }
        int PageCount { get; }
        int PageEndAt(int pageIndex);
        int PageStartAt(int pageIndex);
    }
}
