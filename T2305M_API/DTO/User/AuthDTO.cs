using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO.User
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; } // nhận vào password thường lưu db password hash
        
    }
    public class AuthResponse
    {
        public string Jwt { get; set; }
        public DateTime Expired_at { get; set; }
        public string Role {  get; set; }
    }
}

