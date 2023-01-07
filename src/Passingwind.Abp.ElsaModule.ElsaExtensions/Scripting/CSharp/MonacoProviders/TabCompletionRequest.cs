namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;

public class TabCompletionRequest
{
    public virtual string Code { get; set; }

    public virtual int Position { get; set; }

}
