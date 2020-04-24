using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class Campaign
    {
        [Key]
        public int CampaignId {get; set;}
        [Required]
        public string Title {get; set;}
        [Required]
        [FutureDate]
        [Display(Name = "Start Time")]
        public DateTime Date {get; set;}
        public string Duration {get; set;}
        [Required]
        [MaxLength(50)]
        public string Description {get; set;}
        public int UserId {get; set;}
        public User Planner {get; set;}
        public List<Particpant> Attendees {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpatedAt {get; set;} = DateTime.Now;
    }
}