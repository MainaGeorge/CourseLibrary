using System;

namespace CourseLibrary.API.ExtensionMethods
{
    public static class CalculatedValuesExtensionMethods
    {
        public static int GetAge(this DateTimeOffset date)
        {
            var currentDate = DateTime.UtcNow;

            var age = currentDate.Year - date.Year;

            if (currentDate < date.AddYears(age)) age--;

            return age;

        }
    }
}
