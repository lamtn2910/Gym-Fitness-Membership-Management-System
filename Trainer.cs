namespace OOP_Project
{
    public class Trainer : Person
    {
        private string Specialization;
        public Trainer(string id, string name, string email, string phoneNumber, string specialization) : base(id, name, email, phoneNumber)
        {
            this.Specialization = specialization;
        }
        public void AsssignClass(fitnessClass FitnessClass)
        {
       
       }
    }
}