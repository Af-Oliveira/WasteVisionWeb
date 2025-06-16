using Microsoft.AspNetCore.Mvc;

namespace DDDSample1.Application.Shared
{
    public enum StatusCodeEnum
    {
        Success = 200,
        CreatedSuccess = 201,
        NotFoundError = 404,
        BadRequestError = 400,
        ServerError = 500,
        NotImplementedError = 501,
    }

    public static class ApiResponse
    {
        public static Builder<T> For<T>(T data = default) => new Builder<T>().WithData(data);

        public class Builder<T>
        {
            private T _data;
            private string _message;
            private bool _isError;

            public Builder<T> WithData(T data)
            {
                _data = data;
                return this;
            }

            public Builder<T> WithMessage(string message)
            {
                _message = message;
                return this;
            }

            public Builder<T> AsError()
            {
                _isError = true;
                return this;
            }

            public ActionResult Build(StatusCodeEnum statusCodeEnum = StatusCodeEnum.Success)
            {
                object response = _isError
                    ? new { message = _message }
                    : _data;

                return new ObjectResult(response)
                {
                    StatusCode = (int)statusCodeEnum
                };
            }
        }
    }
}
