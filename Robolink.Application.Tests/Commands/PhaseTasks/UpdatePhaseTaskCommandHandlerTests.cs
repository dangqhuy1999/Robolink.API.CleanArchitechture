using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Enums;
using Robolink.Core.Interfaces;
using AutoMapper;

namespace Robolink.Application.Tests.Commands.PhaseTasks
{
    public class UpdatePhaseTaskCommandHandlerTests
    {
        private readonly Mock<IGenericRepository<PhaseTask>> _mockTaskRepo;
        private readonly Mock<IProjectSystemPhaseConfigRepository> _mockPhaseConfigRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdatePhaseTaskCommandHandler _handler;

        public UpdatePhaseTaskCommandHandlerTests()
        {
            _mockTaskRepo = new Mock<IGenericRepository<PhaseTask>>();
            _mockPhaseConfigRepo = new Mock<IProjectSystemPhaseConfigRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdatePhaseTaskCommandHandler(_mockTaskRepo.Object, _mockPhaseConfigRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldUpdatePhaseTask()
        {
            // ✅ Arrange
            var taskId = Guid.NewGuid();
            var phaseConfigId = Guid.NewGuid();

            var existingTask = new PhaseTask
            {
                Id = taskId,
                Name = "Old Name",
                Description = "Old Description",
                Status = Task_Status.Cancelled,
                Priority = 0,
                AssignedStaffId = Guid.NewGuid(),
                DueDate = DateTime.Now,
                ProjectSystemPhaseConfigId = phaseConfigId,
                // Gán giá trị giả để thỏa mãn điều kiện 'required'
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            };

            var command = new UpdatePhaseTaskCommand
            {
                Id = taskId,
                Name = "New Name",
                Description = "New Description",
                Status = Task_Status.InProgress,
                Priority = 2,
                CreatedBy = "Test User"
            };

            var dto = new PhaseTaskDto { Id = taskId, Name = "New Name" };

            _mockTaskRepo.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);
            _mockTaskRepo.Setup(x => x.UpdateAsync(It.IsAny<PhaseTask>()))
                .Returns(Task.CompletedTask);
            _mockTaskRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockPhaseConfigRepo.Setup(x => x.GetByIdAsync(phaseConfigId))
                .ReturnsAsync(new ProjectSystemPhaseConfig { Id = phaseConfigId, RowVersion = new byte[8] });
            _mockMapper.Setup(x => x.Map<PhaseTaskDto>(It.IsAny<PhaseTask>()))
                .Returns(dto);

            // ✅ Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // ✅ Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.Name);
            _mockTaskRepo.Verify(x => x.UpdateAsync(It.IsAny<PhaseTask>()), Times.Once);
            _mockTaskRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullableProperties_ShouldPreserveExistingValues()
        {
            // ✅ Arrange
            var taskId = Guid.NewGuid();
            var phaseConfigId = Guid.NewGuid();
            var originalName = "Original Name";
            var originalStaffId = Guid.NewGuid();

            var existingTask = new PhaseTask
            {
                Id = taskId,
                Name = originalName,
                AssignedStaffId = originalStaffId,
                ProjectSystemPhaseConfigId = phaseConfigId,
                Status = Task_Status.Cancelled,
                Priority = 1,
                DueDate = DateTime.Now,
                // Gán giá trị giả để thỏa mãn điều kiện 'required'
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }

            };

            var command = new UpdatePhaseTaskCommand
            {
                Id = taskId,
                Name = null,  // ✅ Không thay đổi
                AssignedStaffId = null,  // ✅ Giữ giá trị cũ
                CreatedBy = "Test User"
            };

            var dto = new PhaseTaskDto { Id = taskId, Name = originalName };

            _mockTaskRepo.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);
            _mockTaskRepo.Setup(x => x.UpdateAsync(It.IsAny<PhaseTask>()))
                .Callback<PhaseTask>(task =>
                {
                    // ✅ Verify rằng giá trị cũ được giữ lại
                    Assert.Equal(originalName, task.Name);
                    Assert.Equal(originalStaffId, task.AssignedStaffId);
                })
                .Returns(Task.CompletedTask);
            _mockTaskRepo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockPhaseConfigRepo.Setup(x => x.GetByIdAsync(phaseConfigId))
                .ReturnsAsync(new ProjectSystemPhaseConfig { RowVersion = new byte[8] });
            _mockMapper.Setup(x => x.Map<PhaseTaskDto>(It.IsAny<PhaseTask>()))
                .Returns(dto);

            // ✅ Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // ✅ Assert
            Assert.NotNull(result);
            _mockTaskRepo.Verify(x => x.UpdateAsync(It.IsAny<PhaseTask>()), Times.Once);
        }
    }
}