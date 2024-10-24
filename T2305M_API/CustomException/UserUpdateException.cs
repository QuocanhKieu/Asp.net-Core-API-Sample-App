using T2305M_API.DTO.User;

namespace T2305M_API.CustomException
{
    public class UserUpdateException : Exception
    {
        public UpdateUserResponseDTO ResponseDTO { get; }

        public UserUpdateException(UpdateUserResponseDTO responseDTO, string message)
            : base(message)
        {
            ResponseDTO = responseDTO;
        }

        public UserUpdateException(UpdateUserResponseDTO responseDTO, string message, Exception innerException)
    : base(message, innerException)
        {
            ResponseDTO = responseDTO;
        }

    }
}
