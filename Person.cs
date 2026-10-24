namespace OOP_project
{
    public abstract class Person
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Person(string? id , string? name, string? email, string? phoneNumber)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
        }
    }
}