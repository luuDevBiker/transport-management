using AutoMapper;
using Transport.Application.DTOs.Customer;
using Transport.Application.DTOs.Driver;
using Transport.Application.DTOs.Invoice;
using Transport.Application.DTOs.Payment;
using Transport.Application.DTOs.Trip;
using Transport.Application.DTOs.Truck;
using Transport.Domain.Entities;

namespace Transport.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer mappings
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();

        // Truck mappings
        CreateMap<Truck, TruckDto>();
        CreateMap<CreateTruckDto, Truck>();

        // Driver mappings
        CreateMap<Driver, DriverDto>();
        CreateMap<CreateDriverDto, Driver>();

        // Trip mappings
        CreateMap<Trip, TripDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.TruckLicensePlate, opt => opt.MapFrom(src => src.Truck.LicensePlate))
            .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.FullName));
        CreateMap<CreateTripDto, Trip>();

        // Invoice mappings
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.TripNumber, opt => opt.MapFrom(src => src.Trip != null ? src.Trip.TripNumber : null))
            .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => src.Payments.Sum(p => p.Amount)))
            .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.TotalAmount - src.Payments.Sum(p => p.Amount)));
        CreateMap<CreateInvoiceDto, Invoice>();

        // Payment mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.Invoice.InvoiceNumber));
        CreateMap<CreatePaymentDto, Payment>();
    }
}

