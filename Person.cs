namespace OOP_Project
{
    abstract class Person
    {
        protected string Id;
        protected string Name;
        protected string Email;
        protected string PhoneNumber;
        public Person(string id , string name, string email, string phoneNumber)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
        }
        public string GetId() { return Id; }
        public string GetName() { return Name; }
    }
}