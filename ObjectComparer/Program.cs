using System; 

namespace SimilarObjectComparer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Student Student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            Student Student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 90, 80, 100 } };

            Student Student3 = new Student() { Name = "John", Id = 101, Marks = new[] { 80, 90, 100 } };
            Student Student4 = new Student() { Name = "John", Id = 100, Marks = new[] { 100, 90, 80 } };
            Student Student5 = new Student() { Name = "John", Id = 100, Marks = new[] { 90, 80, 100 } };


            //if (Compare<Student>(Student1, Student2))
            //    Console.WriteLine("Both objects are equal");
            //else
            //    Console.WriteLine("Both objects are not equal");
            ////if (Compare<Student>(Student1, Student3))
            ////    Console.WriteLine("Both objects are equal");
            ////else
            ////    Console.WriteLine("Both objects are not equal");
            //if (Compare<Student>(Student1, Student2))
            //    Console.WriteLine("Both objects are equal");
            //else
            //    Console.WriteLine("Both objects are not equal");
            //var result = Utility.Compare(Student1, Student2);
            //var output = result ? "Both objects are equal" : "Both objects are NOT equal";
            ////Console.WriteLine(output);

            bool result = Utility.AreEqual(Student1, Student2);
            string output = result ? "Both objects are equal" : "Both objects are NOT equal";
            Console.WriteLine(output);

            //result = Student1.CompareTo(Student2);
            //output = result ? "Both objects are equal" : "Both objects are NOT equal";
            //Console.WriteLine(output);
            //if (Compare<Student>(Student1, Student5))
            //    Console.WriteLine("Both objects are equal");
            //else
            //    Console.WriteLine("Both objects are not equal");
            //Console.Read();
        }


    }
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Marks { get; set; }
    }
    public class College
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
