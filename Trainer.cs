namespace OOP_project
{
    public class Trainer : Person
    {
        private static int _nextId = 1;

        private string _specialization = string.Empty;
        public string Specialization
        {
            get => _specialization;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Chuyên môn không được để trống.", nameof(value));
                _specialization = value.Trim();
            }
        }

        // Constructor tự sinh ID dựa trên biến static
        public Trainer(string name, string email, string phoneNumber, string specialization)
            : base(GenerateNextId(), name, email, phoneNumber)
        {
            Specialization = specialization;
        }

        // Constructor truyền ID tùy chọn (nạp dữ liệu cũ)
        public Trainer(string id, string name, string email, string phoneNumber, string specialization)
            : base(id, name, email, phoneNumber)
        {
            Specialization = specialization;
            SynchronizeStaticId(id);
        }

        private static string GenerateNextId()
        {
            return $"T{_nextId++:D3}";
        }

        private static void SynchronizeStaticId(string id)
        {
            if (id.StartsWith("T", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(id[1..], out int num) && num >= _nextId)
            {
                _nextId = num + 1;
            }
        }

        public string GetSpecialization() => Specialization;

        public void AssignClass(FitnessClass fitnessClass)
        {
            ArgumentNullException.ThrowIfNull(fitnessClass, nameof(fitnessClass));
            fitnessClass.Trainer = this;
        }
    }
}
