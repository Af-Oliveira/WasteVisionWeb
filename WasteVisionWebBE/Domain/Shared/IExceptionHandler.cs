using System;

namespace DDDSample1.Domain.Shared
{
    public interface IExceptionHandler
    {
        void HandleException(Exception ex);
    }
}
