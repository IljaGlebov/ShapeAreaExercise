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
            throw new NotImplementedException();
        }
    }

    static class IO {
        public static IO<string> Read(string help) => throw    new NotImplementedException();
        public static IO<T> Return<T>(T value) => throw    new NotImplementedException();

        public static IO<T> Map<T, I>(this IO<I> io, Func<I, T> map) => io.Bind(arg => Return(map(arg)));
        public static IO<T> Error<T>(string message) => throw    new NotImplementedException();
        
        public static IO<float> Parse(this IO<string> io) => throw    new NotImplementedException();
        public static IO<O> BindResult<T, O>(this IO<T> io, Func<T, Result<O>> func) => throw    new NotImplementedException();

        
    }
    
    
    
    internal class Program {


        public static void Main(string[] args) {
            var io = IO.Read("Figure? [Circle, Triangle]")
                .Map(s => s.ToLower())
                .Bind(s => s switch {
                    "circle" => IO.Read("Radius?").Parse().Map(i => (Shape) new Circle(i)),

                    "triangle" => IO.Read("Side A?").Parse()
                        .Bind(sideA => IO.Read("Side B?").Parse()
                            .Bind(sideB => IO.Read("Side C?").Parse().Map(sideC => (Shape) new Triangle(sideA, sideB, sideC)))),
                    
                    _ => IO.Error<Shape>("Неизвестный тип")
                    }).BindResult(Area.Calculate);


            Run(io);
        }

        private static void Run(IO<double> io) {
            
        }
    }
}