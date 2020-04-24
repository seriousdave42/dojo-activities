using System.ComponentModel.DataAnnotations;
using System;
namespace BeltExam.Models
{
    public class Particpant
    {
        [Key]
        public int ParticipantId {get; set;}
        public int UserId {get; set;}
        public User Attendee {get; set;}
        public int CampaignId {get; set;}
        public Campaign RSVP {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}