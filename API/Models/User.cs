using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class User : IdentityUser<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime TimeAdded { get; set; }
    }
}
