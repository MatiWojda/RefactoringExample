using System;

namespace LegacyApp
{
    public class UserService
    {
        // Stałe
        private const int MinimumAge = 21;
        private const int MinimumCreditLimit = 500;
        private const string VeryImportantClientType = "VeryImportantClient";
        private const string ImportantClientType = "ImportantClient";


        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;

        public UserService()
        {
            _clientRepository = new ClientRepository();
            _userRepository = new UserDataAdapter();
        }

        public UserService(IClientRepository clientRepository, IUserRepository userRepository)
        {
            _clientRepository = clientRepository;
            _userRepository = userRepository;
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!ValidateInput(firstName, lastName, email, dateOfBirth))
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);

            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);

            SetCreditLimit(user, client);

            if (!ValidateCreditLimit(user))
            {
                return false;
            }

            _userRepository.AddUser(user);

            return true;
        }

        private bool ValidateInput(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            // Prosta walidacja email
            if (!email.Contains('@') || !email.Contains('.'))
            {
                return false;
            }

            if (CalculateAge(dateOfBirth) < MinimumAge)
            {
                return false;
            }

            return true;
        }

        // Wydzielona metoda obliczania wieku
        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }
            return age;
        }

        // Wydzielona metoda tworzenia użytkownika
        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
             return new User
             {
                 Client = client,
                 DateOfBirth = dateOfBirth,
                 EmailAddress = email,
                 FirstName = firstName,
                 LastName = lastName
             };
        }

        // Metoda ustawiająca limit kredytowy
        private void SetCreditLimit(User user, Client client)
        {
            if (client.Type == VeryImportantClientType)
            {
                user.HasCreditLimit = false;
            }
            else
            {
                user.HasCreditLimit = true;
                using (IUserCreditService userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    if (client.Type == ImportantClientType)
                    {
                        creditLimit *= 2;
                    }
                    user.CreditLimit = creditLimit;
                }
            }
        }

        // Metoda walidująca limit kredytowy
        private bool ValidateCreditLimit(User user)
        {
            if (user.HasCreditLimit && user.CreditLimit < MinimumCreditLimit)
            {
                return false;
            }
            return true;
        }
    }
}
