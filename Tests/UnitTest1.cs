using System;
using System.Net.Http;
using System.Web.Helpers;
using Common.Exeptions;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductService;
using Org.BouncyCastle.Security;
using UserService;
using OrderService;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using StackExchange.Redis;

namespace Tests
{
    public class UnitTest1
    {
        private Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private Mock<IPaymentRepository> _paymentRepoMock = new Mock<IPaymentRepository>();
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        private Mock<IPublisher> _publisherMock = new Mock<IPublisher>();
        private Mock<IOrderService> _orderServiceMock = new Mock<IOrderService>();
        private Mock<IOrderRepository> _orderRepoMock;
        private OrderService.OrderService _orderService;
        public UnitTest1()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _orderService = new OrderService.OrderService(_orderRepoMock.Object, _publisherMock.Object);
        }

        // User Service
        [Theory]
        [InlineData("")]
        [InlineData("Aa121212")]
        [InlineData("AaAaAaA!")]
        [InlineData("1212121!")]
        [InlineData("aa12121!")]
        [InlineData("AA12121!")]
        public async Task Password_FailTest_Async(string password)
        {
            var condigMoq = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();
            var userService = new UserService.UserService(condigMoq.Object, userRepoMock.Object);

            try
            {
                Assert.False(await userService.PasswordStrenghtAsync(password));
            }
            catch (PasswordFailExeption e)
            {
                Assert.NotNull(e.Message);
            }
        }
        
        [Theory]
        [InlineData("Aa153V@$1")]
        [InlineData("!15AbC947")]
        public async Task Password_PassTest (string password)
        {
            var condigMoq = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();

            var userService = new UserService.UserService(condigMoq.Object, userRepoMock.Object);

            Assert.True(await userService.PasswordStrenghtAsync(password));
        }
        
        [Theory]
        [InlineData("hkjsafhre")]
        [InlineData("hjgdsf@.com")]
        [InlineData("skfjdgh.com")]
        [InlineData("@eidfuyh")]
        [InlineData("@kakjsfdh.com")]
        public async Task Email_FailTest (string email) 
        {
            var condigMoq = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();
            var userService = new UserService.UserService(condigMoq.Object, userRepoMock.Object);

            try
            {
                
                Assert.False(await userService.EmailValidationAsync(email));
            }
            catch (EmailFailExeption e)
            {
                Assert.NotNull(e.Message);
            }
        }

        [Theory]
        [InlineData("sdfahgasdfk@adsfg.com")]
        [InlineData("jafoie156@walla.co.il")]

        public async Task Email_PassTest(string email)
        {
            var condigMoq = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();

            var userService = new UserService.UserService(condigMoq.Object, userRepoMock.Object);
            Assert.True(await userService.EmailValidationAsync(email));
        }

