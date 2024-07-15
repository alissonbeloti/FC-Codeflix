namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;
public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        int times = 4;
        var fixture = new UpdateApiCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < times; index++)
        {
            var input = fixture.GetExampleInput();
            switch (index % totalInvalidCases)
            {
                case 0:
                    input.Name = fixture.GetInvalidShortName();
                    invalidInputList.Add(new object[] { input, $"Name should be at leats 3 caracters long" });
                    break;
                case 1:
                    input.Name = fixture.GetInvalidLongName();
                    invalidInputList.Add(new object[] { input, $"Name should be less or equal 255 characters" });
                    break;
                case 2:
                    input.Description = fixture.GetInvalidTooLongDescription();
                    invalidInputList.Add(new object[] { input, $"Description should be less or equal 10000 characters" });
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}
