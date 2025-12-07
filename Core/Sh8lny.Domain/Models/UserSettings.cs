using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class UserSettings
    {
        // Primary key
        public int SettingID { get; set; }

        // Foreign key
        public int UserID { get; set; }

        // Notification preferences
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool MessageNotifications { get; set; }
        public bool ApplicationNotifications { get; set; }

        // Display preferences
        public required string Language { get; set; }
        public required string Timezone { get; set; }

        // Privacy
        public ProfileVisibility ProfileVisibility { get; set; }

        // Timestamp
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }

    /// <summary>
    /// Profile visibility enumeration
    /// </summary>
    public enum ProfileVisibility
    {
        Public,
        Private,
        UniversityOnly
    }
}
