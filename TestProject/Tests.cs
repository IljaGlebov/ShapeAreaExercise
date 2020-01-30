using System;
using System.Diagnostics;
using System.Linq;
using ClassLibrary;
using FsCheck;
using NUnit.Framework;
using Random = FsCheck.Random;

namespace TestProject {

    static class Helper {
        public static bool AssertIsPositiveOrError(this Result<double> result) => result.Match(d => d > 0, _ => true);
    }
    
    [TestFixture]
    public class Tests {

        [FsCheck.NUnit.Property]
        public void triangle_area_should_be_positive() {
            Prop.ForAll<float, float, float>((a, b, c) => Area.Calculate(new Triangle(a, b, c)).AssertIsPositiveOrError()).QuickCheckThrowOnFailure();
        }
        
        [FsCheck.NUnit.Property]
        public void circle_area_should_be_positive() {
            Prop.ForAll<float>(radius => Area.Calculate(new Circle(radius)).AssertIsPositiveOrError()).QuickCheckThrowOnFailure();
        }
        
        [FsCheck.NUnit.Property]
        public void polygon_area_should_be_positive() {
            var size = Gen.Resize(100, Arb.Default.Int16().Generator);

            var points = Gen.Two(Arb.Default.Float32().Generator).Select(tuple => new Point(tuple.Item1, tuple.Item2));

            var args = size.SelectMany(s => Gen.ArrayOf(s, points)).Select(ps => new Polygon(ps)).ToArbitrary();

            Prop.ForAll(args, polygon => Area.Calculate(polygon).AssertIsPositiveOrError()).QuickCheckThrowOnFailure();
        }
        
        [Test] 
        public void should_calculate_area_polygon() {
            var calculate = Area.Calculate(new Polygon(new []{ new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 0) }));

            var result = calculate.Match(d => d, s => throw new AssertionException(s));
            
            Assert.That(result, Is.EqualTo(1f));
        }
        
        [Test] 
        public void should_calculate_triangle_polygon() {
            var calculate = Area.Calculate(new Polygon(new []{ new Point(0, 0), new Point(1, 2), new Point(2, 0) }));

            var result = calculate.Match(d => d, s => throw new AssertionException(s));
            
            Assert.That(result, Is.EqualTo(2f));
        }
        
        [Test] 
        public void should_be_error_on_empty_polygon() {
            var calculate = Area.Calculate(new Polygon(new Point[0]));

            var hasError = calculate.Match(_ => false, s => true);
            
            Assert.That(hasError);
        }

        [Test]
        public void should_calculate_triangle() {
            var calculate = Area.Calculate(new Triangle(10, 10, 10));

            var result = calculate.Match(d => d, s => throw new AssertionException(s));
            
            Assert.That(result, Is.EqualTo(43.301).Within(.0005));
        }
        
        [Test] 
        public void should_be_error_on_invalid_triangle() {
            var calculate = Area.Calculate(new Triangle(0, 1, 1));

            var hasError = calculate.Match(_ => false, s => true);
            
            Assert.That(hasError);
        }
    }
}