using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PermissionSystemMVC.Models.ViewModels
{
    public class ViewRequest
    {
        [Display(Name = "Prmisssion Type")]
        public string PrmisssionType { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "promission Date")]
        public DateTime DatePrmission { get; set; }

        [Display(Name = "From Time")]
        public int FromTime { get; set; }

        [Display(Name = "To Time")]
        public int ToTime { get; set; }
        public string Status { get; set; }
    }
}
