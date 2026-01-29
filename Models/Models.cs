using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PaieApi.Models
{


    public class Utilisateur
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("telephone")]
        public string Telephone { get; set; }

        [BsonElement("telephone_verifie")]
        public bool TelephoneVerifie { get; set; }

        [BsonElement("date_creation")]
        public DateTime DateCreation { get; set; }

        [BsonElement("derniere_connexion")]
        public DateTime? DerniereConnexion { get; set; }

        [BsonElement("actif")]
        public bool Actif { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } = "User"; // "Admin", "User", "Manager"

        [BsonElement("tenantId")]
        public string? TenantId { get; set; }
    }

    // Collection pour suivre les tentatives de connexion
    public class TentativeConnexion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("telephone")]
        public string Telephone { get; set; }

        [BsonElement("date_tentative")]
        public DateTime DateTentative { get; set; }

        [BsonElement("succes")]
        public bool Succes { get; set; }

        [BsonElement("ip_address")]
        public string IpAddress { get; set; }
    }

    // Sessions de vérification temporaires
    public class SessionVerification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("telephone")]
        public string Telephone { get; set; }

        [BsonElement("type")] // "inscription" ou "connexion"
        public string Type { get; set; }

        [BsonElement("date_demande")]
        public DateTime DateDemande { get; set; }

        [BsonElement("expire_a")]
        public DateTime ExpireA { get; set; }

        [BsonElement("tentatives")]
        public int Tentatives { get; set; }
    }

    public class Shift
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("dayName")]
        public string DayName { get; set; }

        [BsonElement("entry")]
        public string Entry { get; set; }

        [BsonElement("exit")]
        public string Exit { get; set; }

        [BsonElement("hours")]
        public string Hours { get; set; }

        [BsonElement("salary")]
        public decimal Salary { get; set; }

        [BsonElement("isVacation")]
        public bool IsVacation { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("extra")]
        public decimal Extra { get; set; }

        [BsonElement("deduction")]
        public decimal Deduction { get; set; }
    }

    public class WeeklyPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("weekStart")]
        public DateTime WeekStart { get; set; }

        [BsonElement("weekEnd")]
        public DateTime WeekEnd { get; set; }

        [BsonElement("label")]
        public string Label { get; set; }

        [BsonElement("days")]
        public List<WeeklyPlanDay> Days { get; set; }
    }

    public class WeeklyPlanDay
    {
        [BsonElement("dayName")]
        public string DayName { get; set; }

        [BsonElement("shiftName")]
        public string ShiftName { get; set; }

        [BsonElement("entry")]
        public string Entry { get; set; }

        [BsonElement("exit")]
        public string Exit { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }


    public class ShiftType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("numericId")]
        public int NumericId { get; set; } // Matches frontend 'id' (1, 2, 3...)

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("entry")]
        public string Entry { get; set; }

        [BsonElement("exit")]
        public string Exit { get; set; }



        [BsonElement("break")]
        public int Break { get; set; }

        [BsonElement("extra")]
        public decimal Extra { get; set; }

        [BsonElement("deduction")]
        public decimal Deduction { get; set; }

        [BsonElement("rates")]
        public List<Rate> Rates { get; set; }
    }

    public class Rate
    {
        [BsonElement("limit")]
        public string Limit { get; set; }

        [BsonElement("value")]
        public decimal Value { get; set; }
    }

    public class UserSetting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("salaryStartDay")]
        public int SalaryStartDay { get; set; }

        [BsonElement("hourlyWage")]
        public decimal HourlyWage { get; set; }

        [BsonElement("taxSettings")]
        public TaxSettings TaxSettings { get; set; }

        [BsonElement("generalSettings")]
        public GeneralSettings GeneralSettings { get; set; }
    }

    public class TaxSettings
    {
        [BsonElement("creditPoints")]
        public decimal CreditPoints { get; set; }

        [BsonElement("pointValue")]
        public decimal PointValue { get; set; }

        [BsonElement("isIncomeTaxExempt")]
        public bool IsIncomeTaxExempt { get; set; }

        [BsonElement("isNationalInsuranceExempt")]
        public bool IsNationalInsuranceExempt { get; set; }

        [BsonElement("isHealthTaxExempt")]
        public bool IsHealthTaxExempt { get; set; }

        [BsonElement("isShiftTaxCredit")]
        public bool IsShiftTaxCredit { get; set; }

        [BsonElement("studyFund")]
        public decimal StudyFund { get; set; }

        [BsonElement("isStudyFundFixed182")]
        public bool IsStudyFundFixed182 { get; set; }

        [BsonElement("pensionFund")]
        public decimal PensionFund { get; set; }

        [BsonElement("isPensionFundFixed182")]
        public bool IsPensionFundFixed182 { get; set; }
    }

    public class AdditionDeduction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("type")] // "addition" or "deduction"
        public string Type { get; set; }

        [BsonElement("period")] // "monthly" or "daily"
        public string Period { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("minutes")]
        public int Minutes { get; set; }

        [BsonElement("mode")]
        public string Mode { get; set; } // "amount" or "time"

        [BsonElement("shiftId")] // Optional filter
        public int? ShiftId { get; set; }

        [BsonElement("shiftIds")]
        public List<string>? ShiftIds { get; set; }

        [BsonElement("frontendId")]
        public string FrontendId { get; set; } // To sync with frontend GUIDs if needed
    }

    public class PaymentType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("numericId")]
        public int NumericId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; } // Optional
    }

    public class SickType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("numericId")]
        public int NumericId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; } // Optional
    }

    public class GeneralSettings
    {
        [BsonElement("pushNotifications")]
        public bool PushNotifications { get; set; }

        [BsonElement("workTimeMinutes")]
        public int WorkTimeMinutes { get; set; }

        [BsonElement("fixedBreak")]
        public bool FixedBreak { get; set; }

        [BsonElement("fixedBreakMinutes")]
        public int FixedBreakMinutes { get; set; }

        [BsonElement("recuperationValue")]
        public decimal RecuperationValue { get; set; }

        [BsonElement("allowUnpaidLeave")]
        public bool AllowUnpaidLeave { get; set; }

        [BsonElement("vacationDay")]
        public DaySetting VacationDay { get; set; }

        [BsonElement("holiday")]
        public DaySetting Holiday { get; set; }

        [BsonElement("sickDay1")]
        public DaySetting SickDay1 { get; set; }

        [BsonElement("sickDay2")]
        public DaySetting SickDay2 { get; set; }

        [BsonElement("sickDay3")]
        public DaySetting SickDay3 { get; set; }

        [BsonElement("sickDay4")]
        public DaySetting SickDay4 { get; set; }
    }

    public class DaySetting
    {
        [BsonElement("hours")]
        public string Hours { get; set; }

        [BsonElement("percent")]
        public decimal Percent { get; set; }
    }

    public class Lead
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("listType")]
        public string ListType { get; set; } = "lead"; // lead, quotes, followup, notrelevant, closeddeals

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("initial")]
        public string Initial { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("vip")]
        public bool Vip { get; set; }

        [BsonElement("isIcon")]
        public bool IsIcon { get; set; }

        [BsonElement("sourceId")]
        public string? SourceId { get; set; } // Link to original activity if moved
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Activity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("number")]
        public string Number { get; set; } // Caller ID

        [BsonElement("initial")]
        public string Initial { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }


        [BsonElement("textColor")]
        public string TextColor { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } // incoming, missed, outgoing

        [BsonElement("timestamp")]
        public long Timestamp { get; set; } // Unix timestamp preferred for syncing

        [BsonElement("time")]
        public string Time { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; }

        [BsonElement("isExpanded")]
        public bool IsExpanded { get; set; }

        [BsonElement("isIcon")]
        public bool IsIcon { get; set; }
    }

    public class Tenant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } // Company Name

        [BsonElement("industry")]
        public string Industry { get; set; }

        [BsonElement("companySize")]
        public string CompanySize { get; set; }

        [BsonElement("ownerId")]
        public string OwnerId { get; set; } // The user who created this tenant

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("members")]
        public List<string> Members { get; set; } = new List<string>(); // List of UserIds
    }

    public class Invitation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("tenantId")]
        public string TenantId { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("invitedBy")]
        public string InvitedBy { get; set; } // UserId of inviter

        [BsonElement("role")]
        public string Role { get; set; } = "Member";

        [BsonElement("status")]
        public string Status { get; set; } = "Pending"; // Pending, Accepted

        [BsonElement("token")]
        public string Token { get; set; } // Unique link token

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}