﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolShipmentPackageDto
    {
        public TrendyolAddressDto ShipmentAddress { get; set; }
        public string OrderNumber { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTyDiscount { get; set; }
        public string TaxNumber { get; set; }
        public TrendyolAddressDto InvoiceAddress { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerEmail { get; set; }
        public long CustomerId { get; set; }
        public string CustomerLastName { get; set; }
        public long Id { get; set; } // shipmentPackageId
        public string CargoTrackingNumber { get; set; }
        public string CargoTrackingLink { get; set; }
        public string CargoSenderNumber { get; set; }
        public string CargoProviderName { get; set; }
        public List<TrendyolOrderLineDto> Lines { get; set; } = new List<TrendyolOrderLineDto>();
        public long OrderDate { get; set; }
        public string IdentityNumber { get; set; }
        public string CurrencyCode { get; set; }
        public List<TrendyolPackageHistoryDto> PackageHistories { get; set; } = new List<TrendyolPackageHistoryDto>();
        public string ShipmentPackageStatus { get; set; }
        public string Status { get; set; }
        public string DeliveryType { get; set; }
        public int TimeSlotId { get; set; }
        public string ScheduledDeliveryStoreId { get; set; }
        public long EstimatedDeliveryStartDate { get; set; }
        public long EstimatedDeliveryEndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddressType { get; set; }
        public long AgreedDeliveryDate { get; set; }
        public bool AgreedDeliveryDateExtendible { get; set; }
        public long? ExtendedAgreedDeliveryDate { get; set; }
        public long? AgreedDeliveryExtensionStartDate { get; set; }
        public long? AgreedDeliveryExtensionEndDate { get; set; }
        public string InvoiceLink { get; set; }
        public bool FastDelivery { get; set; }
        public string FastDeliveryType { get; set; }
        public long OriginShipmentDate { get; set; }
        public long LastModifiedDate { get; set; }
        public bool Commercial { get; set; }
        public bool DeliveredByService { get; set; }
        public bool Micro { get; set; }
        public bool GiftBoxRequested { get; set; }
        public string EtgbNo { get; set; }
        public long? EtgbDate { get; set; }
        public bool _3pByTrendyol { get; set; }
        public bool ContainsDangerousProduct { get; set; }
        public decimal CargoDeci { get; set; }
        public bool IsCod { get; set; }
        public string CreatedBy { get; set; }
        public List<long> OriginPackageIds { get; set; }
    }
}
