namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.UpdateCategory;
public class UpdateCategoryTestDataGenerate
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int indice = 0; indice < times; indice++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);
            yield return new object[] { exampleCategory, exampleInput };
        }
    }
    public static IEnumerable<object[]> GetInvalidInputs(int times = 9)
    {
        var fixture = new UpdateCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(new object[] { fixture.GetInvalidInputShortName(), $"Name should be at leats 3 caracters long" });
                    break;
                case 1:
                    invalidInputList.Add(new object[] { fixture.GetInvalidInputLongName(), $"Name should be less or equal 255 characters" });
                    break;
                case 2:
                    invalidInputList.Add(new object[] { fixture.GetInvalidInputTooLongDescritpion(), $"Description should be less or equal 10000 characters" });
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}
