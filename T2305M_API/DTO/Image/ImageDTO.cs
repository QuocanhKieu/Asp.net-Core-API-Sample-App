using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO
{
    public class GetBasicImageDTO
    {
        public string Url { get; set; }
    }
    public class CreateImagesDTO
    {
        public List<IFormFile> Images { get; set; }
        public int RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } // "EVENT" or "USERARTICLE"
    }

    public class CreateImagesResponseDTO
    {
        //will manually validate fields
        public bool isSuccess { get; set; }
        public string Message { get; set; }
    }


}
