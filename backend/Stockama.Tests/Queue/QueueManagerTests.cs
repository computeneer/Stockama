using Moq;
using Microsoft.Extensions.Logging;
using Stockama.Core.Exeptions;
using Stockama.Core.Queue;
using Stockama.Core.Queue.Models;

namespace Stockama.Tests.Queue;

public class QueueManagerTests
{
   private readonly Mock<IMessageTemplateManager> _messageTemplateManagerMock;
   private readonly Mock<IQueueTransportManager> _queueTransportManagerMock;
   private readonly Mock<ILogger<QueueManager>> _loggerMock;

   public QueueManagerTests()
   {
      _messageTemplateManagerMock = new Mock<IMessageTemplateManager>();
      _queueTransportManagerMock = new Mock<IQueueTransportManager>();
      _loggerMock = new Mock<ILogger<QueueManager>>();
   }

   [Fact]
   public async Task EnqueueTemplateMessageAsync_ShouldScheduleAndPublishMessage()
   {
      Func<CancellationToken, Task>? queuedWorkItem = null;

      _messageTemplateManagerMock
         .Setup(q => q.RenderAsync("template.key", It.IsAny<Guid>(), It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(new RenderedTemplateMessage("Subject", "Body"));

      var sut = new QueueManager(
         _messageTemplateManagerMock.Object,
         _queueTransportManagerMock.Object,
         _loggerMock.Object);

      await sut.EnqueueTemplateMessageAsync(new QueueTemplateMessageRequest
      {
         QueueName = "queue.name",
         TemplateKey = "template.key",
         Recipient = "dummy@stockama.local",
         LanguageId = Guid.NewGuid(),
         TemplateValues = new Dictionary<string, string>()
      });

      Assert.NotNull(queuedWorkItem);
      await queuedWorkItem!(CancellationToken.None);

      _queueTransportManagerMock.Verify(q =>
         q.PublishAsync(
            "queue.name",
            It.Is<string>(payload => payload.Contains("dummy@stockama.local") && payload.Contains("Subject")),
            It.IsAny<CancellationToken>()),
         Times.Once);
   }

   [Fact]
   public async Task EnqueueTemplateMessageAsync_ShouldThrowValidation_WhenRecipientMissing()
   {
      var sut = new QueueManager(
         _messageTemplateManagerMock.Object,
         _queueTransportManagerMock.Object,
         _loggerMock.Object);

      await Assert.ThrowsAsync<CustomValidationException>(() =>
         sut.EnqueueTemplateMessageAsync(new QueueTemplateMessageRequest
         {
            QueueName = "queue.name",
            TemplateKey = "template.key",
            Recipient = string.Empty
         }));
   }
}
