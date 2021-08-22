using AutoFixture;
using NUnit.Framework;
using ObjectComparer;

namespace ObjectComparerTests
{
    [TestFixture]
    public class ObjectComparerTests
    {
        private readonly Fixture _fixture;
        public ObjectComparerTests()
        {
            _fixture = new Fixture();
        }

        [TestCase(TestName = "Compare similar object")]
        public void GivenObject_WhenObjectAreSimilar_ThenReturnTrue()
        {
            //Arrange
            Student Student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            Student Student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            bool expectedOutput = true;
            //Act
            bool actualOutput = Utility.AreEqual(Student1, Student2);

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }


        [TestCase(TestName = "Compare similar but order is diffrent")]
        public void GivenObject_WhenObjectAreSimilarButOrderIsDiffrent_ThenReturnTrue()
        {
            //Arrange
            Student Student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            Student Student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 90, 80, 100 } };
            bool expectedOutput = true;
            //Act
            bool actualOutput = Utility.AreEqual(Student1, Student2);

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestCase(TestName = "Compare when object type is same but property value is diffrent")]
        public void GivenObject_WhenObjectPropertyHasDiffrentValue_ThenReturnFalse()
        {
            //Arrange
            Student Student1 = new Student() { Name = "John", Id = 101, Marks = new[] { 80, 90, 100 } };
            Student Student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            bool expectedOutput = false;
            //Act
            bool actualOutput = Utility.AreEqual(Student1, Student2);

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestCase(TestName = "Compare when objects are of diffrent types")]
        public void GivenObject_WhenObjectAreDiffrent_ThenReturnFalse()
        {
            //Arrange
            Student Student1 = new Student() { Name = "John", Id = 101, Marks = new[] { 80, 90, 100 } };
            College College1 = new College() { Name = "College Name", Id = 121, Address="Pune, Maharastra" };
            bool expectedOutput = false;
            //Act
            bool actualOutput = Utility.AreEqual(Student1, College1);

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}