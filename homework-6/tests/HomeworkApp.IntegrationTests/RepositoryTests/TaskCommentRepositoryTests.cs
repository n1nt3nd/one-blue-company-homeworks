using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentRepositoryTests
{
    private readonly ITaskCommentRepository _repository;

    public TaskCommentRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
    }

    [Fact]
    public async Task Add_SomeTaskComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        // Act
        var taskCommentId = await _repository.Add(taskComment, default);
        taskComment = taskComment.WithId(taskCommentId);

        var result = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            TakeDeleted = false
        }, default);

        var actualComment = result.Single();

        // Assert
        taskCommentId.Should().BeGreaterThan(0);
        actualComment.Should().BeEquivalentTo(taskComment);
    }

    [Fact]
    public async Task Update_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        var taskCommentId = await _repository.Add(taskComment, default);
        taskComment = taskComment.WithId(taskCommentId);

        var expectedTaskComment = taskComment
            .WithMessage("Some message")
            .WithModifiedAt(DateTimeOffset.UtcNow);

        // Act
        await _repository.Update(expectedTaskComment, default);

        var result = await _repository.Get(new TaskCommentGetModel()
            {
                TaskId = expectedTaskComment.TaskId,
                TakeDeleted = true
            },
            default);

        var actualComment = result.Single();

        // Assert
        actualComment.Should().BeEquivalentTo(expectedTaskComment);
    }

    [Fact]
    public async Task SetDeleted_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        var taskCommentId = await _repository.Add(taskComment, default);
        taskComment = taskComment.WithId(taskCommentId);

        // Act
        await _repository.SetDeleted(taskCommentId, default);

        // Assert
        var result = await _repository.Get(new TaskCommentGetModel()
            {
                TaskId = taskComment.TaskId,
                TakeDeleted = true
            },
            default);

        result.Should().HaveCount(1);
        var actualComment = result.Single();

        actualComment.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTaskComments_DoNotIncludeDeleted_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        await _repository.Add(taskComment, default);
        
        // Act
        var results = await _repository.Get(new TaskCommentGetModel()
            {
                TaskId = taskComment.TaskId,
                TakeDeleted = false
            },
            default);

        // Assert
        results.Should().ContainSingle();

        var result = results.Single();
        result.Should().NotBeNull();
        result.TaskId.Should().Be(taskComment.TaskId);
        result.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetTaskComments_IncludeDeleted_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker
            .Generate();

        var taskCommentId = await _repository.Add(taskComment, default);
        
        await _repository.SetDeleted(taskCommentId, default);

        // Act
        var results = await _repository.Get(new TaskCommentGetModel()
            {
                TaskId = taskComment.TaskId,
                TakeDeleted = true
            },
            default);

        // Assert
        results.Should().ContainSingle();

        var result = results.Single();
        result.Should().NotBeNull();
        result.TaskId.Should().Be(taskComment.TaskId);
        result.DeletedAt.Should().NotBeNull();
    }
}