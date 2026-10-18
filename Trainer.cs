namespace OOP_project
{
    public class Trainer : Person
    {
        private string Specialization; // chuyên môn
        public Trainer(string id, string name, string email, string phoneNumber, string specialization) : base(id, name, email, phoneNumber)
        {
            this.Specialization = specialization;
        }
        public void AssignClass(FitnessClass fitnessClass)
        {
            fitnessClass.Trainer = this;
        }
    }
}