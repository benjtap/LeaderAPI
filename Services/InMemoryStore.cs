using System;
using System.Collections.Generic;
using PaieApi.Models;

namespace LeaderApi.Services
{
    public static class InMemoryStore
    {
        public static List<Lead> Leads = new List<Lead>
        {
            new Lead { Id = 1, Name = "בנק מזרחי", Phone = "076-804-8860", Initial = "ב", Color = "#3f51b5", Vip = false },
            new Lead { Id = 2, Name = "hr", Phone = "054-508-4222", Initial = "H", Color = "#e0b0ff", Vip = false },
            new Lead { Id = 3, Name = "Bnei", Phone = "052-632-0677", Initial = "B", Color = "#fdd835", Vip = true },
            new Lead { Id = 4, Name = "03-552-9320", Phone = "", Initial = null, IsIcon = true, Color = "#6200ea", Vip = false },
            new Lead { Id = 5, Name = "054-740-7421", Phone = "", Initial = null, IsIcon = true, Color = "#6200ea", Vip = false }
        };

        public static List<Activity> Activities = new List<Activity>
        {
            new Activity {
                Id = 1,
                Name = "רוני",
                Initial = "ר",
                Color = "#000080",
                Type = "incoming",
                Time = "10:21 AM",
                Date = DateTime.Now,
                IsExpanded = false
            },
            new Activity {
                Id = 2,
                Name = "רועי",
                Initial = "ר",
                Color = "#E6E6FA",
                TextColor = "#a0a0a0",
                Type = "incoming",
                Time = "10:21 AM",
                Date = DateTime.Now,
                IsExpanded = false
            },
            new Activity {
                Id = 3,
                Name = "אבישי",
                Initial = "א",
                Color = "#E6E6FA",
                TextColor = "#a0a0a0",
                Type = "missed",
                Time = "6:51 AM",
                Date = DateTime.Now,
                IsExpanded = false
            },
            new Activity {
                Id = 4,
                Name = "שלומי",
                Initial = "ש",
                Color = "#8A2BE2",
                Type = "incoming",
                Time = "16 Jan",
                Date = DateTime.Now.AddDays(-2),
                IsExpanded = false
            },
            new Activity {
                Id = 5,
                Name = "*6868",
                Initial = null,
                IsIcon = true,
                Color = "#4B0082",
                Type = "missed",
                Time = "15 Jan",
                Date = DateTime.Now.AddDays(-3),
                IsExpanded = false
            },
             new Activity {
                Id = 6,
                Name = "08-851-2411",
                Initial = null,
                IsIcon = true,
                Color = "#4B0082",
                Type = "incoming",
                Time = "15 Jan",
                Date = DateTime.Now.AddDays(-3),
                IsExpanded = false
            }
        };
        public static List<Lead> Quotes = new List<Lead>();
        public static List<Lead> FollowUp = new List<Lead>();
        public static List<Lead> NotRelevant = new List<Lead>();
        public static List<Lead> ClosedDeals = new List<Lead>();
    }
}
