namespace T2305M_API.DTO.UserArticleCommentDTO
{
    public class CommentDto
    {
        public string Content { get; set; }         // The comment content
        public int? ParentCommentId { get; set; }   // null for top-level comment
        public int? TopLevelCommentId { get; set; } // have only this if a reply to the topLevelComment
        // have both for reply to relply
        // have nothing for the topMostComment

    }

}
