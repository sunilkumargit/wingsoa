using System.ServiceModel;

namespace Wing.Services.IdentityManagerService
{
    [ServiceContract]
    [ServiceKnownType(typeof(UserEntity))]
    [ServiceKnownType(typeof(RoleEntity))]
    public interface IIdentityManagerService
    {
        [OperationContract]
        UserEntity GetUser(string userName);

        [OperationContract]
        bool SaveUser(UserEntity user);
    }
}
