using System;

namespace ClassLibrary {
    public abstract class Result<T> {
        class SuccessResult<T> : Result<T> {
            public T Result { get; }
            public SuccessResult(T result) {
                Result = result;
            }

            public override Result<O> Map<O>(Func<T, O> map) {
                return map(Result);
            }

            public override Result<O> Bind<O>(Func<T, Result<O>> map) {
                return map(Result);
            }

            public override O Match<O>(Func<T, O> success, Func<string, O> error) {
                return success(Result);
            }
        }

        class ErrorResult<T> : Result<T> {
            public string Message { get; }
            public ErrorResult(string message) {
                Message = message;
            }

            public override Result<O> Map<O>(Func<T, O> map) {
                return new ErrorResult<O>(Message);
            }

            public override Result<O> Bind<O>(Func<T, Result<O>> map) {
                return new ErrorResult<O>(Message); 
            }

            public override O Match<O>(Func<T, O> success, Func<string, O> error) {
                return error(Message);
            }
        }
        
        public static implicit operator Result<T>(T value) => new SuccessResult<T>(value);
        public static Result<T> Error(string message) => new ErrorResult<T>(message);
        public static Result<T> Success(T value) => new SuccessResult<T>(value);


        public abstract Result<O> Map<O>(Func<T, O> map);
        public abstract Result<O> Bind<O>(Func<T, Result<O>> map);
        public abstract O Match<O>(Func<T, O> success, Func<string, O> error);
    }
    
     
}