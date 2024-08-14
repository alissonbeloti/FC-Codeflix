using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;

using System.Collections;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Update;
public class UpdateVideoTestDataGenerator : IEnumerable<object[]>
{

    public IEnumerator<object[]> GetEnumerator()
    {
        var fixture = new UpdateVideoTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases * 2; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(new object[] {
                        new UpdateVideoInput(
                            Guid.NewGuid(),
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
                        new UpdateVideoInput(
                            Guid.NewGuid(),
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
                        new UpdateVideoInput(
                            Guid.NewGuid(),
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
                        new UpdateVideoInput(
                            Guid.NewGuid(),
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