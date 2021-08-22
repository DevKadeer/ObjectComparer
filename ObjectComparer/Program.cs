using ObjectComparer.Model;
using System;

namespace ObjectComparer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            var student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            var student3 = new Student() { Name = "John", Id = 100, Marks = new[] { 100, 90, 80 } };

            var student4 = new Student() { Name = "John", Id = 101, Marks = new[] { 80, 90, 100 } };
            var student5 = new Student() { Name = "John", Id = 100, Marks = new[] { 90, 80 } };

            bool result = Utility.AreEqual(student1, student2);
            string output = result ? "Both objects are equal" : "Both objects are NOT equal";
            Console.WriteLine(output); //should return equal

            result = Utility.AreEqual(student1, student3);
            output = result ? "Both objects are equal" : "Both objects are NOT equal";
            Console.WriteLine(output); //should return equal

            result = Utility.AreEqual(student1, student4);
            output = result ? "Both objects are equal" : "Both objects are NOT equal";
            Console.WriteLine(output); //should return NOT equal

            result = Utility.AreEqual(student1, student5);
            output = result ? "Both objects are equal" : "Both objects are NOT equal";
            Console.WriteLine(output); //should return NOT equal
        }


    }
}
