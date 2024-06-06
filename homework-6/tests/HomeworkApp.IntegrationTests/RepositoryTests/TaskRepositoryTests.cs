using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);

        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }

    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);

        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] {expectedTaskId}
        }, default);

        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();

        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);

        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);

        // Act
        await _repository.Assign(assign, default);

        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] {expectedTaskId}
        }, default);

        results.Should().HaveCount(1);
        var task = results.Single();

        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task GetSubTaskInStatus_WithIdenticalStatuses_ReturnAllChildTasks()
    {
        // Arrange
        const int count = 4;

        var tasks = TaskEntityV1Faker.Generate(count);

        for (var i = 0; i < count; i++)
        {
            tasks[i] = tasks[i].WithParentId(null);
            tasks[i] = tasks[i].WithStatus(TaskStatus.InProgress);
        }

        var taskIds = await _repository.Add(tasks, default);
        var parentTaskId = taskIds.First();

        for (var i = 0; i < count; i++)
        {
            tasks[i] = tasks[i].WithId(taskIds[i]);

            if (i != 0)
            {
                tasks[i] = tasks[i].WithParentId(tasks[i - 1].Id);
                await _repository.SetParentTask(tasks[i].Id, tasks[i].ParentTaskId, default);
            }
        }

        // Act
        var results = await _repository.GetSubTasksInStatus(parentTaskId, new[]
        {
            TaskStatus.InProgress
        }, default);

        // Assert
        results.Should().HaveCount(count - 1);
        foreach (var result in results)
        {
            result.ParentTaskIds.Should().EndWith(result.TaskId);
        }
    }

    [Fact]
    public async Task GetSubTaskInStatus_WithDifferentStatuses_ReturnOnlyOneChildTasks()
    {
        // Arrange
        const int count = 20;
        var expectedInProgressCount = 0;

        var tasks = TaskEntityV1Faker.Generate(count);

        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = tasks[i].WithParentId(null);

            if (Random.Shared.Next() % 2 == 0)
            {
                tasks[i] = tasks[i].WithStatus(TaskStatus.InProgress);

                if (i != 0)
                    expectedInProgressCount++;
            }
            else tasks[i] = tasks[i].WithStatus(TaskStatus.Done);
        }

        var taskIds = await _repository.Add(tasks, default);
        var parentTaskId = taskIds.First();

        for (var i = 0; i < count; i++)
        {
            tasks[i] = tasks[i].WithId(taskIds[i]);

            if (i != 0)
            {
                var randomParent = tasks[Random.Shared.Next(0, i)];
                tasks[i] = tasks[i].WithParentId(randomParent.Id);
                await _repository.SetParentTask(tasks[i].Id, tasks[i].ParentTaskId, default);
            }
        }

        // Act
        var results = await _repository.GetSubTasksInStatus(parentTaskId, new[]
        {
            TaskStatus.InProgress
        }, default);

        // Assert
        results.Should().HaveCount(expectedInProgressCount);

        foreach (var result in results)
        {
            result.ParentTaskIds.Should().NotContain(parentTaskId);
            result.ParentTaskIds.Should().EndWith(result.TaskId);
        }
    }
}