using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAPI.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Mã bắt buộc")]
        public string UserName { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Tên bắt buộc")]
        public string Password { get; set; }

        [StringLength(250)]
        public string Email { get; set; }
    }
}
