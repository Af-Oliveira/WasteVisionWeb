using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Shared
{
    public class DatabaseExceptionHandler : IExceptionHandler
    {
        private static string GetMySqlErrorMessage(MySqlException ex)
        {
            return ex.Number switch
            {
                (int)MySqlErrorCodes.ForeignKeyConstraint => "Invalid reference data provided.",
                (int)MySqlErrorCodes.CannotAddOrUpdateChild => "Cannot modify this record due to existing references.",
                (int)MySqlErrorCodes.DataTooLong => "One or more fields exceed maximum length.",
                _ => "An error occurred while saving the data."
            };
        }



        public void HandleException(Exception ex)
        {
            if (ex is DbUpdateException dbEx && dbEx.InnerException is MySqlException mysqlEx)
            {
                var errorMessage = GetMySqlErrorMessage(mysqlEx);
                throw new BusinessRuleValidationException(errorMessage);
            }

            if (ex is not BusinessRuleValidationException)
            {
                throw new BusinessRuleValidationException("An unexpected error occurred while processing your request.");
            }

            throw ex;
        }
    }


    public enum MySqlErrorCodes
    {
        DuplicateEntry = 1062,
        ForeignKeyConstraint = 1452,
        CannotAddOrUpdateChild = 1451,
        DataTooLong = 1406
    }
}
