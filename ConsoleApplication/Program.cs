using System;
using System.Collections.Immutable;
using System.Linq;
using ClassLibrary;


namespace ConsoleApplication {
    
    
    internal class Program {


        static IO<Point> ReadPoint() => IO.Read("X?").ToFloat().Bind(x => IO.Read("Y?").ToFloat().Bind(y => IO.Return(new Point(x, y))));
        static IO<Point> ReadPointN(int i) => IO.Print($"Point {i}").Bind(_ => ReadPoint()); 
        
        
        static IO<Point[]> ReadPoints(int count) {
            var first = ReadPointN(0).Bind(point => IO.Return(ImmutableArray<Point>.Empty.Add(point)));
            
            return Enumerable.Range(1, count - 1).Aggregate(first, (io, i) => io.Bind(array => ReadPointN(i).Bind(point => IO.Return(array.Add(point)))))
                    .Map(array => array.ToArray());
        }

        public static void Main(string[] args) {
            var io = IO.Read("Figure? [circle, triangle, polygon]")
                .Map(s => s.ToLower())
                .Bind(s => s switch {
                    "circle" => IO.Read("Radius?").ToFloat().Map(i => (Shape) new Circle(i)),

                    "triangle" => IO.Read("Side A?").ToFloat()
                        .Bind(sideA => IO.Read("Side B?").ToFloat()
                            .Bind(sideB => IO.Read("Side C?").ToFloat().Map(sideC => (Shape) new Triangle(sideA, sideB, sideC)))),
                    
                    "polygon" => IO.Read("Size?")
                                   .ToInt()
                                   .Bind(ReadPoints)
                                   .Map(points => (Shape) new Polygon(points)),
                    
                    _ => IO.Error<Shape>("Неизвестный тип"),
                    
                    }).BindResult(Area.Calculate)
                    .Bind(d => IO.Print($"Area = {d}"));


            Run(io);

            Console.ReadLine();
        }

        private static void Run<T>(IO<T> io) {
            switch (io) {
                case IO<Read, string, T> read:
                    
                    Console.WriteLine(read.Input.Help);

                    var readLine = Console.ReadLine();
                    
                    Run(read.Next(readLine));
                    
                    break;
                
                case IO<Print, Unit, T> print : 
                    Console.WriteLine(print.Input.Text);
                    
                    Run(print.Next(new Unit()));
                    
                    break;
                
                case Error<T> error :
                    Console.WriteLine("Ошибка : " + error.Message);
                    
                    break;
            }
        }
    }
}