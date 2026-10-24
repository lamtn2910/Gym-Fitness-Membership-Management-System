namespace OOP_project
{
    public class Trainer : Person
    {
        public string? Specialization {get; set;} // chuyên môn
        public Trainer(string? id, string? name, string? email, string? phoneNumber, string? specialization) : base(id, name, email, phoneNumber)
        {
            this.Specialization = specialization;
        }
        public string? GetSpecialization() { return Specialization; }
        public void AssignClass(FitnessClass fitnessClass)
        {
            fitnessClass.Trainer = this;
        }
    }
}