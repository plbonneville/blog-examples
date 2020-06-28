using System.ComponentModel.DataAnnotations;

namespace SendingAntiForgeryTokenWithAspnetCoreMvcAjaxRequests.Models
{
    public class PersonViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }
    }
}
