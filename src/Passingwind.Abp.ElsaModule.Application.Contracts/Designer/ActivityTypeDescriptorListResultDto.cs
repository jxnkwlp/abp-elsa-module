using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Designer
{
    public class ActivityTypeDescriptorListResultDto : ListResultDto<ActivityTypeDescriptorDto>
    {
        public string[] Categories { get; set; }
    }
}
