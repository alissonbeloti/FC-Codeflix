using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

using Moq;

using System.Collections;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Create;
public class CreateVideoTestDataGenerator : IEnumerable<object[]>
{

    public IEnumerator<object[]> GetEnumerator()
    {
        var fixture = new CreateVideoTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases * 2; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(new object[] {
                        new CreateVideoInput(
                            "",
                            fixture.GetValidDescription(),
                            fixture.GetRandomRationg(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandoBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandoBoolean()
                        ),
                        $"'Title' is required" });
                    break;
                case 1:
                    invalidInputList.Add(new object[] {
                        new CreateVideoInput(
                            fixture.GetValidTitle(),
                            "",
                            fixture.GetRandomRationg(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandoBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandoBoolean()
                        ),
                        $"'Description' is required" });
                    break;
                case 2:
                    invalidInputList.Add(new object[] {
                        new CreateVideoInput(
                            fixture.GetTooLongTitle(),
                            fixture.GetValidDescription(),
                            fixture.GetRandomRationg(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandoBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandoBoolean()
                        ),
                        $"'Title' should be less or equal 255 characters long" });
                    break;
                case 3:
                    invalidInputList.Add(new object[] {
                        new CreateVideoInput(
                            fixture.GetValidTitle(),
                            fixture.GetTooLongDescription(),
                            fixture.GetRandomRationg(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandoBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandoBoolean()
                        ),
                        $"'Description' should be less or equal 4000 characters long" });
                    break;

                default:
                    break;
            }
        }

        return invalidInputList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
