using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public List<Wedding> Weddings { get; set; }
        public List<Invite> Invitations { get; set; }
        public User()
        {
            Created_At = DateTime.Now;
            Updated_At = DateTime.Now;
            Weddings = new List<Wedding>();
            Invitations = new List<Invite>();
        }
    }
    public class Wedding
    {
        [Key]
        public int Id { get; set; }
        public string Groom { get; set; }
        public string Bride { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public DateTime Created_At { get; set;}
        public DateTime Updated_At { get; set;}
        public int UserId { get; set; }
        public User Creator { get; set; }
        public List<Invite> Guests { get; set; }
        public Wedding()
        {
            Created_At = DateTime.Now;
            Updated_At = DateTime.Now;
            Guests = new List<Invite>();
        }
    }
    public class Invite
    {
        [Key]
        public int Id { get; set; }
        public int WeddingId { get; set; }
        public Wedding Wedding { get; set; }
        public int UserId { get; set; }
        public User InvitedUser { get; set; }
    }
}