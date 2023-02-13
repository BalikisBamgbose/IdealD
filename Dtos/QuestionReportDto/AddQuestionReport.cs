﻿using IdealDiscuss.Entities;

namespace IdealDiscuss.Dtos.QuestionReportDto
{
    public class AddQuestionReport
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public string AdditionalComment { get; set; }
        public int Id { get; set; }
        public string CreatedBy { get; set; } 
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
