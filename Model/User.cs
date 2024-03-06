namespace SimpleCRUD.Model
{
    public class User : BaseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Skillsets { get; set; }
        public string Hobby { get; set; }
    }
}
