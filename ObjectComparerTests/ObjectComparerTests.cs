using AutoFixture;
using NUnit.Framework;
using ObjectComparer;
using ObjectComparer.Model;

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
            Student student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            Student student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            bool expectedOutput = true;

            //Act
            bool actualOutput = Utility.AreEqual(student1, student2);

            //Assert
            Assert.IsTrue(actualOutput);
        }


        [TestCase(TestName = "Compare similar but order is diffrent")]
        public void GivenObject_WhenObjectAreSimilarButOrderIsDiffrent_ThenReturnTrue()
        {
            //Arrange
            Student student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            Student student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 90, 80, 100 } };

            //Act
            bool actualOutput = Utility.AreEqual(student1, student2);
            //bool actualOutput = Utility.ComparGenerice(student1, student2);

            //Assert
            Assert.IsTrue(actualOutput);

        }


        [TestCase(TestName = "Compare when object type is same but property value count is diffrent")]
        public void GivenObject_WhenObjectPropertyHasDiffrentValueCount_ThenReturnFalse()
        {
            //Arrange
            Student student1 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };
            Student student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 100 } };

            //Act
            bool actualOutput = Utility.AreEqual(student1, student2);
            //bool actualOutput = Utility.ComparGenerice(student1, student2);

            //Assert
            Assert.IsFalse(actualOutput);
        }

        [TestCase(TestName = "Compare when object type is same but property value is diffrent")]
        public void GivenObject_WhenObjectPropertyHasDiffrentValue_ThenReturnFalse()
        {
            //Arrange
            Student student1 = new Student() { Name = "John", Id = 101, Marks = new[] { 80, 90, 100 } };
            Student student2 = new Student() { Name = "John", Id = 100, Marks = new[] { 80, 90, 100 } };

            //Act

            bool actualOutput = Utility.AreEqual(student1, student2);

            //Assert
            Assert.IsFalse(actualOutput);
        }

        [TestCase(TestName = "Compare when objects are of diffrent types")]
        public void GivenObject_WhenObjectAreDiffrent_ThenReturnFalse()
        {
            //Arrange
            Student student1 = new Student() { Name = "John", Id = 101, Marks = new[] { 80, 90, 100 } };
            var college1 = new { Name = "College Name", Id = 121, Address = "Pune, Maharastra" };

            //Act
            bool actualOutput = Utility.AreEqual(student1, college1);

            //Assert
            Assert.IsFalse(actualOutput);
        }
    }
}