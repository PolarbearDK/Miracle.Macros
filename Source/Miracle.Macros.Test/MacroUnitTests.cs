using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Miracle.Macros.Test
{
    internal class CustomMacro<T> : GenericMacro<T>
    {
        public CustomMacro(string macro) :
            base(macro)
        {
        }

        public CustomMacro(string macro, string startMacro, string endMacro, string formatSeparator) 
            : base(macro, startMacro, endMacro, formatSeparator)
        {
        }

        protected override IMacroFragment<T> FragmentFactory(string propertyPath, string format)
        {
            return PropertyMacroFragment<T>.Factory(propertyPath, format);
        }

    }

    internal class MyData
    {
        public string MyString { get; set; }
        public int MyNumber { get; set; }
    }

    internal class MyNestedData
    {
        public MyData MyData { get; set; }
    }

    [TestFixture]
    public class MacroUnitTests
    {
        private void ExerciseMacro(string macro, string expected, IFormatProvider formatProvider = null)
        {
            var actual = macro.ExpandMacros(formatProvider);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        private void ExerciseMacro<T>(string macro, T data, string expected, IFormatProvider formatProvider = null)
        {
            var actual = macro.ExpandMacros(data, formatProvider);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestSimpleMacroByExtension()
        {
            var dateTime = new DateTime(1963, 11, 22, 12, 30, 0);
            const string sampleString = "iqwuytoiausfiuahsf%&%/%¤¤${asdf}gasdf";

            ExerciseMacro("Hello ${Location", new { Location = "World" }, "Hello ${Location");
            ExerciseMacro("Hello ${{Location}", new { Location = "World" }, "Hello ${{Location}");


            ExerciseMacro("Hello ${Location}", new { Location = "World" }, "Hello World");
            ExerciseMacro("Hello ${Location}. How are ${Individual}?", new { Location = "World", Individual = "You" }, "Hello World. How are You?");

            ExerciseMacro("Hello ${Now}", string.Format("Hello {0}", DateTime.Now));
            ExerciseMacro("Hello ${Now}", new { }, string.Format("Hello {0}", DateTime.Now));
            ExerciseMacro("Hello ${Now}", new { Now = dateTime }, string.Format("Hello {0}", dateTime));

            ExerciseMacro("${MachineName}", Environment.MachineName);
            ExerciseMacro("${MachineName}", new { }, Environment.MachineName);
            ExerciseMacro("${MachineName}", new { MachineName = sampleString }, sampleString);

            ExerciseMacro("${CurrentThread.ManagedThreadId}", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
            ExerciseMacro("${CurrentThread.ManagedThreadId}", new { }, Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
            ExerciseMacro("${CurrentThread.ManagedThreadId}", new { CurrentThread = new { ManagedThreadId = sampleString} }, sampleString);
        }

        [Test]
        public void TestFormattingByExtension()
        {
            const double pi = Math.PI;
            CultureInfo danishCulture = CultureInfo.CreateSpecificCulture("da-DK");

            ExerciseMacro("Hello ${Number:0.0000}", new { Number = pi}, "Hello 3.1416", CultureInfo.InvariantCulture);
            ExerciseMacro("Hello ${Number:0.00000}", new { Number = pi}, "Hello 3,14159", danishCulture);

            ExerciseMacro("Hello ${Number:0000}", new { Number = 2 }, "Hello 0002", CultureInfo.InvariantCulture);
            ExerciseMacro("Hello ${Number:00000}", new { Number = 42 }, "Hello 00042", danishCulture);
            ExerciseMacro("Hello ${MyObj.Number:0000}", new { MyObj = new { Number = 42 } }, "Hello 0042", CultureInfo.InvariantCulture);

            ExerciseMacro("Hello ${Number:#,##0}", new { Number = 1000000 }, "Hello 1,000,000", CultureInfo.InvariantCulture);
            ExerciseMacro("Hello ${Number:#,##0}", new { Number = 1000000 }, "Hello 1.000.000", danishCulture);
            ExerciseMacro("Hello ${Number:#,##0.00}", new { Number = 1000002.03 }, "Hello 1,000,002.03", CultureInfo.InvariantCulture);
            ExerciseMacro("Hello ${Number:#,##0.00}", new { Number = 1000002.03 }, "Hello 1.000.002,03", danishCulture);
        }

        [Test]
        public void TestNestedMacroObject()
        {
            ExerciseMacro("Hello ${MyObj.MyProperty}", new { MyObj = new { MyProperty = 42 } }, "Hello 42", CultureInfo.InvariantCulture);
            ExerciseMacro("Hello ${MyObj.MyProperty}", new { MyObj = new { MyProperty = (string)null } }, "Hello ", CultureInfo.InvariantCulture);
            ExerciseMacro("Hello ${MyData.MyString}", new MyNestedData() {MyData = new MyData() {MyString = "foo"}}, "Hello foo");
            ExerciseMacro("Hello ${MyData.MyString}", new MyNestedData() {MyData = null}, "Hello ");
        }

        [Test]
        public void TestStaticMacroChanging()
        {
            var macro = new Macro<object>("Test static macro object ${Now}");

            var actual1 = macro.Expand(new object());
            Assert.That(actual1, Is.Not.Null);
            Console.WriteLine(actual1);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var actual2 = macro.Expand(new object());
            Assert.That(actual2, Is.Not.Null);
            Console.WriteLine(actual2);

            Assert.That(actual1, Is.Not.EqualTo(actual2));
        }

        [Test]
        public void TestMacroObject()
        {
            var dateTime = new DateTime(1963, 11, 22, 12, 30, 0);
            var macro = new Macro<DateTime>("Test macro object ${Year}.");

            var actual = macro.Expand(DateTime.Today);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo(string.Format("Test macro object {0}.", DateTime.Today.Year)));

            actual = macro.Expand(dateTime);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo(string.Format("Test macro object {0}.", dateTime.Year)));
        }

        [Test]
        public void TestCustomMacroObject()
        {
            var macro = new CustomMacro<MyData>("Test macro object ${MyString} ${Now}.");

            var actual = macro.Expand(new MyData(){MyString = "foo"});
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo("Test macro object foo ${Now}."));

            actual = macro.Expand(new MyData() { MyString = "bar" });
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo("Test macro object bar ${Now}."));
        }

        [Test]
        public void TestCustomMacroMarkers()
        {
            var macro = new CustomMacro<MyData>("Test custom markers {{MyNumber||000}}.", "{{","}}","||");

            var actual = macro.Expand(new MyData() { MyNumber = 42 });
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo("Test custom markers 042."));

            macro = new CustomMacro<MyData>("{{MyNumber}} Test custom markers {{MyNumber||0000}}. {{MyString}}", "{{", "}}", "||");

            actual = macro.Expand(new MyData() { MyNumber = 42, MyString = "Hi"});
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo("42 Test custom markers 0042. Hi"));
        }
    }
}