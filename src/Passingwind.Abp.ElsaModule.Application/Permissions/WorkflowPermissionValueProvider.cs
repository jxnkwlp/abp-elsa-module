//using System;
//using System.Threading.Tasks;
//using Volo.Abp.Authorization.Permissions;

//namespace Passingwind.Abp.ElsaModule.Permissions;

//public class WorkflowPermissionValueProvider : PermissionValueProvider
//{
//    public override string Name => "Workflow";

//    public WorkflowPermissionValueProvider(IPermissionStore permissionStore) : base(permissionStore)
//    {
//    }

//    public override Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
//    {
//        throw new NotImplementedException();
//    }

//    public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
//    {
//        throw new NotImplementedException();
//    }
//}
