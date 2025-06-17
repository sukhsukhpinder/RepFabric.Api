using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepFabric.Api.Models.Response
{
    /// <summary>
    /// Represents the response data for a purchase order, including order details,
    /// customer and principal information, financials, related entities, and line items.
    /// </summary>
    public class PurchaseOrderResponse
    {
        /// <summary>Gets or sets the unique identifier of the purchase order.</summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>Gets or sets the purchase order number.</summary>
        [JsonPropertyName("po-number")]
        public string PoNumber { get; set; }

        /// <summary>Gets or sets the date of the purchase order.</summary>
        [JsonPropertyName("date")]
        public string Date { get; set; }

        /// <summary>Gets or sets the sales team ID associated with the order.</summary>
        [JsonPropertyName("sales-team-id")]
        public int SalesTeamId { get; set; }

        /// <summary>Gets or sets the sales team name associated with the order.</summary>
        [JsonPropertyName("sales-team-name")]
        public string SalesTeamName { get; set; }

        /// <summary>Gets or sets the customer ID.</summary>
        [JsonPropertyName("customer-id")]
        public int CustomerId { get; set; }

        /// <summary>Gets or sets the customer name.</summary>
        [JsonPropertyName("customer-name")]
        public string CustomerName { get; set; }

        /// <summary>Gets or sets the principal ID.</summary>
        [JsonPropertyName("principal-id")]
        public int PrincipalId { get; set; }

        /// <summary>Gets or sets the principal name.</summary>
        [JsonPropertyName("principal-name")]
        public string PrincipalName { get; set; }

        /// <summary>Gets or sets the distributor ID.</summary>
        [JsonPropertyName("distributor-id")]
        public int DistributorId { get; set; }

        /// <summary>Gets or sets the distributor name.</summary>
        [JsonPropertyName("distributor-name")]
        public string DistributorName { get; set; }

        /// <summary>Gets or sets the program name or code associated with the order.</summary>
        [JsonPropertyName("program")]
        public string Program { get; set; }

        /// <summary>Gets or sets the secondary customer ID.</summary>
        [JsonPropertyName("secondary-customer-id")]
        public int SecondaryCustomerId { get; set; }

        /// <summary>Gets or sets the secondary customer name.</summary>
        [JsonPropertyName("secondary-customer-name")]
        public string SecondaryCustomerName { get; set; }

        /// <summary>Gets or sets the close status name of the order.</summary>
        [JsonPropertyName("close-status-name")]
        public string CloseStatusName { get; set; }

        /// <summary>Gets or sets the total price of the purchase order.</summary>
        [JsonPropertyName("total-price")]
        public decimal TotalPrice { get; set; }

        /// <summary>Gets or sets the commission rate for the order.</summary>
        [JsonPropertyName("commission-rate")]
        public decimal CommissionRate { get; set; }

        /// <summary>Gets or sets the projected commission for the order.</summary>
        [JsonPropertyName("commission-projected")]
        public decimal CommissionProjected { get; set; }

        /// <summary>Gets or sets the sales order number.</summary>
        [JsonPropertyName("so-number")]
        public string SoNumber { get; set; }

        /// <summary>Gets or sets the sales order date.</summary>
        [JsonPropertyName("so-date")]
        public string? SoDate { get; set; }

        /// <summary>Gets or sets the linked job ID.</summary>
        [JsonPropertyName("linked-job")]
        public int LinkedJob { get; set; }

        /// <summary>Gets or sets the linked job name.</summary>
        [JsonPropertyName("linked-job-name")]
        public string LinkedJobName { get; set; }

        /// <summary>Gets or sets the linked opportunity ID.</summary>
        [JsonPropertyName("linked-opp")]
        public int LinkedOpp { get; set; }

        /// <summary>Gets or sets the linked opportunity name.</summary>
        [JsonPropertyName("linked-opp-name")]
        public string LinkedOppName { get; set; }

        /// <summary>Gets or sets the linked quote ID.</summary>
        [JsonPropertyName("linked-quote")]
        public int LinkedQuote { get; set; }

        /// <summary>Gets or sets the linked quote name.</summary>
        [JsonPropertyName("linked-quote-name")]
        public string LinkedQuoteName { get; set; }

        /// <summary>Gets or sets the list of email addresses associated with the order.</summary>
        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; }

        /// <summary>Gets or sets the list of line items in the purchase order.</summary>
        [JsonPropertyName("line-items")]
        public List<PurchaseOrderLineItemResponse> LineItems { get; set; }

        /// <summary>Gets or sets the purchase order timeline events.</summary>
        [JsonPropertyName("po-timeline")]
        public List<object> PoTimeline { get; set; }

        /// <summary>Gets or sets the commission rate settings for the order.</summary>
        [JsonPropertyName("commission-rate-settings")]
        public List<object> CommissionRateSettings { get; set; }

        /// <summary>Gets or sets the billing address line 1.</summary>
        [JsonPropertyName("billto-address1")]
        public string BillToAddress1 { get; set; }

        /// <summary>Gets or sets the billing city.</summary>
        [JsonPropertyName("billto-city")]
        public string BillToCity { get; set; }

        /// <summary>Gets or sets the billing state.</summary>
        [JsonPropertyName("billto-state")]
        public string BillToState { get; set; }

        /// <summary>Gets or sets the billing ZIP code.</summary>
        [JsonPropertyName("billto-zip")]
        public string BillToZip { get; set; }

        /// <summary>Gets or sets the billing country.</summary>
        [JsonPropertyName("billto-country")]
        public string BillToCountry { get; set; }

        /// <summary>Gets or sets the shipping address line 1.</summary>
        [JsonPropertyName("shipto-address1")]
        public string ShipToAddress1 { get; set; }

        /// <summary>Gets or sets the shipping city.</summary>
        [JsonPropertyName("shipto-city")]
        public string ShipToCity { get; set; }

        /// <summary>Gets or sets the shipping state.</summary>
        [JsonPropertyName("shipto-state")]
        public string ShipToState { get; set; }

        /// <summary>Gets or sets the shipping ZIP code.</summary>
        [JsonPropertyName("shipto-zip")]
        public string ShipToZip { get; set; }

        /// <summary>Gets or sets the shipping country.</summary>
        [JsonPropertyName("shipto-country")]
        public string ShipToCountry { get; set; }

        /// <summary>Gets or sets the shipping details.</summary>
        [JsonPropertyName("ship-details")]
        public string ShipDetails { get; set; }

        /// <summary>Gets or sets the freight details.</summary>
        [JsonPropertyName("freight-details")]
        public string FreightDetails { get; set; }

        /// <summary>Gets or sets any notes associated with the order.</summary>
        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        /// <summary>Gets or sets the data source for the order.</summary>
        [JsonPropertyName("data-source")]
        public string DataSource { get; set; }

        /// <summary>Gets or sets the freight cost for the order.</summary>
        [JsonPropertyName("freight-cost")]
        public decimal FreightCost { get; set; }

        /// <summary>Gets or sets the creation time of the order.</summary>
        [JsonPropertyName("created-time")]
        public string CreatedTime { get; set; }

        /// <summary>Gets or sets the last modified time of the order.</summary>
        [JsonPropertyName("last-modified-time")]
        public string LastModifiedTime { get; set; }
    }

    /// <summary>
    /// Represents a line item within a purchase order, including product, quantity, pricing, and shipping details.
    /// </summary>
    public class PurchaseOrderLineItemResponse
    {
        /// <summary>Gets or sets the unique identifier of the line item.</summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>Gets or sets the part number.</summary>
        [JsonPropertyName("part-num")]
        public string PartNum { get; set; }

        /// <summary>Gets or sets the customer part number.</summary>
        [JsonPropertyName("cust-part")]
        public string CustPart { get; set; }

        /// <summary>Gets or sets the product description.</summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the product family name.</summary>
        [JsonPropertyName("product-family")]
        public string ProductFamily { get; set; }

        /// <summary>Gets or sets the product family ID.</summary>
        [JsonPropertyName("product-family-id")]
        public string ProductFamilyId { get; set; }

        /// <summary>Gets or sets the ordered quantity.</summary>
        [JsonPropertyName("order-qty")]
        public int OrderQty { get; set; }

        /// <summary>Gets or sets the standard rate for the item.</summary>
        [JsonPropertyName("std-rate")]
        public decimal StdRate { get; set; }

        /// <summary>Gets or sets the multiplier applied to the rate.</summary>
        [JsonPropertyName("multiplier")]
        public decimal Multiplier { get; set; }

        /// <summary>Gets or sets the unit of measure.</summary>
        [JsonPropertyName("uom")]
        public string Uom { get; set; }

        /// <summary>Gets or sets the number of units per unit of measure.</summary>
        [JsonPropertyName("uom-units")]
        public decimal UomUnits { get; set; }

        /// <summary>Gets or sets the unit price.</summary>
        [JsonPropertyName("unit-price")]
        public decimal UnitPrice { get; set; }

        /// <summary>Gets or sets the total price for the line item.</summary>
        [JsonPropertyName("total-price")]
        public decimal TotalPrice { get; set; }

        /// <summary>Gets or sets the quantity being shipped.</summary>
        [JsonPropertyName("shipping-qty")]
        public decimal ShippingQty { get; set; }

        /// <summary>Gets or sets the shipping status name.</summary>
        [JsonPropertyName("shipping-status-name")]
        public string ShippingStatusName { get; set; }

        /// <summary>Gets or sets the shipping date, if available.</summary>
        [JsonPropertyName("shipping-date")]
        public string? ShippingDate { get; set; }

        /// <summary>Gets or sets the invoice number.</summary>
        [JsonPropertyName("invc-num")]
        public string InvcNum { get; set; }

        /// <summary>Gets or sets the invoice date, if available.</summary>
        [JsonPropertyName("invc-date")]
        public string? InvcDate { get; set; }

        /// <summary>Gets or sets the commission rate for the line item.</summary>
        [JsonPropertyName("comm-rate")]
        public decimal CommRate { get; set; }

        /// <summary>Gets or sets the projected commission for the line item.</summary>
        [JsonPropertyName("comm-projected")]
        public decimal CommProjected { get; set; }

        /// <summary>Gets or sets the planned quantity.</summary>
        [JsonPropertyName("plan-qty")]
        public decimal PlanQty { get; set; }

        /// <summary>Gets or sets the accounting date, if available.</summary>
        [JsonPropertyName("accounting-date")]
        public string? AccountingDate { get; set; }

        /// <summary>Gets or sets the planned date, if available.</summary>
        [JsonPropertyName("plan-date")]
        public string? PlanDate { get; set; }

        /// <summary>Gets or sets the unit cost.</summary>
        [JsonPropertyName("unit-cost")]
        public decimal UnitCost { get; set; }

        /// <summary>Gets or sets the overage percent for the line item.</summary>
        [JsonPropertyName("overage-percent")]
        public decimal OveragePercent { get; set; }

        /// <summary>Gets or sets the overage amount for the line item.</summary>
        [JsonPropertyName("overage-amount")]
        public decimal OverageAmount { get; set; }

        /// <summary>Gets or sets the brand name for the line item.</summary>
        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        /// <summary>Gets or sets the creation time of the line item.</summary>
        [JsonPropertyName("created-time")]
        public string CreatedTime { get; set; }

        /// <summary>Gets or sets the last modified time of the line item.</summary>
        [JsonPropertyName("last-modified-time")]
        public string LastModifiedTime { get; set; }
    }
}