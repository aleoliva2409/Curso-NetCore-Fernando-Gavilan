using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Tests.UnitTests
{
    [TestClass]
    public class CapitalLetterAttributeTests
    {
        [TestMethod]
        public void FirstLetterLowerCaseError()
        {
            // Preparation
            var firstCapitalLetter = new CapitalLetterAttribute();
            var value = "alejo";
            var valContext = new ValidationContext(new { Name = value });

            // Ejecution
            var result = firstCapitalLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.AreEqual("First letter must be capital", result.ErrorMessage);
        }

        [TestMethod]
        public void NullValue_NoError()
        {
            // Preparation
            var firstCapitalLetter = new CapitalLetterAttribute();
            string? value = null;
            var valContext = new ValidationContext(new { Name = value });

            // Ejecution
            var result = firstCapitalLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CapitalLetterValue_NoError()
        {
            // Preparation
            var firstCapitalLetter = new CapitalLetterAttribute();
            string value = "Alejo";
            var valContext = new ValidationContext(new { Name = value });

            // Ejecution
            var result = firstCapitalLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }
    }
}