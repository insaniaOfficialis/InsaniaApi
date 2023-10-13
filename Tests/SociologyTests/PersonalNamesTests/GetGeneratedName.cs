using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

/// <summary>
/// ���� ��������� �����
/// </summary>
public class GetGeneratedName : BaseTest
{
    /// <summary>
    /// ����������� ����� �������� �����
    /// </summary>
    public GetGeneratedName() : base()
    {
    }

    /// <summary>
    /// ���� �� �������� ���������� ����������
    /// </summary>
    [Fact]
    public async void Success()
    {
        //������ �������� ������
        var _mockLogger = new Mock<ILogger<PersonalNames>>();

        //������ ����� ��������� ������� ������ ���
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //�������� ��������� ���������������� �����
        var result = await personalNames.GetGeneratedName(1, true, false);

        //���������, ��� ��������� ��������
        Assert.True(result.Success);
    }
}