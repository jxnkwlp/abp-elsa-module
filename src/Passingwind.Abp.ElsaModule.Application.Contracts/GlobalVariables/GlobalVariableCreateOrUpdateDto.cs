using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.GlobalVariables
{
    public class GlobalVariableCreateOrUpdateDto
    {
        [Required]
        [MaxLength(64)]
        public string Key { get; set; }

        public string Value { get; set; }

        public bool IsSecret { get; set; }
    }
}
