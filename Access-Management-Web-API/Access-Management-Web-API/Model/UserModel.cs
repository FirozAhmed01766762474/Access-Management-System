namespace Access_Management_Web_API.Model
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? Isactive { get; set; }
        public string? Statusname { get; set; }
        public string Role { get; set; }
    }
}
