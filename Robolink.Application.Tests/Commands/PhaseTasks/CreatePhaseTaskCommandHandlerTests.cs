using AutoMapper;
using MediatR;
using Moq;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Robolink.Application.Tests.Commands.PhaseTasks
{
    public class CreatePhaseTaskCommandHandlerTests
    {

        private readonly Mock<IGenericRepository<PhaseTask>> _mockTaskRepo;
        private readonly Mock<IGenericRepository<ProjectSystemPhaseConfig>> _mockPhaseConfigRepo; // ✅ Đổi sang Generic cho đồng bộ
        private readonly Mock<IGenericRepository<Staff>> _mockStaffRepo; // ✅ Thêm mới
        private readonly Mock<IMediator> _mockMediator; // ✅ Thêm mới để bắn Event
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreatePhaseTaskCommandHandler _handler;

        public CreatePhaseTaskCommandHandlerTests()
        {
            _mockTaskRepo = new Mock<IGenericRepository<PhaseTask>>();
            _mockStaffRepo = new Mock<IGenericRepository<Staff>>(); // ✅ Khởi tạo Mock Staff
            _mockPhaseConfigRepo = new Mock<IGenericRepository<ProjectSystemPhaseConfig>>(); // ✅ Khởi tạo Mock Config
            _mockMediator = new Mock<IMediator>(); // ✅ Khởi tạo Mock Mediator
            _mockMapper = new Mock<IMapper>();

            // ✅ Truyền ĐẦY ĐỦ 5 tham số theo đúng thứ tự trong Handler mới
            _handler = new CreatePhaseTaskCommandHandler(
                _mockTaskRepo.Object,
                _mockPhaseConfigRepo.Object,
                _mockStaffRepo.Object,
                _mockMapper.Object,
                _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_CreatesPhaseTask()
        {
            // ✅ Arrange
            var projectId = Guid.NewGuid();
            var phaseConfigId = Guid.NewGuid();
            var staffId = Guid.NewGuid();

            var createRequest = new CreatePhaseTaskRequest
            {
                Name = "Test Task",
                Description = "Test Description",
                ProjectId = projectId,
                ProjectSystemPhaseConfigId = phaseConfigId,
                AssignedStaffId = staffId,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Priority = 1,
                Status = 0
            };

            var command = new CreatePhaseTaskCommand
            {
                Request = createRequest,
                CreatedBy = "Test User"
            };

            var phaseConfig = new ProjectSystemPhaseConfig { Id = phaseConfigId, // Gán giá trị giả để thỏa mãn điều kiện 'required'
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            };
            var createdTask = new PhaseTask
            {
                Id = Guid.NewGuid(),
                Name = createRequest.Name,
                ProjectId = projectId,
                ProjectSystemPhaseConfigId = phaseConfigId,
                AssignedStaffId = staffId,
                Status = Task_Status.Cancelled,
                // Gán giá trị giả để thỏa mãn điều kiện 'required'
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            };

            var dto = new PhaseTaskDto 
            { 
                Id = createdTask.Id, 
                Name = "Test Task",
                ProjectId = projectId,
                ProjectSystemPhaseConfigId = phaseConfigId
            };

            _mockPhaseConfigRepo
                .Setup(x => x.GetByIdAsync(phaseConfigId))
                .ReturnsAsync(phaseConfig);

            _mockTaskRepo
                .Setup(x => x.AddAsync(It.IsAny<PhaseTask>()))
                .Returns(Task.CompletedTask);

            _mockTaskRepo.Setup(x => x.SaveChangesAsync())
             .ReturnsAsync(1); // Trả về 1 bản ghi đã lưu thành công

            _mockMapper
                .Setup(x => x.Map<PhaseTaskDto>(It.IsAny<PhaseTask>()))
                .Returns(dto);

            // ✅ Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // ✅ Assert
            Assert.NotNull(result);
            Assert.Equal("Test Task", result.Name);
            Assert.Equal(projectId, result.ProjectId);
            _mockTaskRepo.Verify(x => x.AddAsync(It.IsAny<PhaseTask>()), Times.Once);
            _mockTaskRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithoutPhaseConfig_ThrowsException()
        {
            // ✅ Arrange
            var command = new CreatePhaseTaskCommand
            {
                Request = new CreatePhaseTaskRequest
                {
                    Name = "Test Task",
                    ProjectId = Guid.NewGuid(),
                    ProjectSystemPhaseConfigId = Guid.NewGuid(),
                    AssignedStaffId = Guid.NewGuid(),
                    StartDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = 1,
                    Status = 0
                },
                CreatedBy = "Test User"
            };

            _mockPhaseConfigRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ProjectSystemPhaseConfig?)null);

            // ✅ Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }
    }
}