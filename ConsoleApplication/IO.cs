using System;
using ClassLibrary;

namespace ConsoleApplication {
    abstract class IO<T> {
        public abstract IO<B> Bind<B>(Func<T, IO<B>> f);
    }

    class IO<I, O, T> : IO<T> {
        public I Input { get; }
        public Func<O, IO<T>> Next { get; }

        public IO(I input, Func<O, IO<T>> next) {
            Input = input;
            Next = next;
        }

        public override IO<B> Bind<B>(Func<T, IO<B>> f) {
            return new IO<I, O, B>(Input, o => Next(o).Bind(f));
        }
    }

    class Return<A> : IO<A> {
        public A Result { get; }

        public Return(A result) {
            Result = result;
        }

        public override IO<B> Bind<B>(Func<A, IO<B>> f) => f(Result);
    }

    class Error<A> : IO<A> {
        public string Message { get; }

        public Error(string message) {
            Message = message;
        }

        public override IO<B> Bind<B>(Func<A, IO<B>> f) {
            return new Error<B>(Message);
        }
    }
    
    readonly struct Unit {}

    readonly struct Read {
        public readonly string Help;

        public Read(string help) {
            this.Help = help;
        }
    }
    
    readonly struct Print {
        public readonly string Text;

        public Print(string text) {
            this.Text = text;
        }
    }
    
    readonly struct Parse {}


    static class IO {
        public static IO<string> Read(string help) => new IO<Read, string, string>(new Read(help), Return);
        
        public static IO<T> Return<T>(T value) => new Return<T>(value);

        public static IO<T> Map<T, I>(this IO<I> io, Func<I, T> map) => io.Bind(arg => Return(map(arg)));
        public static IO<T> Error<T>(string message) => new Error<T>(message);
        
        public static IO<float> ToFloat(this IO<string> io) => io.BindResult(s => float.TryParse(s, out var f) ? Result<float>.Success(f) : Result<float>.Error("Не число")) ;
        public static IO<int> ToInt(this IO<string> io) => io.BindResult(s => int.TryParse(s, out var f) ? Result<int>.Success(f) : Result<int>.Error("Не число")) ;
        public static IO<O> BindResult<T, O>(this IO<T> io, Func<T, Result<O>> func) => io.Bind(arg => func(arg).Match(Return, Error<O>));


        public static IO<Unit> Print(string s) {
            return new IO<Print, Unit, Unit>(new Print(s), Return);
        }
    }
}