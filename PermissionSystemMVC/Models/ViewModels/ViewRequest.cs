using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PermissionSystemMVC.Models.ViewModels
{
    public class ViewRequest
    {
        [Display(Name = "Employee")]
        public string CreatedById { get; set; }

        [Display(Name = "Prmisssion Type")]
        public Request.PrmisssionTypeEnum PrmisssionType { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "promission Date")]
        public DateTime DatePrmission { get; set; }

        [Display(Name = "From Time")]
        public string FromTime { get; set; }

        [Display(Name = "To Time")]
        public string ToTime { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public Request.PrmisssionStatusEnum Status { get; set; } = Request.PrmisssionStatusEnum.New;

        public int TotalTime { get; set; }
    }
}
