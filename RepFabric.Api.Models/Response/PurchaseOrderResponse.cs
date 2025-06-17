using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepFabric.Api.Models.Response
{
    public class PurchaseOrderResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("po-number")]
        public string PoNumber { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("sales-team-id")]
        public int SalesTeamId { get; set; }

        [JsonPropertyName("sales-team-name")]
        public string SalesTeamName { get; set; }

        [JsonPropertyName("customer-id")]
        public int CustomerId { get; set; }

        [JsonPropertyName("customer-name")]
        public string CustomerName { get; set; }

        [JsonPropertyName("principal-id")]
        public int PrincipalId { get; set; }

        [JsonPropertyName("principal-name")]
        public string PrincipalName { get; set; }

        [JsonPropertyName("distributor-id")]
        public int DistributorId { get; set; }

        [JsonPropertyName("distributor-name")]
        public string DistributorName { get; set; }

        [JsonPropertyName("program")]
        public string Program { get; set; }

        [JsonPropertyName("secondary-customer-id")]
        public int SecondaryCustomerId { get; set; }

        [JsonPropertyName("secondary-customer-name")]
        public string SecondaryCustomerName { get; set; }

        [JsonPropertyName("close-status-name")]
        public string CloseStatusName { get; set; }

        [JsonPropertyName("total-price")]
        public decimal TotalPrice { get; set; }

        [JsonPropertyName("commission-rate")]
        public decimal CommissionRate { get; set; }

        [JsonPropertyName("commission-projected")]
        public decimal CommissionProjected { get; set; }

        [JsonPropertyName("so-number")]
        public string SoNumber { get; set; }

        [JsonPropertyName("so-date")]
        public string? SoDate { get; set; }

        [JsonPropertyName("linked-job")]
        public int LinkedJob { get; set; }

        [JsonPropertyName("linked-job-name")]
        public string LinkedJobName { get; set; }

        [JsonPropertyName("linked-opp")]
        public int LinkedOpp { get; set; }

        [JsonPropertyName("linked-opp-name")]
        public string LinkedOppName { get; set; }

        [JsonPropertyName("linked-quote")]
        public int LinkedQuote { get; set; }

        [JsonPropertyName("linked-quote-name")]
        public string LinkedQuoteName { get; set; }

        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; }

        [JsonPropertyName("line-items")]
        public List<PurchaseOrderLineItemResponse> LineItems { get; set; }

        [JsonPropertyName("po-timeline")]
        public List<object> PoTimeline { get; set; }

        [JsonPropertyName("commission-rate-settings")]
        public List<object> CommissionRateSettings { get; set; }

        [JsonPropertyName("billto-address1")]
        public string BillToAddress1 { get; set; }

        [JsonPropertyName("billto-city")]
        public string BillToCity { get; set; }

        [JsonPropertyName("billto-state")]
        public string BillToState { get; set; }

        [JsonPropertyName("billto-zip")]
        public string BillToZip { get; set; }

        [JsonPropertyName("billto-country")]
        public string BillToCountry { get; set; }

        [JsonPropertyName("shipto-address1")]
        public string ShipToAddress1 { get; set; }

        [JsonPropertyName("shipto-city")]
        public string ShipToCity { get; set; }

        [JsonPropertyName("shipto-state")]
        public string ShipToState { get; set; }

        [JsonPropertyName("shipto-zip")]
        public string ShipToZip { get; set; }

        [JsonPropertyName("shipto-country")]
        public string ShipToCountry { get; set; }

        [JsonPropertyName("ship-details")]
        public string ShipDetails { get; set; }

        [JsonPropertyName("freight-details")]
        public string FreightDetails { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("data-source")]
        public string DataSource { get; set; }

        [JsonPropertyName("freight-cost")]
        public decimal FreightCost { get; set; }

        [JsonPropertyName("created-time")]
        public string CreatedTime { get; set; }

        [JsonPropertyName("last-modified-time")]
        public string LastModifiedTime { get; set; }
    }

    public class PurchaseOrderLineItemResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("part-num")]
        public string PartNum { get; set; }

        [JsonPropertyName("cust-part")]
        public string CustPart { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("product-family")]
        public string ProductFamily { get; set; }

        [JsonPropertyName("product-family-id")]
        public string ProductFamilyId { get; set; }

        [JsonPropertyName("order-qty")]
        public int OrderQty { get; set; }

        [JsonPropertyName("std-rate")]
        public decimal StdRate { get; set; }

        [JsonPropertyName("multiplier")]
        public decimal Multiplier { get; set; }

        [JsonPropertyName("uom")]
        public string Uom { get; set; }

        [JsonPropertyName("uom-units")]
        public decimal UomUnits { get; set; }

        [JsonPropertyName("unit-price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("total-price")]
        public decimal TotalPrice { get; set; }

        [JsonPropertyName("shipping-qty")]
        public decimal ShippingQty { get; set; }

        [JsonPropertyName("shipping-status-name")]
        public string ShippingStatusName { get; set; }

        [JsonPropertyName("shipping-date")]
        public string? ShippingDate { get; set; }

        [JsonPropertyName("invc-num")]
        public string InvcNum { get; set; }

        [JsonPropertyName("invc-date")]
        public string? InvcDate { get; set; }

        [JsonPropertyName("comm-rate")]
        public decimal CommRate { get; set; }

        [JsonPropertyName("comm-projected")]
        public decimal CommProjected { get; set; }

        [JsonPropertyName("plan-qty")]
        public decimal PlanQty { get; set; }

        [JsonPropertyName("accounting-date")]
        public string? AccountingDate { get; set; }

        [JsonPropertyName("plan-date")]
        public string? PlanDate { get; set; }

        [JsonPropertyName("unit-cost")]
        public decimal UnitCost { get; set; }

        [JsonPropertyName("overage-percent")]
        public decimal OveragePercent { get; set; }

        [JsonPropertyName("overage-amount")]
        public decimal OverageAmount { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("created-time")]
        public string CreatedTime { get; set; }

        [JsonPropertyName("last-modified-time")]
        public string LastModifiedTime { get; set; }
    }
}