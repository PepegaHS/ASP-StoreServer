using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;

namespace StoreServer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cart, CartDTO>();

            CreateMap<CreateCartDTO, Cart>()
                .ForMember(dest => dest.IdCart, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<UpdateCartDTO, Cart>()
                .ForMember(dest => dest.IdCart, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<CartItem, CartItemDTO>();

            CreateMap<CreateCartItemDTO, CartItem>()
                .ForMember(dest => dest.IdCartItem, opt => opt.Ignore())
                .ForMember(dest => dest.Cart, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<UpdateCartItemDTO, CartItem>()
                .ForMember(dest => dest.IdCartItem, opt => opt.Ignore())
                .ForMember(dest => dest.Cart, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<Category, CategoryDTO>();

            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.IdCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.IdCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<Order, OrderDTO>();

            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.IdOrder, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<UpdateOrderDTO, Order>()
                .ForMember(dest => dest.IdOrder, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<OrderItem, OrderItemDTO>();

            CreateMap<CreateOrderItemDTO, OrderItem>()
                .ForMember(dest => dest.IdOrderItem, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<UpdateOrderItemDTO, OrderItem>()
                .ForMember(dest => dest.IdOrderItem, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<Product, ProductDTO>();

            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.IdProduct, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());

            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.IdProduct, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());

            CreateMap<Role, RoleDTO>();

            CreateMap<CreateRoleDTO, Role>()
                .ForMember(dest => dest.IdRole, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore());

            CreateMap<UpdateRoleDTO, Role>()
                .ForMember(dest => dest.IdRole, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore());

            CreateMap<User, UserDTO>();

            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.IdUser, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Carts, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.IdUser, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Carts, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Reservations, opt => opt.Ignore());
            CreateMap<Role, RoleSummaryDTO>();
            CreateMap<User, UserSummaryDTO>();
            CreateMap<Category, CategorySummaryDTO>();
            CreateMap<Product, ProductSummaryDTO>();
            CreateMap<Cart, CartSummaryDTO>();
            CreateMap<Order, OrderSummaryDTO>();
            CreateMap<User, LoginResponseDTO>();
            CreateMap<ProductReservation, ProductReservationDTO>();
            CreateMap<CreateProductReservationDTO, ProductReservation>()
                .ForMember(dest => dest.IdProductReservation, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledAt, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<UpdateProductReservationDTO, ProductReservation>()
                .ForMember(dest => dest.IdProductReservation, opt => opt.Ignore())
                .ForMember(dest => dest.IdProduct, opt => opt.Ignore())
                .ForMember(dest => dest.IdUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledAt, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
