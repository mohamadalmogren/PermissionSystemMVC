﻿using System;
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
            Rejected
        }

        public int Id { get; set; }

        public PrmisssionTypeEnum PrmisssionType { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePrmission { get; set; }

        public int FromTime { get; set; }

        public int ToTime { get; set; }
        public PrmisssionStatusEnum Status { get; set; }
        public AppUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public AppUser ModifiedBy { get; set; }
        public string ModifiedById { get; set; }

    }
}
