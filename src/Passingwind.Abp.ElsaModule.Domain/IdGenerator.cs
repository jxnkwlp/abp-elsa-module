using Yitter.IdGenerator;

namespace Passingwind.Abp.ElsaModule
{
    public class IdGenerator : IIdGenerator
    {
        static IdGenerator()
        {
            var options = new IdGeneratorOptions();
            YitIdHelper.SetIdGenerator(options);
        }

        public long Generate()
        {
            return YitIdHelper.NextId();
        }
    }
}
