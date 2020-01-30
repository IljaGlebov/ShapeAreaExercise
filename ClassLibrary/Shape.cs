using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace ClassLibrary {
    public abstract class Shape { }

    public class Circle : Shape {
        public double Radius { get; }

        public Circle(double radius) {
            Radius = radius;
        }
    }

    public class Triangle : Shape {
        public double SideA { get; }
        public double SideB { get; }
        public double SideC { get; }

        public Triangle(double sideA, double sideB, double sideC) {
            SideA = sideA;
            SideB = sideB;
            SideC = sideC;
        }
    }

    public readonly struct Point {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y) => (X, Y) = (x, y);
    }

    public class Polygon : Shape {
        public IReadOnlyList<Point> Points { get; }

        public Polygon(IReadOnlyList<Point> points) {
            Points = points;
        }
    }


    public static class Area {
        private static V Pipe<V, T>(this T t, Func<T, V> func) => func(t);

        private static bool IsGreatThan0(double f) => !double.IsInfinity(f) && f > double.Epsilon;

        private static Result<Shape> TriangleSidesAreGreatThan0(Triangle triangle) =>
            IsGreatThan0(triangle.SideA)
            && IsGreatThan0(triangle.SideB)
            && IsGreatThan0(triangle.SideC)
                ? triangle
                : Result<Shape>.Error("Все стороны треугольника должы быть > 0");

        private static Result<Shape> TriangleInequality(Triangle triangle) =>
            triangle.SideA < triangle.SideB + triangle.SideC && 
            triangle.SideB < triangle.SideA + triangle.SideC && 
            triangle.SideC < triangle.SideB + triangle.SideA 
                ? triangle  
                : Result<Shape>.Error("Не соблюдается неравенство треугольника");
        
        private static Result<Shape> Validate(Shape shape) =>
            shape switch{
                null => throw new ArgumentException(),
                Circle {Radius: var radius} => IsGreatThan0(radius) ? shape : Result<Shape>.Error("Радиус должен быть > 0"),
                Triangle triangle => TriangleSidesAreGreatThan0(triangle).Bind(_ => TriangleInequality(triangle)),
                Polygon {Points: var points } => points.Count > 2 ? points.Aggregate(Result<Shape>.Success(shape), (s, point) => ValidatePoint(point).Bind(_ => s)) : 
                                                                    Result<Shape>.Error("Нет точек"),
                _ => Result<Shape>.Error("Неизвестная фигура")
            };

        private static Result<Point> ValidatePoint(Point point) =>
            double.IsInfinity(point.X) || 
            double.IsNaN(point.X) || 
            double.IsInfinity(point.Y) ||
            double.IsNaN(point.Y)  ? 
                Result<Point>.Error("Ошибка в координатах точки") : point;
        

        public static Result<double> Calculate(Shape shape) =>
            Validate(shape)
                .Map(s => s switch {
                    Circle {Radius: var radius} => radius * radius * Math.PI,

                    Triangle {SideA: var a, SideB: var b, SideC: var c} =>
                        ((a + b + c) / 2).Pipe(p => p * (p - a) * (p - b) * (p - c)).Pipe(Math.Sqrt),
                    
                    /* Формула площади Гаусса */
                    Polygon { Points: var points } => points.Zip(points.Skip(1), (p1, p2) => (p1.X * p2.Y, p1.Y * p2.X))
                                                            .Aggregate((0d, 0d), (sum, v) => (sum.Item1 + v.Item1, sum.Item2 + v.Item2))
                                                            .Pipe(tuple => Math.Abs(tuple.Item1 - tuple.Item2) / 2),
                    _ => throw new ArgumentException()
                });
            

    }
    
    
}