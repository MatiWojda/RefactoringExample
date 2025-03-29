using System;

namespace LegacyApp
{
    public interface IUserCreditService : IDisposable
    {
        int GetCreditLimit(string lastName, DateTime dateOfBirth);
    }
}