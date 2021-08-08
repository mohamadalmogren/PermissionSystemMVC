using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PermissionSystemMVC.Models
{
    public class Request
    {
        public enum PrmisssionTypeEnum
        {
            Work,
            Personal
        }

        public enum PrmisssionStatusEnum
        {
            New,
            Approved,
            Rejected,
            Canceled
        }

        public int Id { get; set; }
        [Display(Name = "Prmisssion Type")]
        public PrmisssionTypeEnum PrmisssionType { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Prmisssion Date")]

        public DateTime DatePrmission { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
        public PrmisssionStatusEnum Status { get; set; }
        public AppUser CreatedBy { get; set; }
        [Display(Name = "Employee")]
        public string CreatedById { get; set; }
        public AppUser ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }
}