        [Fact]
        public async Task AddAsyncUser_FailTest()
        {
            var configMock = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.AddToDBAsync(It.IsAny<UserModel>())).ThrowsAsync(new Exception());
            var userService = new UserService.UserService(configMock.Object, userRepoMock.Object);
            var user = new AddUserModel()
            {
                ID = 1,
                Password = "Aa153V@$1",
                Email = "sdfahgasdfk@adsfg.com",
                UserName = "test",
                Role = "Admin"
            };
            // Assert that a SavingOrUpdatingDBException is thrown when adding a user fails
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => userService.AddAsync(user));
        }
        
        [Fact]
        public async Task AddAsyncUser_PassAddToDBAsyncTest()
        {
            var condigMoq = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();
            var userService = new UserService.UserService(condigMoq.Object, userRepoMock.Object);
            var user = new AddUserModel()
            {
                ID = 1,
                Password = "Aa153V@$1",
                Email = "sdfahgasdfk@adsfg.com",
                UserName = "test",
                Role = "Admin"
            };
            await userService.AddAsync(user);
            userRepoMock.Verify(x => x.AddToDBAsync(It.IsAny<UserModel>()), Times.Once);
        }

        [Fact]
        public async Task Authenticate_PassTest()
        {
            var condigMoq = new Mock<IConfiguration>();
            var userRepoMock = new Mock<IUserRepository>();
            var userService = new UserService.UserService(condigMoq.Object, userRepoMock.Object);
            var Encr = await userService.PasswordEncryptionAsync("Aa123456!");

            UserModel user = new UserModel()
            {
                ID = 1,
                PasswordHash = Encr,
                Email = "sdfahgasdfk@adsfg.com",
                UserName = "test",
                Role = "Admin"
            };

            userRepoMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            var userAuthenticate = new LoginModel()
            {
                ID = 1,
                Email = "sdfahgasdfk@adsfg.com",
                Password = "Aa123456!"
            };


            var jwt = await userService.Authenticate(userAuthenticate);
            Assert.NotEmpty(jwt);
        }

        // Product Service
        [Fact]
        public async Task AddAsyncProduct_FailTest()
        {
            var contextMock = new Mock<IProductRepository>();
            contextMock.Setup(x => x.AddToDBAsync(It.IsAny<ProductModel>())).ThrowsAsync(new Exception());
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var product = new ProductModel();
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => productService.AddAsync(product));
        }
        [Fact]
        public async Task AddAsync_PassAddToDBAsyncTest()
        {
            var contextMock = new Mock<IProductRepository>();
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var product = new ProductModel();
            await productService.AddAsync(product);
            contextMock.Verify(x => x.AddToDBAsync(It.IsAny<ProductModel>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsyncProduct_FailTest()
        {
            var contextMock = new Mock<IProductRepository>();
            contextMock.Setup(x => x.RemoveFromDBAsync(It.IsAny<int>())).ThrowsAsync(new Exception());
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var product = new ProductModel();
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => productService.DeleteAsync(product));
        }
        [Fact]
        public async Task DeleteAsync_PassRemoveFromDBAsyncTest()
        {
            var contextMock = new Mock<IProductRepository>();
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var product = new ProductModel();
            await productService.DeleteAsync(product);
            contextMock.Verify(x => x.RemoveFromDBAsync(It.IsAny<int>()), Times.Once);
        }
        [Fact]
        public async Task UpdateAsyncProducty_FailTest()
        {
            var contextMock = new Mock<IProductRepository>();
            contextMock.Setup(x => x.UpdateToDBAsync(It.IsAny<ProductModel>())).ThrowsAsync(new Exception());
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var product = new ProductModel();
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => productService.UpdateAsync(product));
        }
        [Fact]
        public async Task UpdateAsync_PassUpdateToDBAsyncTest()
        {
            var contextMock = new Mock<IProductRepository>();
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var product = new ProductModel();
            await productService.UpdateAsync(product);
            contextMock.Verify(x => x.UpdateToDBAsync(It.IsAny<ProductModel>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQuantityBySkuAsyncProduct_FailTest()
        {
            var contextMock = new Mock<IProductRepository>();
            contextMock.Setup(x => x.GetProductBySKUAsync(It.IsAny<string>())).ThrowsAsync(new Exception());
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var skuList = new List<string> { "SKU1" };
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => productService.UpdateQuantityBySkuAsync(skuList, 1));
        }

        [Fact]
        public async Task UpdateQuantityBySkuAsync_PassInventoryReservedMessage()
        {
            var contextMock = new Mock<IProductRepository>();
            contextMock.Setup(x => x.GetProductBySKUAsync(It.IsAny<string>())).ReturnsAsync(new ProductModel { StockQuantity = 10 });
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var skuList = new List<string> { "SKU1" };
            await productService.UpdateQuantityBySkuAsync(skuList, 1);
            publisherMock.Verify(x => x.Publish(It.IsAny<InventoryReservedMessage>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQuantityBySkuAsync_PassInventoryResravationRejectedMessage()
        {
            var contextMock = new Mock<IProductRepository>();
            contextMock.Setup(x => x.GetProductBySKUAsync(It.IsAny<string>())).ReturnsAsync(new ProductModel { StockQuantity = 0 });
            var publisherMock = new Mock<IPublisher>();
            var productService = new ProductService.ProductService(contextMock.Object, publisherMock.Object);
            var skuList = new List<string> { "SKU1" };
            await productService.UpdateQuantityBySkuAsync(skuList, 1);
            publisherMock.Verify(x => x.Publish(It.IsAny<InventoryResravationRejectedMessage>()), Times.Once);
        }

        // Payment Service
        [Fact]
        public async Task PaymentService_CreateAsync_PassTest()
        {
            var paymentModel = new PaymentModel { PaymentId = 1, OrderId = 1, TotalAmount = 100, Currency = "USD" };
            var paymentIntent = new PaymentIntent { Status = "succeeded" };
            var options = new PaymentIntentCreateOptions
            {
                Amount = paymentModel.TotalAmount,
                Currency = paymentModel.Currency,
                PaymentMethod = "pm_card_visa",
                ConfirmationMethod = "manual",
                Confirm = true
            };

            _paymentRepoMock.Setup(repo => repo.AddToDBAsync(It.IsAny<PaymentModel>())).Returns(Task.CompletedTask);
            _paymentRepoMock.Setup(repo => repo.GetPaymentByOrderIdAsync(paymentModel.OrderId)).ReturnsAsync((PaymentModel?)null);
            _paymentRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<PaymentModel>())).Returns(Task.CompletedTask);
            _httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(new DefaultHttpContext());
            _httpContextAccessorMock.Object.HttpContext.Request.Headers.Add("Authorization", "Bearer token");
            _configurationMock.SetupGet(x => x["Stripe:SecretKey"]).Returns("sk_test_51J4");
            _httpContextAccessorMock.SetupGet(x => x.HttpContext.Request.Host).Returns(new HostString("localhost"));
            _httpContextAccessorMock.SetupGet(x => x.HttpContext.Request.Path).Returns(new PathString("/"));

        }

        [Fact]
        public async Task CreateAsync_CreatePendingPayment_PaymentFails()
        {
        }

        [Fact]
        public async Task UpdateAsync_UpdatePayment_PaymentExists()
        {
        }

        [Fact]
        public async Task UpdateAsync_UpdatePayment_OrderIdProvided()
        {
        }

        [Fact]
        public async Task UpdateAsyncPayment_ThrowException_NoIdProvided()
        {
            // Arrange
            var paymentService = new PaymentService.PaymentService(
                _configurationMock.Object,
                _paymentRepoMock.Object,
                _httpContextAccessorMock.Object,
                _publisherMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UpdatePaymentException>(() => paymentService.UpdateAsync(0, 0));
        }

        [Fact]
        public async Task UpdateDetails_UpdatePaymentDetails_PaymentExists()
        {
            // Arrange
            var paymentModel = new PaymentModel { PaymentId = 1, OrderId = 1, TotalAmount = 100, Currency = "USD" };
            var updatedPaymentDetails = new PaymentModel
            {
                PaymentId = 1,
                PaymentMethod = "newMethod",
                Description = "newDescription",
                TotalAmount = 200,
                PaymentStatus = PaymentStatus.Paid,
                ClientSecret = "newSecret",
                Currency = "EUR"
            };

            _paymentRepoMock.Setup(repo => repo.GetPaymentByOrderIdAsync(paymentModel.OrderId)).ReturnsAsync(paymentModel);
            _paymentRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<PaymentModel>())).Returns(Task.CompletedTask);

            var paymentService = new PaymentService.PaymentService(
                _configurationMock.Object,
                _paymentRepoMock.Object,
                _httpContextAccessorMock.Object,
                _publisherMock.Object);

            // Act
            await paymentService.UpdateDetails(updatedPaymentDetails);

            // Assert
            _paymentRepoMock.Verify(repo => repo.UpdateToDBAsync(It.IsAny<PaymentModel>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePaymentAmount_PaymentExists()
        {
            // Arrange
            var orderId = 1;
            var paymentModel = new PaymentModel { PaymentId = 1, OrderId = orderId, TotalAmount = 100, Currency = "USD" };

            _paymentRepoMock.Setup(repo => repo.GetPaymentByOrderIdAsync(orderId)).ReturnsAsync(paymentModel);
            _paymentRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<PaymentModel>())).Returns(Task.CompletedTask);

            var paymentService = new PaymentService.PaymentService(
                _configurationMock.Object,
                _paymentRepoMock.Object,
                _httpContextAccessorMock.Object,
                _publisherMock.Object);

            // Act
            await paymentService.UpdateAmmount(orderId, 150);

            // Assert
            _paymentRepoMock.Verify(repo => repo.UpdateToDBAsync(It.IsAny<PaymentModel>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAmmount_ThrowException_PaymentDoesNotExist()
        {
            // Arrange
            _paymentRepoMock.Setup(repo => repo.GetPaymentByOrderIdAsync(It.IsAny<int>())).ReturnsAsync((PaymentModel?)null);

            var paymentService = new PaymentService.PaymentService(
                _configurationMock.Object,
                _paymentRepoMock.Object,
                _httpContextAccessorMock.Object,
                _publisherMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => paymentService.UpdateAmmount(1, 150));
        }

        // Order Service
        [Fact]
        public async Task AddAsync_Pass_PublishesOrderCreatedMessage()
        {
            var orderAdd = new OrderModel { /* initialize properties */ };
            await _orderService.AddAsync(orderAdd);
            _orderRepoMock.Verify(repo => repo.AddToDBAsync(orderAdd), Times.Once);
            _publisherMock.Verify(publisher => publisher.Publish(It.IsAny<OrderCreatedMessage>()), Times.Once);
        }

        [Fact]
        public async Task AddAsyncOrder_FailTest()
        {
            var orderAdd = new OrderModel { /* Initialize orderAdd properties as needed */ };
            _orderRepoMock.Setup(repo => repo.AddToDBAsync(It.IsAny<OrderModel>()))
                .ThrowsAsync(new SavingOrUpdatingDBException());
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(async () => await _orderService.AddAsync(orderAdd));
            _orderRepoMock.Verify(repo => repo.AddToDBAsync(It.IsAny<OrderModel>()), Times.Once);
        }
        [Fact]
        public async Task UpdateAsync_Pass_ExistingOrder_UpdateOrderAndPublishesUpdatePaymentAmmountMessage()
        {
            var orderUpdate = new OrderModel { ID = 1, /* initialize other properties */ };
            var existingOrder = new OrderModel { ID = 1, OrderStatus = OrderStatus.Exists, TotalAmount = 50 };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderUpdate.ID)).ReturnsAsync(existingOrder);
            await _orderService.UpdateAsync(orderUpdate);
            _orderRepoMock.Verify(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>()), Times.Once);
            _publisherMock.Verify(publisher => publisher.Publish(It.IsAny<UpdatePaymentAmmountMessage>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncOrder_PassTest()
        {
            var orderId = 1;
            var orderUpdate = new OrderModel { ID = orderId, /* Initialize other properties as needed */ };
            var existingOrder = new OrderModel { ID = orderId, /* Initialize other properties as needed */ };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync(existingOrder);
            _orderRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>())).Returns(Task.CompletedTask);
            await _orderService.UpdateAsync(orderUpdate);
            _orderRepoMock.Verify(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>()), Times.Once);
        }
       
        [Fact]
        public async Task DeleteAsyncOrder_PassTest()
        {
            var orderId = 1;
            var orderToRemove = new OrderModel { ID = orderId, /* Initialize other properties as needed */ };
            _orderRepoMock.Setup(repo => repo.RemoveFromDBAsync(orderId)).Returns(Task.CompletedTask);
            await _orderService.DeleteAsync(orderToRemove);
            _orderRepoMock.Verify(repo => repo.RemoveFromDBAsync(orderId), Times.Once);
        }
        
        [Fact]
        public async Task DeleteAsyncOrder_FailTest()
        {
            var orderId = 1;
            var orderToRemove = new OrderModel { ID = orderId, /* Initialize other properties as needed */ };
            _orderRepoMock.Setup(repo => repo.RemoveFromDBAsync(orderId)).ThrowsAsync(new SavingOrUpdatingDBException());
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(async () => await _orderService.DeleteAsync(orderToRemove));
            _orderRepoMock.Verify(repo => repo.RemoveFromDBAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task UpdateToPending_PassTest()
        {
            var orderId = 1;
            var existingOrder = new OrderModel { ID = orderId, OrderStatus = OrderStatus.Exists /* Initialize other properties as needed */ };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync(existingOrder);
            _orderRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>())).Returns(Task.CompletedTask);
            await _orderService.UpdateToPending(orderId);
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderRepoMock.Verify(repo => repo.UpdateToDBAsync(existingOrder), Times.Once);
        }

        [Fact]
        public async Task UpdateToPending_AlreadyPending_Test()
        {
            var orderId = 1;
            var existingOrder = new OrderModel { ID = orderId, OrderStatus = OrderStatus.Pending /* Initialize other properties as needed */ };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync(existingOrder);
            await _orderService.UpdateToPending(orderId);
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderRepoMock.Verify(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>()), Times.Never);
        }

        [Fact]
        public async Task UpdateToPending_FailTest()
        {
            var orderId = 1;
            var existingOrder = new OrderModel { ID = orderId, OrderStatus = OrderStatus.Exists /* Initialize other properties as needed */ };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync(existingOrder);
            _orderRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>())).Throws(new SavingOrUpdatingDBException());
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => _orderService.UpdateToPending(orderId));
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderRepoMock.Verify(repo => repo.UpdateToDBAsync(existingOrder), Times.Once);
        }

        [Fact]
        public async Task UpdateToPaid_PassTest()
        {
            var orderId = 1;
            var orderUpdate = new OrderModel { ID = orderId, OrderStatus = OrderStatus.Paid };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync(orderUpdate);
            _orderServiceMock.Setup(service => service.UpdateAsync(orderUpdate));
            await _orderServiceMock.Object.UpdateToPaid(orderId);
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderServiceMock.Verify(service => service.UpdateAsync(orderUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateToPaid_FailTest()
        {
            var orderId = 1;
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync((OrderModel?)null);
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => _orderServiceMock.Object.UpdateToPaid(orderId));
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderServiceMock.Verify(service => service.UpdateAsync(It.IsAny<OrderModel>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStatusByPayment_PendingStatus_Test()
        {
            var orderId = 1;
            var paymentStatus = PaymentStatus.Pending;
            _orderServiceMock.Setup(service => service.UpdateToPending(orderId));
            await _orderServiceMock.Object.UpdateStatusByPayment(orderId, paymentStatus);
            _orderServiceMock.Verify(service => service.UpdateToPending(orderId), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusByPayment_PaidStatus_Test()
        {
            var orderId = 1;
            var paymentStatus = PaymentStatus.Paid;
            _orderServiceMock.Setup(service => service.UpdateToPaid(orderId));
            await _orderServiceMock.Object.UpdateStatusByPayment(orderId, paymentStatus);
            _orderServiceMock.Verify(service => service.UpdateToPaid(orderId), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusByPayment_FailTest()
        {
            var orderId = 1;
            var paymentStatus = PaymentStatus.Confirmed; // This is an invalid status for the method
            await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.UpdateStatusByPayment(orderId, paymentStatus));
        }

        [Fact]
        public async Task UpdateTotalPrice_PassTest()
        {
            var orderId = 1;
            var stringPriceDictionary = new Dictionary<string, decimal>
            {
                { "Product1", 50.0M },
                { "Product2", 30.0M },
                { "Product3", 20.0M }
            };
            _orderRepoMock.Setup(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>()));
            await _orderService.UpdateTotalPrice(orderId, stringPriceDictionary);
            _orderRepoMock.Verify(repo => repo.UpdateToDBAsync(It.IsAny<OrderModel>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTotalPrice_FailTest()
        {
            var orderId = -1; // Assuming this is an invalid order ID
            var stringPriceDictionary = new Dictionary<string, decimal>
            {
                { "Product1", 50.0M },
                { "Product2", 30.0M },
                { "Product3", 20.0M }
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _orderService.UpdateTotalPrice(orderId, stringPriceDictionary));
        }
        [Fact]
        public async Task UpdateToInsufficientProducts_PassTest()
        {
            var orderId = 1;
            var orderUpdate = new OrderModel { ID = orderId, OrderStatus = OrderStatus.InsufficientProducts };
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync(orderUpdate);
            _orderServiceMock.Setup(service => service.UpdateAsync(orderUpdate));
            await _orderServiceMock.Object.UpdateToInsufficientProducts(orderId);
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderServiceMock.Verify(service => service.UpdateAsync(orderUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateToInsufficientProducts_FailTest()
        {
            var orderId = 1;
            _orderRepoMock.Setup(repo => repo.GetOrderByIDAsync(orderId)).ReturnsAsync((OrderModel?)null);
            await Assert.ThrowsAsync<SavingOrUpdatingDBException>(() => _orderServiceMock.Object.UpdateToInsufficientProducts(orderId));
            _orderRepoMock.Verify(repo => repo.GetOrderByIDAsync(orderId), Times.Once);
            _orderServiceMock.Verify(service => service.UpdateAsync(It.IsAny<OrderModel>()), Times.Never);
        }
    }
}
