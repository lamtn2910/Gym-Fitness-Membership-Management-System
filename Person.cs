namespace OOP_project
{
    public abstract class Person
    {
        public string Id { get; protected set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên không được để trống.", nameof(value));
                _name = value.Trim();
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                    throw new ArgumentException("Email không hợp lệ.", nameof(value));
                _email = value.Trim();
            }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Số điện thoại không được để trống.", nameof(value));
                _phoneNumber = value.Trim();
            }
        }

        protected Person(string id, string name, string email, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID không được để trống.", nameof(id));

            Id = id.Trim();
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
