namespace OOP_project
{
    public class Trainer : Person
    {
        private string Specialization;
        public Trainer(string id, string name, string email, string phoneNumber, string specialization) : base(id, name, email, phoneNumber)
        {
            this.Specialization = specialization;
        }
        public void AsssignClass(FitnessClass fitnessClass)
        {
            fitnessClass.Trainer = this;
        }
    }
}