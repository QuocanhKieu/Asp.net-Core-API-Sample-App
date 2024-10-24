namespace T2305M_API.Entities
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Url { get; set; }
        public int RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } // "EVENT" or "USERARTICLE"
    }

}
