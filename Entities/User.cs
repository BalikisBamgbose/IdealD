﻿namespace IdealDiscuss.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string HashSalt { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<QuestionReport> QuestionReports { get; set; } = new HashSet<QuestionReport>();
    }
}
