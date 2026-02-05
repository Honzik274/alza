using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using UI.Template.Components.Basic;
using UI.Template.Framework.Extensions;

namespace UI.Template.Pages;

public class CheckoutPage : BasePage
{
    // =====================================================
    // INPUTS
    // =====================================================

    private readonly Simple FirstName = new(By.Id("firstName"));
    private readonly Simple LastName = new(By.Id("lastName"));
    private readonly Simple Street = new(By.Id("street"));
    private readonly Simple City = new(By.Id("city"));
    private readonly Simple ZipCode = new(By.Id("zipCode"));
    private readonly Simple DateOfBirth = new(By.Id("dateOfBirth"));
    private readonly Simple Email = new(By.Id("email"));
    private readonly Simple PhoneNumber = new(By.Id("phoneNumber"));

    // =====================================================
    // SELECTS
    // =====================================================

    private readonly Simple CountrySelect = new(By.Id("countryCode"));
    private readonly Simple DeliveryMethodSelect = new(By.Id("deliveryMethod"));
    private readonly Simple PaymentMethodSelect = new(By.Id("paymentMethod"));

    // =====================================================
    // ACTION BUTTONS
    // =====================================================

    private readonly Button PayButton =
    new(By.CssSelector("[ko-id='checkout-container'] [ko-id='checkout-form'] [ko-id='form-actions'] [ko-id='pay-button']"));

    // =====================================================
    // ORDER CONFIRMATION (PO PAY)
    // =====================================================

    private readonly Simple OrderConfirmation =
        new(By.CssSelector("[ko-id='order-confirmation']"));

    private readonly Simple OrderPaymentMethod =
        new(By.CssSelector(
            "[ko-id='order-confirmation'] [ko-id='order-paymentMethod']"
        ));

    private readonly Simple OrderDeliveryMethod =
        new(By.CssSelector(
            "[ko-id='order-confirmation'] [ko-id='order-deliveryMethod']"
        ));

    private readonly Simple OrderTotalValue =
        new(By.CssSelector(
            "[ko-id='order-summary-details'] " +
            "[ko-id='order-summary-row-total'] " +
            "[ko-id='order-total-value']"
        ));

    // =====================================================
    // FILL METHODS
    // =====================================================

    public void FillFirstName(string value)
    {
        FirstName.Element.Clear();
        FirstName.Element.SendKeys(value);
    }

    public void FillLastName(string value)
    {
        LastName.Element.Clear();
        LastName.Element.SendKeys(value);
    }

    public void FillStreet(string value)
    {
        Street.Element.Clear();
        Street.Element.SendKeys(value);
    }

    public void FillCity(string value)
    {
        City.Element.Clear();
        City.Element.SendKeys(value);
    }

    public void FillZipCode(string value)
    {
        ZipCode.Element.Clear();
        ZipCode.Element.SendKeys(value);
    }

    public void FillDateOfBirth(string value)
    {
        DateOfBirth.Element.Clear();
        DateOfBirth.Element.SendKeys(value);
    }

    public void FillEmail(string value)
    {
        Email.Element.Clear();
        Email.Element.SendKeys(value);
    }

    public void FillPhoneNumber(string value)
    {
        PhoneNumber.Element.Clear();
        PhoneNumber.Element.SendKeys(value);
    }

    // =====================================================
    // SELECT METHODS
    // =====================================================

    public void SelectCountry(string visibleText)
    {
        new SelectElement(CountrySelect.Element).SelectByText(visibleText);
    }

    public void SelectDeliveryMethod(string visibleText)
    {
        new SelectElement(DeliveryMethodSelect.Element).SelectByText(visibleText);
    }

    public void SelectPaymentMethod(string visibleText)
    {
        new SelectElement(PaymentMethodSelect.Element).SelectByText(visibleText);
    }

    // =====================================================
    // ACTIONS
    // =====================================================

    /// <summary>
    /// Pouze klikne – NIC nečeká
    /// </summary>
    public void Pay()
    {
        PayButton.ScrollTo();
        PayButton.WaitForEnabled();
        PayButton.Click();
    }

    // =====================================================
    // ORDER ASSERT HELPERS
    // =====================================================

    public bool IsOrderConfirmed()
    {
        OrderConfirmation.WaitForDisplayed();
        return OrderConfirmation.Element.Displayed;
    }

    public string GetOrderPaymentMethod()
    {
        OrderPaymentMethod.WaitForDisplayed();

        return OrderPaymentMethod.Element.Text
            .Replace("Payment Method:", "")
            .Trim();
    }

    public string GetOrderDeliveryMethod()
    {
        OrderDeliveryMethod.WaitForDisplayed();

        return OrderDeliveryMethod.Element.Text
            .Replace("Delivery Method:", "")
            .Trim();
    }

    public decimal GetOrderTotal()
    {
        OrderTotalValue.WaitForDisplayed();

        var text = OrderTotalValue.Element.Text
            .Replace("$", "")
            .Trim();

        return decimal.Parse(text, CultureInfo.InvariantCulture);
    }
}
