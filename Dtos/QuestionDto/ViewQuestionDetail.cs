﻿namespace IdealDiscuss.Dtos.QuestionDto
{
    public class ViewQuestionDetail
    {
        public string QuestionText { get; set; }
        public string ImageUrl { get; set; }
        public bool IsClosed { get; set; }
        public DateTime LastModified { get; set; }
        public string CreatedBy { get; set; }
        public int UserId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
